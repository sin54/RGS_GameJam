using UnityEngine;
using Mirror;
public class Interactor : NetworkBehaviour
{
    private PlayerInputHandler inputHandler;

    [Header("Interact Test")]
    [SerializeField] private Transform interactionPos;
    [SerializeField] private Vector2 interactionRange;
    [SerializeField] private LayerMask interactionLayer;

    [Header("UI")]
    [SerializeField] private GameObject interactPromptPrefab;
    private GameObject promptInstance;

    private IInteractable currentInteractable;
    private IInteractable previousInteractable;

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;   
        UpdateInteractable();

        if (inputHandler.interactionJustPressed && currentInteractable != null)
        {
            if (currentInteractable.CanInteract())
                currentInteractable.Interact(this);
        }
    }

    private void UpdateInteractable()
    {
        if (DoInteractionTest(out IInteractable nearest))
        {
            currentInteractable = nearest;
        }
        else
        {
            currentInteractable = null;
        }

        // 상태 변화 감지
        if (previousInteractable != currentInteractable)
        {
            // 범위에서 나감
            if (previousInteractable != null)
            {
                previousInteractable.OnExitRange(this);
            }

            // 범위 안으로 들어옴
            if (currentInteractable != null)
            {
                currentInteractable.OnEnterRange(this);
            }

            previousInteractable = currentInteractable;
        }

        // UI 처리
        if (currentInteractable != null)
            ShowPrompt(currentInteractable);
        else
            HidePrompt();
    }

    private void ShowPrompt(IInteractable target)
    {
        if (!target.isAppearTransform)
        {
            HidePrompt();
            return;
        }

        if (promptInstance == null)
            promptInstance = Instantiate(interactPromptPrefab);

        promptInstance.SetActive(true);

        Transform appear = target.AppearTransform;
        promptInstance.transform.position = appear.position;
    }


    private void HidePrompt()
    {
        if (promptInstance != null)
            promptInstance.SetActive(false);
    }

    private bool DoInteractionTest(out IInteractable interactable)
    {
        interactable = null;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(interactionPos.position, interactionRange, 0, Vector2.right, 0, interactionLayer);

        float closestDist = float.MaxValue;
        for (int i = 0; i < hits.Length; i++)
        {
            float dist = Vector2.Distance(transform.position, hits[i].transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                interactable = hits[i].collider.GetComponent<IInteractable>();
            }
        }

        return interactable != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(interactionPos.position, interactionRange);
    }
}
