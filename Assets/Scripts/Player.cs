using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : NetworkBehaviour,IkitchenObjectParents
{
    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPickedSometing;
    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
    }
    public static Player localInstance{get; private set;}
    
    //其他物品箱子
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public event EventHandler OnPickSometing;
    public class OnSelectedCounterChangedEventArgs:EventArgs
    {
        public BaseCounter selectedCounter;
    }
    [SerializeField]
    private float moveSpeed = 7f;
    private bool isWalking;
    private Vector3 lastPInteractDir;
    private BaseCounter selectedCounter;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private LayerMask collisonLayerMask;
    [FormerlySerializedAs("counterTopPoint")] [SerializeField]
    private Transform kitcheObjectHoldPoint;

    [SerializeField] private List<Vector3> spawnPositionList;
    [SerializeField] private PlayerVisual playerVisual;
    private KitcheObject kitcheObject;

    private void Awake()
    {
        
        //Instance=this;
    }

    private void Start()
    {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAtion;
        GameInput.Instance.OnInteractAlernateAction += GameInput_OnInteractAlernateAction;
        PlayerData playerData=KichenGameMultiPlayer.Instance.GetPlayerDataFromClientID(OwnerClientId);
        playerVisual.SetPlayerColor(KichenGameMultiPlayer.Instance.GetPlayerColor(playerData.colorID));
        
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            localInstance = this;
        }

        transform.position = spawnPositionList[KichenGameMultiPlayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
        if(IsServer)
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if (clientId == OwnerClientId&& HasKitchenObject())
        {
            KitcheObject.DestoryKitchenObject(GetKitcheObject());
        }
    }

    private void GameInput_OnInteractAlernateAction(object sender, System.EventArgs e)
    {
        if(!GameManager.Instance.IsGameplaying())return;
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlernate(this);
        }
    }

    private void GameInput_OnInteractAtion(object sender, System.EventArgs e)
    {
        if(!GameManager.Instance.IsGameplaying())return;
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }
    private void Update()
    {
        if(!IsOwner)return;
        
        HandleMovement();
        HandleInteraction();
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteraction()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDir= new Vector3(inputVector.x, 0f, inputVector.y);
        if (moveDir != Vector3.zero)
        {
            lastPInteractDir=moveDir;
        }
        float interactionDistance = 2f;
        //射线检测前面物体
        if (Physics.Raycast(transform.position, lastPInteractDir, out RaycastHit raycastHit, interactionDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter( baseCounter);
                    
                }
            }
            else
            {
                SetSelectedCounter(null);
               

            }
            
        }
        else
        {
            SetSelectedCounter(null);
         

        }
        
    }

    
    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDir= new Vector3(inputVector.x, 0f, inputVector.y);
        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        //float playerHeight = 2f;
        bool canMove = !Physics.BoxCast(transform.position,Vector3.one*playerRadius, 
            moveDir,Quaternion.identity,moveDistance,collisonLayerMask);
        //对角线移动时 
        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove=moveDir.x!=0&&!Physics.BoxCast(transform.position,Vector3.one*playerRadius, 
                moveDirX,Quaternion.identity,moveDistance,collisonLayerMask);
            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0,0, moveDir.z).normalized;
                canMove=moveDir.z!=0&&!Physics.BoxCast(transform.position,Vector3.one*playerRadius, 
                    moveDirZ,Quaternion.identity,moveDistance,collisonLayerMask);
                if (canMove)
                {
                    moveDir= moveDirZ;
                }
                else
                {
                    
                }
            }
        }
        if(canMove)
            transform.position += moveDir * moveDistance ;
        //判断ANIMATOR
        isWalking = moveDir != Vector3.zero;
        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward , moveDir,Time.deltaTime*rotateSpeed);
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this,new OnSelectedCounterChangedEventArgs { selectedCounter = selectedCounter });

    }
    
    public Transform GetKitchenObjectFollowTransform()
    {
        return kitcheObjectHoldPoint;
    }

    public void SetKitchenObject(KitcheObject kitcheObject)
    {
        this.kitcheObject = kitcheObject;
        if (kitcheObject != null)
        {
            OnPickSometing?.Invoke(this, EventArgs.Empty);
            OnAnyPickedSometing?.Invoke(this,EventArgs.Empty);
        }
    }

    public KitcheObject GetKitcheObject()
    {
        return kitcheObject;
    }

    public void ClearKitchenObject()
    {
        kitcheObject=null;
    }

    public bool HasKitchenObject()
    {
        return kitcheObject != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
