using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;
    public static DeliveryManager Instance{get; private set;}
    [SerializeField] private RecipeSOList receipeSOList;
    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer=4;
    private float spawnRecipeTimermax = 4;
    private int watingRecipeMax = 4;

    private int successRecipeAmount;
    private void Awake()
    {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
       
        
    }

    private void Update()
    {
        if(!IsServer)return;
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0)
        {
            spawnRecipeTimer = spawnRecipeTimermax;
            
            if (GameManager.Instance.IsGameplaying()&&waitingRecipeSOList.Count < watingRecipeMax)
            {
                int waitingRecipeSOIdenx = Random.Range(0, receipeSOList.recipeSOList.Count);
                SpawnRecipeWaitingRecipeClientRpc(waitingRecipeSOIdenx);
            
                
            }
        }
        
    }

    [ClientRpc]
    private void SpawnRecipeWaitingRecipeClientRpc(int waitingRecipeSOIndex)
    {
        RecipeSO waitingRecipeSO=receipeSOList.recipeSOList[waitingRecipeSOIndex];
        waitingRecipeSOList.Add(waitingRecipeSO);
            
        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    //交货    
    public void DeliveryRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];
            if (waitingRecipeSO.kitchenObjectsSoList.Count == plateKitchenObject.GetkitchenObjectsList().Count)
            {
                bool plateRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSo in waitingRecipeSO.kitchenObjectsSoList)
                {
                    bool inFound=false;
                    foreach (KitchenObjectSO plateKitchenObjectSo in plateKitchenObject.GetkitchenObjectsList())
                    {
                        if (plateKitchenObjectSo == recipeKitchenObjectSo)
                        {
                            inFound = true;
                            break;
                        }
                    }

                    if (!inFound)
                    {
                        plateRecipe = false;
                    }
                }
                //交的时正确的
                if (plateRecipe)
                {
                    DeliverCorrectRecipeServerRpc(i);
                    return;
                }
            }
        }
        //错误
        DeliverIncorrectRecipeServerRpc();
    }

    //交付错误
    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc()
    {
        DeliverIncorrectRecipeClientRpc();
    }
    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOIdenx)
    {
        DeliverCorrectRecipeClientRpc(waitingRecipeSOIdenx);
    }
    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOIdenx)
    {
        successRecipeAmount++;
        waitingRecipeSOList.RemoveAt(waitingRecipeSOIdenx);
        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);

    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetSuccessRecipeAmount()
    {
        return successRecipeAmount;
    }
}
