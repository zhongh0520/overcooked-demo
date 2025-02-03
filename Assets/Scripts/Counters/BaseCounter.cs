using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class BaseCounter : NetworkBehaviour,IkitchenObjectParents
{
   public static event EventHandler OnAnyObjectPlacedHere; 
   public static void ResetStaticData()
   {
      OnAnyObjectPlacedHere = null;
   }
   //[SerializeField]private KitchenObjectSO kitchenObjectSo; 
   [SerializeField]private Transform counterTopPoint;
   private KitcheObject kitcheObject;
   public virtual void Interact(Player player)
   {
      
   }
   public virtual void InteractAlernate(Player player)
   {
      
   }
   public Transform GetKitchenObjectFollowTransform()
   {
      return counterTopPoint;
   }

   public void SetKitchenObject(KitcheObject kitcheObject)
   {
      this.kitcheObject = kitcheObject;
      if (kitcheObject != null)
      {
         OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
      }
   }

   public KitcheObject GetKitcheObject()
   {
      return kitcheObject;
   }

   public void ClearKitchenObject()
   {
      kitcheObject=null;
   }

   public bool HasKitchenObject()
   {
      return kitcheObject != null;
   }

   public NetworkObject GetNetworkObject()
   {
      return NetworkObject;
   }
}
