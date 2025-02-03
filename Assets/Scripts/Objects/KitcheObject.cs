using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class KitcheObject : NetworkBehaviour 
{
    [SerializeField]private KitchenObjectSO kitchenObjectSo;
    private IkitchenObjectParents kitchenObjectParents;
    private FollowTransfrom followTransfrom;

    protected virtual void Awake()
    {
        followTransfrom = GetComponent<FollowTransfrom>();
    }


    public KitchenObjectSO GetKitchenObjectSo()
    {
        return kitchenObjectSo;
    }

    public void SetKitchenObjectParents(IkitchenObjectParents kitchenObjectParents)
    {
        
        SetKitchenObjectParentServerRpc(kitchenObjectParents.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectReferenceNetworkObjectReference)
    {
        SetKitchenObjectParentClientRpc(kitchenObjectReferenceNetworkObjectReference);
    }

    [ClientRpc]
    private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectReferenceNetworkObjectReference)
    {
        kitchenObjectReferenceNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentsNetworkObject);
        IkitchenObjectParents kitchenObjectParents =
            kitchenObjectParentsNetworkObject.GetComponent<IkitchenObjectParents>();
        //改变父级
        if(this.kitchenObjectParents!=null)
        {
            this.kitchenObjectParents.ClearKitchenObject();
        }
        this.kitchenObjectParents = kitchenObjectParents;
        if (kitchenObjectParents.HasKitchenObject()) {
            Debug.LogError("IKitchenObjectParent already has a KitchenObject!");
        }
        kitchenObjectParents.SetKitchenObject(this);
        
        followTransfrom.SetTargetTransform(kitchenObjectParents.GetKitchenObjectFollowTransform());

    }

    public IkitchenObjectParents GetKitchenObjectParents()
    {
        return kitchenObjectParents;
    }

    public void DestroySelf()
    {
        
        Destroy(gameObject);
    }

    public void ClearKitchenObjectOnParents()
    {
        kitchenObjectParents.ClearKitchenObject();
    }

    public bool TrtGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        else
        {
            plateKitchenObject = null;
            return false;
        }
    }

    
    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO,IkitchenObjectParents kitchenObjectParents)
    {
        KichenGameMultiPlayer.Instance.SpawnKitchenObject(kitchenObjectSO, kitchenObjectParents);
        
    }

    public static void DestoryKitchenObject(KitcheObject kitcheObject)
    {
        KichenGameMultiPlayer.Instance.DestroyKitchenObjects(kitcheObject);
    }
    
}
