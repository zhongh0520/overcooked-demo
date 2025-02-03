using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IkitchenObjectParents
{

    public Transform GetKitchenObjectFollowTransform();

    public void SetKitchenObject(KitcheObject kitcheObject);

    public KitcheObject GetKitcheObject();

    public void ClearKitchenObject();

    public bool HasKitchenObject();
    public NetworkObject GetNetworkObject();
}
