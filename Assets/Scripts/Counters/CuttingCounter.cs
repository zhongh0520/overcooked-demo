using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class CuttingCounter : BaseCounter,IHasProgress
{
   public static event EventHandler OnAnyCut;

   new public static void ResetStaticData()
   {
      OnAnyCut = null;
   }
   
   public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
   
   public event EventHandler OnCut;
   
   [SerializeField] private CuttingRecipeSO[] cuttingRecipeSoArray;
   private int cuttingProgress;
   public override void Interact(Player player)
   {
      if (!HasKitchenObject())
      {
         if (player.HasKitchenObject())
         {
            if (HasRecipeWithInput(player.GetKitcheObject().GetKitchenObjectSo()))
            {
               KitcheObject kitcheObject = player.GetKitcheObject();
               kitcheObject.SetKitchenObjectParents(this);
               InteractingLogicPlaceObjectOnCounterServerRpc();

            }
            

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
                  GetKitcheObject().DestroySelf();
                       
               }
            }
         }
         else
         {
            GetKitcheObject().SetKitchenObjectParents(player);
         }
      }
   }

   [ServerRpc(RequireOwnership = false)]
   private void InteractingLogicPlaceObjectOnCounterServerRpc()
   {
      InteractingLogicPlaceObjectOnCounterClientRpc();
   }
   
   [ClientRpc]
   private void InteractingLogicPlaceObjectOnCounterClientRpc()
   {
      cuttingProgress = 0;
     
      //改变UI的Image
      OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
      {
         progressNormalized =0f
      });
   }
   
   public override void InteractAlernate(Player player)
   {
      //不可以二次切割
      if (HasKitchenObject()&& HasRecipeWithInput(GetKitcheObject().GetKitchenObjectSo()))
      {

         CutObjectServerRpc();
         TestCuttingProgressServerRpc();
      }
   }

   [ServerRpc(RequireOwnership = false)]
   private void CutObjectServerRpc()
   {
      CutObjectClientRpc();
   }

   [ClientRpc]
   private void CutObjectClientRpc()
   {
      //获得
      cuttingProgress++;
      //是否在切割
      OnCut?.Invoke(this, EventArgs.Empty);
      OnAnyCut?.Invoke(this, EventArgs.Empty); 
      //查询是否正确以及ui的现实
      CuttingRecipeSO cuttingRecipeSo = GetCuttingRecipeSoWithInput(GetKitcheObject().GetKitchenObjectSo());
      OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
      {
         progressNormalized = (float)cuttingProgress/cuttingRecipeSo.cuttingProgressMax
      });
      

   }

   [ServerRpc(RequireOwnership = false)]
   private void TestCuttingProgressServerRpc()
   {
      CuttingRecipeSO cuttingRecipeSo = GetCuttingRecipeSoWithInput(GetKitcheObject().GetKitchenObjectSo());
      if (cuttingProgress >= cuttingRecipeSo.cuttingProgressMax)
      {
         KitchenObjectSO outputKitchenObjectSo = GetOutputForInput(GetKitcheObject().GetKitchenObjectSo());
         KitcheObject.DestoryKitchenObject(GetKitcheObject());
         
         
         KitcheObject.SpawnKitchenObject(outputKitchenObjectSo,this);
      }
   }
   
   //是否可以切
   private bool HasRecipeWithInput(KitchenObjectSO kitchenObjectSO)
   {
      CuttingRecipeSO cuttingRecipeSo = GetCuttingRecipeSoWithInput(kitchenObjectSO);
      
      return cuttingRecipeSo != null;
   }
   //输出当前
   private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
   {
      CuttingRecipeSO cuttingRecipeSo = GetCuttingRecipeSoWithInput(inputKitchenObjectSO);
      if (cuttingRecipeSo != null)
      {
         return cuttingRecipeSo.output;
      }
      else
      {
         return null;
      }
      
   }
   //获取当前
   private CuttingRecipeSO GetCuttingRecipeSoWithInput(KitchenObjectSO inputKitchenObjectSO)
   {
      foreach (CuttingRecipeSO cuttingRecipeSo in cuttingRecipeSoArray)
      {
         if (cuttingRecipeSo.input == inputKitchenObjectSO)
         {
            return cuttingRecipeSo;
         }
      }

      return null;
   }
}
