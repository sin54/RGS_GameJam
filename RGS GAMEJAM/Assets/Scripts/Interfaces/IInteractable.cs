using UnityEngine;

public interface IInteractable
{
    public bool CanInteract();
    public bool Interact(Interactor interactor);

    public void OnEnterRange(Interactor interactor);
    public void OnExitRange(Interactor interactor);

    public Transform AppearTransform { get; }
    public bool isAppearTransform { get; }
}
