using UnityEngine;
using Mirror;
using Unity.Services.Lobbies.Models;
public class ComposterInteraction : NetworkBehaviour, IInteractable
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
    public float GetHoldTime() => 0.1f;
    public string GetPromptText() => "Åðºñ ¸¸µé±â";

    [SerializeField] private int xpAmount;

    public bool CanInteract(Interactor interactor)
    {
        return true;
    }

    public bool InteractClient(Interactor interactor)
    {
        return true;
    }

    public bool InteractServer(Interactor interactor)
    {
        CustomNetworkGamePlayer.localPlayer.CmdRemoveResource();
        GameManager.Instance.mainTree.AddXp(xpAmount);
        return true;
    }

    public void OnEnterRange(Interactor interactor)
    {
    }

    public void OnExitRange(Interactor interactor)
    {
    }
}
