using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start()
    {
        KichenGameMultiPlayer.Instance.OnTryingToJoinGame += KichenGameMultiPlayer_OnTryingToJoinGame;
        KichenGameMultiPlayer.Instance.OnFailedToJoinGame += KichenGameMultiPlayer_OnFailedToJoinGame;
        Hide();
    }

    private void KichenGameMultiPlayer_OnFailedToJoinGame(object sender, EventArgs e)
    {
        Hide();
    }

    private void KichenGameMultiPlayer_OnTryingToJoinGame(object sender, EventArgs e)
    {
        Show();
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
        KichenGameMultiPlayer.Instance.OnTryingToJoinGame -= KichenGameMultiPlayer_OnTryingToJoinGame;
        KichenGameMultiPlayer.Instance.OnFailedToJoinGame -= KichenGameMultiPlayer_OnFailedToJoinGame;
    }
}
