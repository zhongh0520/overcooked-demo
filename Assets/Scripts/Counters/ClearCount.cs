using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCount : BaseCounter 
{
    
    
   [SerializeField]private KitchenObjectSO kitchenObjectSo;
   
   //生成和给玩家并且检测柜子上面是否有东西
   public override void Interact(Player player)
   {
       if (!HasKitchenObject())
       {
           if (player.HasKitchenObject())
           {
               player.GetKitcheObject().SetKitchenObjectParents(this);
               
           }
           else
           {
               
           }
       }
       else
       {
           if (player.HasKitchenObject())
           {
               if (player.GetKitcheObject().TrtGetPlate(out PlateKitchenObject plateKitchenObject))
               {
                  
                   if (plateKitchenObject.TryAddIngredient(GetKitcheObject().GetKitchenObjectSo()))
                   {
                       KitcheObject.DestoryKitchenObject(GetKitcheObject());
                   ;
                       
                   }
               }
               else
               {
                   if (GetKitcheObject().TrtGetPlate(out  plateKitchenObject))
                   {
                       if (plateKitchenObject.TryAddIngredient(player.GetKitcheObject().GetKitchenObjectSo()))
                       {
                           KitcheObject.DestoryKitchenObject(player.GetKitcheObject());
                           
                       }
                   }
               }
           }
           else
           {
                GetKitcheObject().SetKitchenObjectParents(player);
           }
       }
     
   }


}
