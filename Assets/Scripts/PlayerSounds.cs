using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    
    private Player player;
    private float footstepTimer;
    private float footstepTimerMax=1f;
    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0)
        {
            footstepTimer = footstepTimerMax;
            if (player.IsWalking())
            {
            float volume = 1;
            SoundManager.Instance.PlayFootSound(player.transform.position,volume);
                
            }
        }
    }
}
