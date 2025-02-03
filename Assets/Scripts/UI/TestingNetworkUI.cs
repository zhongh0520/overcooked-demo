using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetworkUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;

    private void Awake()
    {
        startHostButton.onClick.AddListener(() =>
        {
            KichenGameMultiPlayer.Instance.StartHost();
            Hide();
        });
        
        startClientButton.onClick.AddListener((() =>
        {
           KichenGameMultiPlayer.Instance.StartClient();
            Hide();
        }));
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
