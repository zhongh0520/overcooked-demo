using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField]private StoveCounter stoveCounter;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        stoveCounter.OnStageChanged += stoveCounter_OnStageChanged;
    }

    private void stoveCounter_OnStageChanged(object sender, StoveCounter.OnStageChangedEventArgs e)
    {
        bool playSound = e.state == StoveCounter.State.Frying || e.state==StoveCounter.State.Fried;
        if (playSound)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Pause();
            ;
        }
    }
}
