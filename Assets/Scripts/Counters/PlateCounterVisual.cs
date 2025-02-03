using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCounterVisual : MonoBehaviour
{
    
    [SerializeField] private Transform counterTop;
    [SerializeField] private Transform plateVisualPrefab;
    [SerializeField] private PlatesCounter platesCounter;
    private List<GameObject> plateVisualGameObjectsList;

    private void Awake()
    {
        plateVisualGameObjectsList = new List<GameObject>();
    }

    private void Start()
    {
        platesCounter.OnPlateSpawaned += PlatesCounter_OnPlateSpawned;
        platesCounter.OnPlateRemoved += PlatesCounter_OnPlateRemove;
        
    }

    private void PlatesCounter_OnPlateRemove(object sender, EventArgs e)
    {
        GameObject plateGameObject = plateVisualGameObjectsList[plateVisualGameObjectsList.Count - 1];
        plateVisualGameObjectsList.Remove(plateGameObject);
        Destroy(plateGameObject);
        
    }

    private void PlatesCounter_OnPlateSpawned(object sender, EventArgs e)
    {
       Transform plateVisualTransform =Instantiate(plateVisualPrefab, counterTop);
       float plateOffsetY = .1f;
       plateVisualTransform.localPosition = new Vector3(0, plateOffsetY * plateVisualGameObjectsList.Count, 0);
       plateVisualGameObjectsList.Add(plateVisualTransform.gameObject);
    }
}
