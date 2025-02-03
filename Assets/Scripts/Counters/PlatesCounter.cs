using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawaned;
    public event EventHandler OnPlateRemoved;
    [SerializeField] private KitchenObjectSO plateKitchenObjectSo;
    private float spawnPlateTimer;
    private float spawnPlateTimerMax=4f;
    private int plateSpawanAmount;
    private int plateSpawanAmountMax=4;

    private void Update()
    {
        if(!IsOwner)return;
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlateTimerMax)
        {
            spawnPlateTimer = 0;
            if (GameManager.Instance.IsGameplaying()&&plateSpawanAmount < plateSpawanAmountMax)
            {
                SpawnPlateServerRpc();
            }
        }
    }

    [ServerRpc]
    private void SpawnPlateServerRpc()
    {
        SpawnPlateClientRpc();
    }
    
    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        plateSpawanAmount++; 
        OnPlateSpawaned?.Invoke(this, EventArgs.Empty);
    }
    

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            if (plateSpawanAmount > 0)
            {
                
                KitcheObject.SpawnKitchenObject(plateKitchenObjectSo, player);
                InteractLogicServerRpc();
            }
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
        plateSpawanAmount--;
        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }
}
