using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliverySingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI repiceNameText;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform icontemplate;

    private void Awake()
    {
        icontemplate.gameObject.SetActive(false);
    }

    public void SetRecipeSO(RecipeSO recipeSO)
    {
        repiceNameText.text = recipeSO.name;

        foreach (Transform chlid in iconContainer)
        {
            if(chlid==icontemplate)continue;
            Destroy(chlid.gameObject);
        }

        foreach (KitchenObjectSO kitchenObjectSo in recipeSO.kitchenObjectsSoList)
        {
            Transform transform =Instantiate(icontemplate, iconContainer);
            transform.gameObject.SetActive(true);
            transform.GetComponent<Image>().sprite = kitchenObjectSo.sprite;
        }
    }
}
