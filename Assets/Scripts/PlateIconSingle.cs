using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class PlateIconSingle : MonoBehaviour
{
    [SerializeField] private Image image;
    public void SetKitchenObjectSO(KitchenObjectSO KitchenObjectSO)
    {
        image.sprite = KitchenObjectSO.sprite; 
    }
    
}
