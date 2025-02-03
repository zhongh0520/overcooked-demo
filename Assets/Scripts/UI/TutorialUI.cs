using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    private void Start()
    {
        
        GameManager.Instance.OnLocalPlayerRreadyChange += GameManager_OnLocalPlayerRreadyChange;
        Show();
    }

    private void GameManager_OnLocalPlayerRreadyChange(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsLocalPlayerRready())
        {
            Hide();
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
