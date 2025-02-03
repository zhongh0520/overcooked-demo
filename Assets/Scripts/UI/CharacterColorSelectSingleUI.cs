using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterColorSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int colorID;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectObject;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener((() =>
        {
            //KichenGameMultiPlayer.Instance.ChangedPlayerColor(colorID);
        }));
    }

    private void Start()
    {
        image.color = KichenGameMultiPlayer.Instance.GetPlayerColor(colorID);

    }




}
