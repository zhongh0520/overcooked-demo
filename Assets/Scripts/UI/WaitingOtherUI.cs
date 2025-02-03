using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingOtherUI : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.OnLocalPlayerRreadyChange += GameManager_OnLocalPlayerRreadyChange;
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        Hide();
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsCountingDownToStartActive())
        {
            Hide();
        }
    }

    private void GameManager_OnLocalPlayerRreadyChange(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsLocalPlayerRready())
        {
            Show();
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
}
