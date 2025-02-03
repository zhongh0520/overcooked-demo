using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KichenGameMultiPlayer : NetworkBehaviour
{
    private const int MAX_PLAYER_AMOUNT = 4;
   public static KichenGameMultiPlayer Instance{get; private set;}
   [SerializeField] private KitchenGameObjectListSO kitchenGameObjectListSO;
   [SerializeField] private List<Color> playerColorList;
   private NetworkList<PlayerData> playerDataNetworkList;
   public event EventHandler OnTryingToJoinGame;
   public event EventHandler OnFailedToJoinGame;
   public event EventHandler OnPlayerDataNetworkListChanged;

   private void Awake()
   {
      Instance = this;
      DontDestroyOnLoad(gameObject);
      playerDataNetworkList = new NetworkList<PlayerData>();
      playerDataNetworkList.OnListChanged += playerDataNetworkList_OnListChanged;
   }

   private void playerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeevent)
   {
       OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
   }

   public void StartHost()
   {
       NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
       NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
       NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
       NetworkManager.Singleton.StartHost();
   }

   private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientID)
   {
       for (int i = 0; i < playerDataNetworkList.Count; i++)
       {
           PlayerData playerData = playerDataNetworkList[i];
           if (playerData.clientID == clientID)
           {
               playerDataNetworkList.RemoveAt(i);
           }
       }
   }

   private void NetworkManager_OnClientConnectedCallback(ulong clietnentId)
   {
       playerDataNetworkList.Add(new PlayerData()
       {
           clientID = clietnentId,
           colorID = GetFirstUnusedColorId()
       });
   }

   private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse) {
       if (SceneManager.GetActiveScene().name != Loader.Scence.CharacterSelectScene.ToString()) {
           connectionApprovalResponse.Approved = false;
           connectionApprovalResponse.Reason = "Game Start";
           return;
       }
       if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT) {
           connectionApprovalResponse.Approved = false;
           connectionApprovalResponse.Reason = "Game is full";
           return;
       }

       connectionApprovalResponse.Approved = true;

   }

   public void StartClient()
   {
       OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
       NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
       NetworkManager.Singleton.StartClient();
   }

   private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientID)
   {
       OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
       
   }

   public  void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO,IkitchenObjectParents kitchenObjectParents)
   {
       SpawnKitchenObjectsServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO),kitchenObjectParents.GetNetworkObject());
       
   }

   [ServerRpc(RequireOwnership = false)]
   private void SpawnKitchenObjectsServerRpc(int  kitchenObjectSOIndex,NetworkObjectReference kitchenObjectParentsNetworkObjectReference)
   {
       KitchenObjectSO kitchenObjectSO=GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
       Transform kitchenObjectTransform  =Instantiate(kitchenObjectSO.prefab);
       NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
       kitchenObjectNetworkObject.Spawn(true);
       KitcheObject kitcheObject = kitchenObjectTransform.GetComponent<KitcheObject>();
       
       //获取网络物体
       kitchenObjectParentsNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentsNetworkObject);
       IkitchenObjectParents kitchenObjectParents =
           kitchenObjectParentsNetworkObject.GetComponent<IkitchenObjectParents>();
       kitcheObject.SetKitchenObjectParents(kitchenObjectParents);
   }

   public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO)
   {
       return kitchenGameObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
   }

   public KitchenObjectSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex)
   {
       return kitchenGameObjectListSO.kitchenObjectSOList[kitchenObjectSOIndex];
   }

   public void DestroyKitchenObjects(KitcheObject kitcheObject)
   {
       DestroyKitchenObjectsServerRpc(kitcheObject.NetworkObject);
   }

   [ServerRpc(RequireOwnership = false)]
   private void DestroyKitchenObjectsServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
   {
       kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
       KitcheObject kitcheObject = kitchenObjectNetworkObject.GetComponent<KitcheObject>();
       ClearKitchenObjectOnParentClientRpc(kitchenObjectNetworkObjectReference);
       kitcheObject.DestroySelf();
   }
   
   
    [ClientRpc]
   private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
   {
       kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
       KitcheObject kitcheObject = kitchenObjectNetworkObject.GetComponent<KitcheObject>();
       kitcheObject.ClearKitchenObjectOnParents();
   }

    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerDataNetworkList.Count;
    }
    public int GetPlayerDataIndexFromClientId(ulong clientId) {
        for (int i=0; i< playerDataNetworkList.Count; i++) {
            if (playerDataNetworkList[i].clientID == clientId) {
                return i;
                
            }
        }
        return -1;
    }

    public PlayerData GetPlayerDataFromClientID(ulong clientID)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientID == clientID)
            {
                return playerData;
                
            }
        }   
        return default;
    }
        
    public PlayerData GetPlayerDataFromIndex(int playerIndex)
    {
        return  playerDataNetworkList[playerIndex];
    }

    public Color GetPlayerColor(int colorID)
    {
        return playerColorList[colorID];
    }



    private bool IsColorAvailable(int colorId) {
        foreach (PlayerData playerData in playerDataNetworkList) {
            if (playerData.colorID == colorId) {
                return false;
            }
        }
        return true;
    }

    private int GetFirstUnusedColorId() {
        for (int i = 0; i<playerColorList.Count; i++) {
            if (IsColorAvailable(i)) {
                return i;
            }
        }
        return -1;
    }


    
}
