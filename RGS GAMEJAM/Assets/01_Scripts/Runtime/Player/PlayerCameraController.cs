using UnityEngine;
using Mirror;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
public class PlayerCameraController : NetworkBehaviour
{
    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float minZoom = 1f;
    public float maxZoom = 6f;

    [Header("Free Camera Movement")]
    public float cameraMoveSpeed = 10f;
    public int edgeThickness = 20;

    private CinemachineCamera vcam;
    private PlayerInputHandler inputHandler;

    private bool followMode = true;
    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
    }
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        vcam = FindFirstObjectByType<CinemachineCamera>();
        if (vcam != null)
        {
            vcam.Follow = transform;
        }
    }
    private void Update()
    {
        if (!isLocalPlayer) return;
        HandleZoom();
        HandleToggle();

        if (!followMode)
        {
            HandleFreeCameraMovement();
        }
    }
    private void HandleToggle()
    {
        if (inputHandler.toggleCameraPressed)
        {
            followMode = !followMode;

            if (followMode)
            {
                vcam.Follow = transform;
            }
            else
            {
                vcam.Follow = null;
            }
        }
    }
    private void HandleZoom()
    {
        if (vcam == null) return;

        float scroll = inputHandler.zoomInput;

        if (Mathf.Abs(scroll) > 0.01f)
        {
            float currentZoom = vcam.Lens.OrthographicSize;
            currentZoom -= scroll * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

            vcam.Lens.OrthographicSize = currentZoom;
        }
    }
    void HandleFreeCameraMovement()
    {
        Vector3 camPos = vcam.transform.position;
        Vector2 mouse = Mouse.current.position.ReadValue();
        float moveX = 0;
        float moveY = 0;

        if (mouse.x >= Screen.width - edgeThickness) moveX = 1;  
        else if (mouse.x <= edgeThickness) moveX = -1;            

        if (mouse.y >= Screen.height - edgeThickness) moveY = 1;  
        else if (mouse.y <= edgeThickness) moveY = -1; 

        Vector3 move = new Vector3(moveX, moveY, 0) * cameraMoveSpeed * Time.deltaTime;

        vcam.transform.position += move;
    }
}
