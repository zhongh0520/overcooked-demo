using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct kitchenObjectSO_GameObject
    {
        public KitchenObjectSO KitchenObjectSo;
        public GameObject gameObject;
    }
    [SerializeField]private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<kitchenObjectSO_GameObject> kitchenObjectSoGameObjectsList;
    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += plateKitchenObject_OnIngredientAdded;
        foreach (kitchenObjectSO_GameObject kitchenObjectSoGameObject in kitchenObjectSoGameObjectsList)
        {
            
                kitchenObjectSoGameObject.gameObject.SetActive(false);
            
        }
        
    }

    private void plateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArg e)
    {
        foreach (kitchenObjectSO_GameObject kitchenObjectSoGameObject in kitchenObjectSoGameObjectsList)
        {
            if (kitchenObjectSoGameObject.KitchenObjectSo == e.KitchenObjectSo)
            {
                kitchenObjectSoGameObject.gameObject.SetActive(true);
            }
        }
        // e.KitchenObjectSo;
    }
}
