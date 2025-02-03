using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SelectCounter : MonoBehaviour
{
   [FormerlySerializedAs("clearCount")] [SerializeField]private BaseCounter baseCounter;
   [SerializeField] private GameObject[] visualGameObjectArray;
   private void Start()
   {
      if(Player.localInstance!=null)
      Player.localInstance.OnSelectedCounterChanged += Player_OnSelectedCounterchanged;
      else
      {
         Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
      }
   }

   private void Player_OnAnyPlayerSpawned(object sender, EventArgs e)
   {
      if (Player.localInstance != null)
      {
         Player.localInstance.OnSelectedCounterChanged -= Player_OnSelectedCounterchanged;
         Player.localInstance.OnSelectedCounterChanged += Player_OnSelectedCounterchanged;
      }
   }

   private void Player_OnSelectedCounterchanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
   {
      if (e.selectedCounter == baseCounter)
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
      foreach (GameObject visualGameObject in visualGameObjectArray)
      {
      visualGameObject.SetActive(true);    
      }
   }

   private void Hide()
   {
      foreach (GameObject visualGameObject in visualGameObjectArray)
      {
      visualGameObject.SetActive(false);
      }
   }
   
}
