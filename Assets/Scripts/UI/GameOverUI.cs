using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipesText;
    [SerializeField] private Button mainManeButton;

    private void Awake()
    {
        mainManeButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scence.MainMenuScence);
        });
    }

    

    private void Start()
    {
        Hide();
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            Show();
            recipesText.text = DeliveryManager.Instance.GetSuccessRecipeAmount().ToString();
        }
        else
        {
            Hide();
            
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    } 
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}

