using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransfrom : MonoBehaviour
{
   private Transform targetTrans;

   public void SetTargetTransform(Transform targetTransform)
   {
      this.targetTrans = targetTransform;
   }

   private void LateUpdate()
   {
      if (targetTrans == null)
      {
         return;
      }
      transform.position = targetTrans.position;
      transform.rotation = targetTrans.rotation;
      
   }
}
