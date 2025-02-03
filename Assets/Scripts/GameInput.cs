using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance{get; private set;}
    private PlayerInputAction playerInputActions;
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlernateAction;
    public event EventHandler OnPauseAction;
    
    private void Awake()
    {
        Instance = this;
        playerInputActions= new PlayerInputAction();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_prefomed;
        playerInputActions.Player.Pause.performed += Pause_performed;
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Interact.performed -= Interact_performed;
        playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_prefomed;
        playerInputActions.Player.Pause.performed -= Pause_performed;
        playerInputActions.Dispose();
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }


    //事件反应
    private void InteractAlternate_prefomed(UnityEngine.InputSystem.InputAction.CallbackContext pbj)
        {
            
            OnInteractAlernateAction?.Invoke(this,EventArgs.Empty);
        }
    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext pbj)
    {
        
        OnInteractAction?.Invoke(this,EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
        return inputVector;
    }
}
