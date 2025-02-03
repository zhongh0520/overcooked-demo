using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance{get; private set;}
    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnPaused;
    public event EventHandler OnMultiplayerGamePaused;
    public event EventHandler OnMultiplayerGameUnpaused;
    public event EventHandler OnLocalPlayerRreadyChange;
    enum State
    {
        WaitingToStart,
        CountdownToStart,
        Gameplaying,
        GameOver
    }

    [SerializeField] private Transform playerPrefab;

    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    private bool isLocalPlayerRready;
    private NetworkVariable<float> countdownToStartTime = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gameplayingTime = new NetworkVariable<float>(0f);
    private float gameplayingTimeMax = 130f;
    private bool isLocalGamePaused = false;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
    private Dictionary<ulong, bool> playerReadyDict;
    private Dictionary<ulong, bool> playerPauseDict;
    private bool autoTestGamePausedState;

    private void Awake()
    {
        Instance = this;
        
        playerReadyDict = new Dictionary<ulong, bool>();
        playerPauseDict = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnInteractAlernateAction += GameInput_OnInteractAlernateAction;
        
    }

    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += NetworkManager_OnLoadEventCompleted;
        }
    }

    private void NetworkManager_OnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, true);
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        autoTestGamePausedState = true;
    }

    private void IsGamePaused_OnValueChanged(bool previousvalue, bool newvalue)
    {
        if (isGamePaused.Value)
        {
            Time.timeScale = 0f;
            OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f; 
            OnMultiplayerGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    private void State_OnValueChanged(State previousvalue, State newvalue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void GameInput_OnInteractAlernateAction(object sender, EventArgs e)
    {
        if(state.Value == State.WaitingToStart)
        {
            isLocalPlayerRready = true;
            OnLocalPlayerRreadyChange?.Invoke(this, EventArgs.Empty);
            SetPlayerReadyServerRpc();
        }
    }

    //建立多人准备
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDict[serverRpcParams.Receive.SenderClientId] = true;
        bool allClientReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDict.ContainsKey(clientId)||!playerReadyDict[clientId])
            {
                allClientReady = false;
                break;
            }
        }
        if(allClientReady)
        {
         state.Value = State.CountdownToStart;   
        }
    }
    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    private void Update()
    {
        if(!IsServer)return;
        switch (state.Value)
        {
            case State.WaitingToStart:
                
                break;
            case State.CountdownToStart:
                countdownToStartTime.Value -= Time.deltaTime;
                if (countdownToStartTime.Value < 0)
                {
                    state.Value =  State.Gameplaying;
                    gameplayingTime.Value = gameplayingTimeMax;
                    
                }
                break;
            case State.Gameplaying:
                gameplayingTime.Value -= Time.deltaTime;
                if (gameplayingTime.Value < 0)
                {
                    state.Value =  State.GameOver;
                    
                }
                break;
            case State.GameOver:
                break;
        }
        
    }

    private void LateUpdate()
    {
        if (autoTestGamePausedState)
        {
            autoTestGamePausedState = false;
            TestGamePausedState();
        }
    }

    public bool IsGameplaying()
    {
        return state.Value == State.Gameplaying;
    }

    public bool IsCountingDownToStartActive()
    {
        return state.Value == State.CountdownToStart;
    }

    public float GetCountdownToStartTime()
    {
        return countdownToStartTime.Value;
    }

    public bool IsGameOver()
    {
        return state.Value == State.GameOver;
    }

    public bool IsWaitingToStart()
    {
        return state.Value == State.WaitingToStart;
    }

    public bool IsLocalPlayerRready()
    {
        return isLocalPlayerRready;
    }

    public float GetGameplayingTime()
    {
        return 1-(gameplayingTime.Value/gameplayingTimeMax);
    }

    public void TogglePauseGame()
    {
        isLocalGamePaused = !isLocalGamePaused;
        if (isLocalGamePaused)
        {
            PauseGameServerRpc();
            OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
        }
        //流速
        else
        {
            UnPauseGameServerRpc();
            OnLocalGameUnPaused?.Invoke(this, EventArgs.Empty);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPauseDict[serverRpcParams.Receive.SenderClientId] = true;
        TestGamePausedState();
    }
    [ServerRpc(RequireOwnership = false)]
    private void UnPauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPauseDict[serverRpcParams.Receive.SenderClientId] = false;
        TestGamePausedState();
    }
    
    private void TestGamePausedState() {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (playerPauseDict.ContainsKey(clientId) && playerPauseDict[clientId]) {
                isGamePaused.Value = true;
                return;
            }
        }

        isGamePaused.Value = false;
    }
    
}
