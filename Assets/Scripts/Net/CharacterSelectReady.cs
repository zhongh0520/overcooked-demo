using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }
    private Dictionary<ulong, bool> playerReadyDict;
    public event EventHandler OnReadyChanged;

    private void Awake()
    {
        Instance = this;
        playerReadyDict = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }
    //建立多人准备
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
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
            Loader.LoadNetwork(Loader.Scence.GameScence);
        }
    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        playerReadyDict[clientId] = true;
        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientId)
    {
        return playerReadyDict.ContainsKey(clientId)&&playerReadyDict[clientId];
    }
}
