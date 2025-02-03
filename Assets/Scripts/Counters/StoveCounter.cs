using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter,IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStageChangedEventArgs> OnStageChanged;
    public class  OnStageChangedEventArgs:EventArgs
    {
        public State state;
    }
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }
    [SerializeField] private FringRecipeSO[] fringRecipeSoArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSoArray;

    private NetworkVariable<float> fryingTimer= new NetworkVariable<float>(0f);
    private NetworkVariable<float> burningTimer= new NetworkVariable<float>(0f);
    private NetworkVariable<State> state=new NetworkVariable<State>(State.Idle);
    private FringRecipeSO fringRecipeSo;
    private BurningRecipeSO burningRecipeSo;

    
    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTime_OnValueChanged;
        burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        state.OnValueChanged += State_OnValueChanged;
        
    }

    private void State_OnValueChanged(State previousvalue, State newvalue)
    {
        OnStageChanged?.Invoke(this, new OnStageChangedEventArgs
        {
            state = state.Value
        });
        if (state.Value == State.Idle || state.Value == State.Burned)
        {
            OnProgressChanged?.Invoke(this ,new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = 0
            });
        }
    }

    private void BurningTimer_OnValueChanged(float previousvalue, float newvalue)
    {
        float burningTimeMax = burningRecipeSo != null ? burningRecipeSo.burningTimer : 1f;
        
        OnProgressChanged?.Invoke(this ,new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = burningTimer.Value/burningTimeMax
        });
    }

    private void FryingTime_OnValueChanged(float previousvalue, float newvalue)
    {
        float fryingTimeMax = fringRecipeSo != null ? fringRecipeSo.fryingTimeMax : 1f;
        
        OnProgressChanged?.Invoke(this ,new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = fryingTimer.Value/fryingTimeMax
        });
    }
    
    private void Update()
    {
        if(!IsServer)return;
        switch (state.Value)
        {
            case State.Idle:
                break;
            case State.Frying:
                fryingTimer.Value += Time.deltaTime;
                
                if (fryingTimer.Value > fringRecipeSo.fryingTimeMax)
                {
                    KitcheObject.DestoryKitchenObject(GetKitcheObject());
                    
                    KitcheObject.SpawnKitchenObject(fringRecipeSo.output, this);
                    state.Value = State.Fried;
                    burningTimer.Value = 0;
                    SetBuringRecipeSOClientRpc(KichenGameMultiPlayer.Instance.GetKitchenObjectSOIndex(GetKitcheObject().GetKitchenObjectSo()));
                    
                    
                   
                }
                break;
            case State.Fried:
                burningTimer.Value += Time.deltaTime;
                
                if (burningTimer.Value > burningRecipeSo.burningTimer)
                {
                    KitcheObject.DestoryKitchenObject(GetKitcheObject());
                    KitcheObject.SpawnKitchenObject(burningRecipeSo.output, this);
                    state.Value = State.Burned;
                    
                }
                break;
            case State.Burned:
                break;
        }
        
    }

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
                    InteractingLogicPlaceObjectOnCounterServerRpc(
                        KichenGameMultiPlayer.Instance.GetKitchenObjectSOIndex(kitcheObject.GetKitchenObjectSo()));
                   
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
                        KitcheObject.DestoryKitchenObject(GetKitcheObject());
                        SetStateIdleServerRpc();
                        
                       
                    }
                }
            }
            else
            {
                GetKitcheObject().SetKitchenObjectParents(player);
                SetStateIdleServerRpc();
                
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc()
    {
        state.Value = State.Idle;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void InteractingLogicPlaceObjectOnCounterServerRpc(int kitchenObjectIndex)
    {
        fryingTimer.Value = 0;
        state.Value = State.Frying;
        SetFryingRecipeSOClientRpc(kitchenObjectIndex);
    }
    [ClientRpc]
    private void SetFryingRecipeSOClientRpc(int kitchenObjectIndex)
    {
        KitchenObjectSO kitchenObjectSo=KichenGameMultiPlayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectIndex);
        fringRecipeSo = GetFryingRecipeSoWithInput(kitchenObjectSo);
        
        
    }
    [ClientRpc]
    private void SetBuringRecipeSOClientRpc(int kitchenObjectIndex)
    {
        KitchenObjectSO kitchenObjectSo=KichenGameMultiPlayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectIndex);
        burningRecipeSo = GetBurningRecipeSoWithInput(kitchenObjectSo);
        
        
    }
    //是否可以切
    private bool HasRecipeWithInput(KitchenObjectSO kitchenObjectSO)
    {
        FringRecipeSO fringRecipeSO = GetFryingRecipeSoWithInput(kitchenObjectSO);
      
        return fringRecipeSO != null;
    }
    //输出当前
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FringRecipeSO fryingRecipeSO = GetFryingRecipeSoWithInput(inputKitchenObjectSO);
        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.output;
        }
        else
        {
            return null;
        }
      
    }
    //获取当前
    private FringRecipeSO GetFryingRecipeSoWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FringRecipeSO fringRecipeSO in fringRecipeSoArray)
        {
            if (fringRecipeSO.input == inputKitchenObjectSO)
            {
                return fringRecipeSO;
            }
        }

        return null;
    }
    private BurningRecipeSO GetBurningRecipeSoWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningRecipeSO burningRecipeSo in burningRecipeSoArray)
        {
            if (burningRecipeSo.input == inputKitchenObjectSO)
            {
                return burningRecipeSo;
            }
        }

        return null;
    }

    public bool IsFried()
    {
        return state.Value == State.Fried;
    }
}
