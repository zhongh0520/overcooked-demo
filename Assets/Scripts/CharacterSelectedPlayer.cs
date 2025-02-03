using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectedPlayer : MonoBehaviour
{
    
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private PlayerVisual playerVisual;
    private void Start() {
        KichenGameMultiPlayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;
        UpdatePlayer();
    }

    private void CharacterSelectReady_OnReadyChanged(object sender, System.EventArgs e) {
        UpdatePlayer();
    }

    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e) {
        UpdatePlayer();
    }

    private void UpdatePlayer() {
        if (KichenGameMultiPlayer.Instance.IsPlayerIndexConnected(playerIndex)) {
            Show();

            PlayerData playerData = KichenGameMultiPlayer.Instance.GetPlayerDataFromIndex(playerIndex);
            readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientID));
            playerVisual.SetPlayerColor(KichenGameMultiPlayer.Instance.GetPlayerColor(playerData.colorID));
            Debug.Log(playerData.clientID);
        } else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        KichenGameMultiPlayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
    }
}
