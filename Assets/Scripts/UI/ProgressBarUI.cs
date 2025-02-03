using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    private IHasProgress hasProgress; 
    [SerializeField] private GameObject hasProgressObject;
    [SerializeField]private Image barImage;

    private void Start()
    {
        hasProgress=hasProgressObject.GetComponent<IHasProgress>();
        if (hasProgress == null)
        {
            
        }
        hasProgress.OnProgressChanged += HasProgress_OnprogressChanged;
        barImage.fillAmount = 0;
        Hide();
    }

    private void HasProgress_OnprogressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        barImage.fillAmount = e.progressNormalized;
        if (e.progressNormalized == 0 || e.progressNormalized == 1)
        {
            Hide();
        }
        else
        {
            Show(); 
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
