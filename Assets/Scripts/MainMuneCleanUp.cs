using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainMuneCleanUp : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
        if(KichenGameMultiPlayer.Instance!=null)
        {
            Destroy(KichenGameMultiPlayer.Instance.gameObject);
        }
    }
}
