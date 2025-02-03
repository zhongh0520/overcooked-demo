using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitcheObject
{
    public event EventHandler<OnIngredientAddedEventArg> OnIngredientAdded;
    public class  OnIngredientAddedEventArg:EventArgs
    {
        public KitchenObjectSO KitchenObjectSo;
    }
    [SerializeField] private List<KitchenObjectSO> validKitchenObjects;
    private List<KitchenObjectSO> kitchenObjectsList;

    protected virtual void Awake()
    {
        base.Awake();
        kitchenObjectsList = new List<KitchenObjectSO>();
    }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSo)
    {
        if (!validKitchenObjects.Contains(kitchenObjectSo))
        {
            
        return false;
        }
        if (kitchenObjectsList.Contains(kitchenObjectSo))
        {
            return false;
        }
        else
        {
            AddIngredientServerRpc(
                    KichenGameMultiPlayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSo));
            return true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int kitchenObjectIndex)
    {
        AddIngredientClientRpc(kitchenObjectIndex);
    }
    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectIndex)
    {
        KitchenObjectSO kitchenObjectSO =
            KichenGameMultiPlayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectIndex);
        kitchenObjectsList.Add(kitchenObjectSO);

        OnIngredientAdded?.Invoke(this,new OnIngredientAddedEventArg
        {
            KitchenObjectSo = kitchenObjectSO
        });
    }

    public List<KitchenObjectSO> GetkitchenObjectsList()
    {
        return kitchenObjectsList;
    }
}
