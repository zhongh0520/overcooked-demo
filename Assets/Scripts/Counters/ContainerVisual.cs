using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ContainerVisual : MonoBehaviour
{
   private const string OPEN_CLOSE = "OpenClose";
   [SerializeField]private ContainerCounter containerCounter;
   
   private Animator animator;

   private void Awake()
   {
      animator = GetComponent<Animator>();
   }

   private void Start()
   {
      containerCounter.OnPlayerGrabbedObject += ContainerVisual_OnPlayerGrabbedObject;
   }

   private void ContainerVisual_OnPlayerGrabbedObject(object sender, System.EventArgs e)
   {
      animator.SetTrigger(OPEN_CLOSE );
   }
}
