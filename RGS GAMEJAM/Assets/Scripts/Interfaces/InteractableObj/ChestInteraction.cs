using UnityEngine;
using System.Collections;
using Mirror;
public class ChestInteraction : NetworkBehaviour, IInteractable
{
    [Header("UI Prompt")]
    [SerializeField] private Transform appearTransform;
    [SerializeField] private bool useAppearTransform = true;

    [SerializeField] private float moveSpeed = 0.5f;

    public GameObject Obj => gameObject;
    [SyncVar] public bool isLocked;
    public bool IsLocked { get => isLocked; set => isLocked = value; }
    [SyncVar] private NetworkIdentity lockedBy;
    public NetworkIdentity LockedBy { get => lockedBy; set => lockedBy = value; }
    public Transform AppearTransform => useAppearTransform && appearTransform != null ? appearTransform : transform;
    public bool isAppearTransform => useAppearTransform && appearTransform != null;
    public bool isRoomInteractor => false;
    public InteractionType GetInteractionType() => InteractionType.Tap;
    public float GetHoldTime() => 0f;
    public string GetPromptText() => "상자 열기";

    private bool isInRange = false;
    private Interactor currentInteractor;
    private Coroutine transferRoutine;
    private Animator chestAnim;

    private void Awake()
    {
        chestAnim = GetComponentInChildren<Animator>();
    }
    private void Update()
    {
        if (isInRange)
        {

        }
    }
    public bool CanInteract(Interactor interactor)
    {
        if (IsLocked) return false;
        return true;
    }

    public bool InteractClient(Interactor interactor)
    {
        return true;
    }

    public void OnEnterRange(Interactor interactor)
    {
        isInRange = true;
        currentInteractor = interactor;

        if (interactor.isLocalPlayer)
        {
            transferRoutine = StartCoroutine(AutoTransferRoutine());
        }
    }

    public void OnExitRange(Interactor interactor)
    {
        isInRange = false;

        if (interactor.isLocalPlayer && transferRoutine != null)
        {
            StopCoroutine(transferRoutine);
            transferRoutine = null;
        }
        currentInteractor = null;
    }
    private IEnumerator AutoTransferRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveSpeed);

            if (!isInRange) break;

            if (CustomNetworkGamePlayer.localPlayer.isResourceMoved())
            {
                chestAnim.SetTrigger("ChestInteract");
                CustomNetworkGamePlayer.localPlayer.CmdMoveResourceToGlobal();
            }

            
        }
    }
    [Server]
    public bool InteractServer(Interactor interactor)
    {
        return true;
    }
}
