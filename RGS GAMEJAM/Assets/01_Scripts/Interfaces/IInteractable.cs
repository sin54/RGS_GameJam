using Mirror;
using UnityEngine;

public enum InteractionType
{
    Tap, 
    Hold 
}
public interface IInteractable
{
    GameObject Obj { get; }
    public bool IsLocked { get; set; }
    NetworkIdentity LockedBy { get; set;}
    public bool CanInteract(Interactor interactor);
    public bool InteractClient(Interactor interactor);
    public bool InteractServer(Interactor interactor);

    public void OnEnterRange(Interactor interactor);
    public void OnExitRange(Interactor interactor);

    public Transform AppearTransform { get; }
    public bool isAppearTransform { get; }
    public bool isRoomInteractor { get; }
    public InteractionType GetInteractionType();
    public float GetHoldTime();
    public string GetPromptText();
}
