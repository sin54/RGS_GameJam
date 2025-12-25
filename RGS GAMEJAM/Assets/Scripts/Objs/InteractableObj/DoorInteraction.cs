using Mirror;
using UnityEngine;

public class DoorInteraction : NetworkBehaviour, IInteractable
{
    [Header("UI Prompt")]
    [SerializeField] private Transform appearTransform; // UI가 뜰 위치 지정
    [SerializeField] private bool useAppearTransform = true;

    [SerializeField] private GameObject magicDoorPanel;

    public GameObject Obj => gameObject;
    [SyncVar] public bool isLocked;
    public bool IsLocked { get => isLocked; set => isLocked = value; }
    [SyncVar] private NetworkIdentity lockedBy;
    public NetworkIdentity LockedBy { get => lockedBy; set => lockedBy = value; }
    public Transform AppearTransform => useAppearTransform && appearTransform != null ? appearTransform : transform;
    public bool isAppearTransform => useAppearTransform && appearTransform != null;
    public bool isRoomInteractor => true;
    public InteractionType GetInteractionType() => InteractionType.Tap;
    public float GetHoldTime() => 0f;
    public string GetPromptText() => "문 열기";
    public bool CanInteract(Interactor interactor)
    {
        if (isLocked) return false;
        return true;
    }

    public bool InteractClient(Interactor interactor)
    {
        return true;
    }

    public void OnEnterRange(Interactor interactor)
    {
        if (magicDoorPanel != null) {
            magicDoorPanel.SetActive(true);
        }

    }

    public void OnExitRange(Interactor interactor)
    {
        if (magicDoorPanel != null)
        {
            magicDoorPanel.SetActive(false);
        }
    }
    [Server]
    public bool InteractServer(Interactor interactor)
    {
        return true;
    }
}
