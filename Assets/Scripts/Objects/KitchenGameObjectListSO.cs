using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "KitchenGameObjectList")]
public class KitchenGameObjectListSO : ScriptableObject
{
   public List<KitchenObjectSO> kitchenObjectSOList;
}
