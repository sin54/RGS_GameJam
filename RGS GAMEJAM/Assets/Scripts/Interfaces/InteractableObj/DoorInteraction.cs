using UnityEngine;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    [Header("UI Prompt")]
    [SerializeField] private Transform appearTransform; // UI가 뜰 위치 지정
    [SerializeField] private bool useAppearTransform = true;

    [SerializeField] private GameObject magicDoorPanel;
    public Transform AppearTransform => useAppearTransform && appearTransform != null ? appearTransform : transform;
    public bool isAppearTransform => useAppearTransform && appearTransform != null;

    public bool CanInteract()
    {
        return true;
    }

    public bool Interact(Interactor interactor)
    {
        return true;
    }

    public void OnEnterRange(Interactor interactor)
    {
        magicDoorPanel.SetActive(true);
    }

    public void OnExitRange(Interactor interactor)
    {
        magicDoorPanel.SetActive(false);
    }
}
