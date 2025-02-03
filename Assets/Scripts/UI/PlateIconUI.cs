using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconUI : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private Transform iconTemplate;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += plateKitchenObject_OnIngredientAdded;
    }

    private void plateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArg e)
    {
        UpdateViusal();
    }

    private void UpdateViusal()
    {
        foreach (Transform chlid in transform)
        {
            if(chlid==iconTemplate)continue;
         Destroy(chlid.gameObject);   
        }
        foreach (KitchenObjectSO kitchenObjectSo in plateKitchenObject.GetkitchenObjectsList())
        {
            Transform iconTransform =Instantiate(iconTemplate,transform);
            iconTransform.gameObject.SetActive(true); 
            iconTransform.GetComponent<PlateIconSingle>().SetKitchenObjectSO(kitchenObjectSo);
        }
    }
}
