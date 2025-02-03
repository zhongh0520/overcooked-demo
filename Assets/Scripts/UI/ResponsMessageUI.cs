using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ResponsMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        KichenGameMultiPlayer.Instance.OnFailedToJoinGame += KichenGameMultiPlayer_OnFailedToJoinGame;
        Hide();
    }

    private void KichenGameMultiPlayer_OnFailedToJoinGame(object sender, EventArgs e)
    {
        Show();
        messageText.text = NetworkManager.Singleton.DisconnectReason;
        if (messageText.text=="")
        {
            messageText.text = "cannot";
        }
    }

    private void Show()
    {
        this.gameObject.SetActive(true);
    }

    private void Hide()
    {
        this.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        KichenGameMultiPlayer.Instance.OnFailedToJoinGame -= KichenGameMultiPlayer_OnFailedToJoinGame;
    }
}
