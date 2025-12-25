using UnityEngine;
using Mirror;
using System.Collections;
public class Interactor : NetworkBehaviour
{
    private PlayerInputHandler inputHandler;

    [Header("Interact Test")]
    [SerializeField] private Transform interactionPos;
    [SerializeField] private Vector2 interactionRange;
    [SerializeField] private LayerMask interactionLayer;

    [Header("UI")]
    [SerializeField] private GameObject tapPromptPrefab;
    [SerializeField] private GameObject holdPromptPrefab;
    private GameObject promptInstance;
    private PlayerMovement PM;
    private PlayerHealth PH;

    private IInteractable currentInteractable;
    private IInteractable previousInteractable;

    private InteractionType currentType;
    private bool isHolding = false;
    private float holdTimer = 0f;
    private float requiredHoldTime = 0f;
    public bool isRoomInteract;

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        PM = GetComponent<PlayerMovement>();
        PH = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        UpdateInteractable();

        if (currentInteractable == null)
        {
            isHolding = false;
            holdTimer = 0f;
            return;
        }

        var type = currentInteractable.GetInteractionType();

        // TAP 贸府
        if (type == InteractionType.Tap)
        {
            if (inputHandler.interactionJustPressed)
            {
                if (isRoomInteract == currentInteractable.isRoomInteractor &&
                    currentInteractable.CanInteract(this))
                {
                    CmdTryInteract(((Component)currentInteractable).gameObject);
                }
            }
            return;
        }

        // HOLD 贸府
        requiredHoldTime = currentInteractable.GetHoldTime();

        if (!currentInteractable.CanInteract(this) && inputHandler.isInteractionPressed)
        {
            FailedHold(currentInteractable);
            return;
        }

        if (inputHandler.isInteractionPressed)
        {
            if (!isHolding)
            {
                isHolding = true;
                holdTimer = 0f;
                PM.SetVelocityZero();
                CmdRequestLock(((Component)currentInteractable).gameObject);
            }

            holdTimer += Time.deltaTime;

            UpdateHoldProgressUI(holdTimer / requiredHoldTime);
            if (PH.isDead)
            {
                FailedHold(currentInteractable);
            }

            if (holdTimer >= requiredHoldTime)
            {
                CmdTryInteract(((Component)currentInteractable).gameObject);
                isHolding = false;
                holdTimer = 0f;
                UpdateHoldProgressUI(0f);
            }
        }
        if (inputHandler.interactionJustReleased)
        {
            if (isHolding)
            {
                FailedHold(currentInteractable);
            }
        }
    }
    public bool GetHoldingState()
    {
        return isHolding;
    }
    private void UpdateHoldProgressUI(float progress)
    {
        if (promptInstance == null) return;
        if (currentType != InteractionType.Hold) return;

        var ui = promptInstance.GetComponent<InteractionPromptUI_Hold>();
        ui.SetFillAmount(Mathf.Clamp01(progress));
    }
    private void FailedHold(IInteractable II)
    {
        isHolding = false;
        holdTimer = 0f;
        CmdSetLocked(II.Obj, false);
        UpdateHoldProgressUI(0f);
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

        if (previousInteractable != currentInteractable)
        {
            if (previousInteractable != null)
            {
                previousInteractable.OnExitRange(this);
            }

            if (currentInteractable != null)
            {
                currentInteractable.OnEnterRange(this);
            }

            previousInteractable = currentInteractable;
        }

        if (currentInteractable != null)
        {
            if (!currentInteractable.CanInteract(this))
            {
                HidePrompt();
            }
            else
            {
                ShowPrompt(currentInteractable);
            }
        }
        else
        {
            HidePrompt();
        }

    }

    private void ShowPrompt(IInteractable target)
    {
        if (!target.CanInteract(this))
        { 
            HidePrompt(); 
            return; 
        }
        if (isRoomInteract != target.isRoomInteractor)
        {
            HidePrompt();
            return;
        }

        if (!target.isAppearTransform)
        {
            HidePrompt();
            return;
        }
        if (target.IsLocked && target.LockedBy != netIdentity)
        {
            HidePrompt();
            return;
        }
        if (promptInstance == null || currentType != target.GetInteractionType())
        {
            if (promptInstance != null)
                Destroy(promptInstance);

            currentType = target.GetInteractionType();

            if (currentType == InteractionType.Tap)
                promptInstance = Instantiate(tapPromptPrefab);
            else
                promptInstance = Instantiate(holdPromptPrefab);
        }

        promptInstance.SetActive(true);

        Transform appear = target.AppearTransform;
        promptInstance.transform.position = appear.position;

        string text = target.GetPromptText();

        if (currentType == InteractionType.Tap)
        {
            var ui = promptInstance.GetComponent<InteractPromptUI_NonHold>();
            ui?.SetText(text);
        }
        else
        {
            var ui = promptInstance.GetComponent<InteractionPromptUI_Hold>();
            ui?.SetText(text);
        }
    }


    private void HidePrompt()
    {
        if (promptInstance != null)
            promptInstance.SetActive(false);
    }

    private bool DoInteractionTest(out IInteractable interactable)
    {
        interactable = null;
        Collider2D[] hits = Physics2D.OverlapBoxAll(interactionPos.position, interactionRange, 0, interactionLayer);

        float closestDist = float.MaxValue;
        foreach (var col in hits)
        {
            float dist = Vector2.Distance(transform.position, col.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                interactable = col.GetComponent<IInteractable>();
            }
        }

        return interactable != null;
    }
    [Command]
    private void CmdTryInteract(GameObject target)
    {
        var interactable = target.GetComponent<IInteractable>();
        if (interactable == null) return;
        if (!interactable.CanInteract(this)) return;

        interactable.InteractServer(this);
        TargetRunClientInteract(connectionToClient, target);
        interactable.IsLocked = false;
    }
    [Command]
    private void CmdRequestLock(GameObject target)
    {
        var interactable = target.GetComponent<IInteractable>();

        if (interactable == null) return;
        if (interactable.IsLocked) return;
        interactable.IsLocked = true;
        interactable.LockedBy = connectionToClient.identity;
    }
    [Command]
    public void CmdSetLocked(GameObject target, bool value)
    {
        var interactable = target.GetComponent<IInteractable>();
        if (interactable == null) return;

        interactable.IsLocked = value;
    }
    [TargetRpc]
    private void TargetRunClientInteract(NetworkConnectionToClient conn, GameObject target)
    {
        var interactable = target.GetComponent<IInteractable>();
        if (interactable == null) return;

        interactable.InteractClient(this);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(interactionPos.position, interactionRange);
    }
}
