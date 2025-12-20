using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 movementInput { get; private set; }
    public bool isInteractionPressed { get; private set; }
    public bool interactionJustPressed { get; private set; }  // 눌린 순간
    public bool interactionJustReleased { get; private set; } // 뗀 순간
    public float zoomInput { get; private set; }
    public bool toggleCameraPressed { get; private set; }
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnInteractionInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            interactionJustPressed = true;
            isInteractionPressed = true;
        }

        if (context.performed)
        {
            isInteractionPressed = true;
        }

        if (context.canceled)
        {
            interactionJustReleased = true;
            isInteractionPressed = false;
        }
    }
    public void OnZoomInput(InputAction.CallbackContext context)
    {
        Vector2 scroll = context.ReadValue<Vector2>();
        zoomInput = scroll.y;
    }
    public void OnToggleCamera(InputAction.CallbackContext context)
    {
        if (context.started)
            toggleCameraPressed = true;
    }
    private void LateUpdate()
    {
        interactionJustPressed = false;
        interactionJustReleased = false;
        toggleCameraPressed = false;
        zoomInput = 0f;
    }
}
