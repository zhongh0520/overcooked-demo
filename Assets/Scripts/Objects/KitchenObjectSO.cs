using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KitchenObjectSO", menuName = "KitchenObjectSO")]
public class KitchenObjectSO : ScriptableObject
{
    public Transform prefab;
    public Sprite sprite;
    public string objectName;
}
