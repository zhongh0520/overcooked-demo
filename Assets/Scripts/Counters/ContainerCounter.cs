using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    //玩家拿起
    public event EventHandler OnPlayerGrabbedObject;
    [SerializeField]private KitchenObjectSO kitchenObjectSo; 

    //检测玩家手里是否有物品，有不可以再拿
    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            KitcheObject.SpawnKitchenObject(kitchenObjectSo,player);
            InteractLogicServerRpc();
        }
        
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
            InteractLogicClientRpc();
    }
    
    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
    }

}
