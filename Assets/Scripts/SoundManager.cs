using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance{get; private set;}
    
    [SerializeField] private AduioClipSO aduioClipSO;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        Player.OnAnyPickedSometing += Player_OnPickSometing;
        BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
    }

    private void TrashCounter_OnAnyObjectTrashed(object sender, EventArgs e)
    {
        TrashCounter trashCounter =sender as TrashCounter;
        PlaySound(aduioClipSO.trash, trashCounter.transform.position);
    }

    private void BaseCounter_OnAnyObjectPlacedHere(object sender, EventArgs e)
    {
        BaseCounter baseCounter = sender as BaseCounter;
        PlaySound(aduioClipSO.objectDrop, baseCounter.transform.position);
    }

    private void Player_OnPickSometing(object sender, EventArgs e)
    {
        Player player= sender as Player;
        PlaySound(aduioClipSO.objectPickup,player.transform.position);
    }

    private void CuttingCounter_OnAnyCut(object sender, EventArgs e)
    {
        CuttingCounter cuttingCountercounter = sender as CuttingCounter;
        PlaySound(aduioClipSO.chop,cuttingCountercounter.transform.position);
    }

    private void DeliveryManager_OnRecipeFailed(object sender, EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(aduioClipSO.deliveryFail,deliveryCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(aduioClipSO.deliverySuccess,deliveryCounter.transform.position);
    }
    private void PlaySound(AudioClip[] audioClipArray,Vector3 position,float volume=1)
    {
        
        PlaySound(audioClipArray[Random.Range(0,audioClipArray.Length)],position,volume);
    }

    private void PlaySound(AudioClip audioClipclip,Vector3 position,float volume=1)
    {
        
        AudioSource.PlayClipAtPoint(audioClipclip, position,1); 
    }

    public void PlayFootSound(Vector3 position,float volume=1)
    {
        PlaySound(aduioClipSO.footstep,position,volume);
    }
}
