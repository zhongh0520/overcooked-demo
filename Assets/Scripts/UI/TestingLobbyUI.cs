using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingLobbyUI : MonoBehaviour
{
    [SerializeField] private Button createGameButton;
    [SerializeField] private Button joinGameButton;

    private void Awake()
    {
        createGameButton.onClick.AddListener(() =>
        {
            KichenGameMultiPlayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scence.CharacterSelectScene);
        });
        joinGameButton.onClick.AddListener(() =>
        {
            KichenGameMultiPlayer.Instance.StartClient();
        });
    }
}
