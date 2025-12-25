using UnityEngine;
using Mirror;
public class PlayerReviveInteraction : NetworkBehaviour, IInteractable
{
    [Header("UI Prompt")]
    [SerializeField] private Transform appearTransform;
    [SerializeField] private bool useAppearTransform = true;
    public GameObject Obj => gameObject;
    [SyncVar] public bool isLocked;
    public bool IsLocked { get => isLocked; set => isLocked = value; }
    [SyncVar] private NetworkIdentity lockedBy;
    public NetworkIdentity LockedBy { get => lockedBy; set => lockedBy = value; }
    public Transform AppearTransform => useAppearTransform && appearTransform != null ? appearTransform : transform;
    public bool isAppearTransform => useAppearTransform && appearTransform != null;
    public bool isRoomInteractor => false;
    public InteractionType GetInteractionType() => InteractionType.Hold;
    public float GetHoldTime() => playerHealth.GetPlayerReviveTime();
    public string GetPromptText() => "플레이어 부활";

    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    public bool CanInteract(Interactor interactor)
    {
        if (IsLocked && lockedBy != interactor.netIdentity) return false;
        if (!playerHealth.isDead) return false;
        if (interactor.netIdentity == this.netIdentity) return false;
        return true;
    }

    public bool InteractClient(Interactor interactor)
    {
        return true;
    }

    public bool InteractServer(Interactor interactor)
    {
        playerHealth.Revive();
        playerHealth.ServerSetHealth(1);
        return true;
    }

    public void OnEnterRange(Interactor interactor)
    {

    }

    public void OnExitRange(Interactor interactor)
    {

    }
}
