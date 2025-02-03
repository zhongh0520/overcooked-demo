using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveWarningUI : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;

    private void Start()
    {
        stoveCounter.OnProgressChanged += stoveCounter_OnProgressChanged;
        Hide();
    }

    private void stoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        float burnShow = .5f;
         bool show = stoveCounter.IsFried() && e.progressNormalized >= burnShow;
         if (show)
         {
             Show();
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
