using UnityEngine;
using UnityEngine.InputSystem; // 새 인풋 시스템
using Mirror;

public class PlayerPing : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler inputHandler; // 같은 오브젝트에 있으면 자동으로 GetComponent
    [SerializeField] private Camera playerCamera;             // 비우면 자동으로 Camera.main
    [SerializeField] private GameObject pingPrefab;           // 다이아몬드 프리팹 (NetworkIdentity 필수)
    [SerializeField] private PingWheelUI pingWheelUI;         // 원형 UI

    [Header("Settings")]
    [SerializeField] private float minDragDistance = 30f;     // 몇 픽셀 이상 드래그해야 핑 확정

    private bool isPinging;
    private Vector2 startScreenPos;
    private Vector3 worldPingPos;
    private PingType currentType;

    private void Awake()
    {
        if (!inputHandler)
            inputHandler = GetComponent<PlayerInputHandler>();

        if (pingWheelUI == null)
        {
            // 비활성화된 오브젝트까지 포함해서 PingWheelUI 찾기
            pingWheelUI = Object.FindFirstObjectByType<PingWheelUI>(
                FindObjectsInactive.Include
            );
        }
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        if (playerCamera == null)
            playerCamera = Camera.main;

        HandlePingInput();
    }

    private void HandlePingInput()
    {
        // 디바이스 직접 읽기 (새 인풋 시스템 방식)
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if (keyboard == null || mouse == null) return;

        bool ctrlHeld =
            (keyboard.leftCtrlKey != null && keyboard.leftCtrlKey.isPressed) ||
            (keyboard.rightCtrlKey != null && keyboard.rightCtrlKey.isPressed);

        // Ctrl 안 누르면 진행 중이던 핑도 취소
        if (!ctrlHeld)
        {
            if (isPinging)
                CancelPing();
            return;
        }

        // 1) Ctrl + 우클릭 눌렀을 때 시작
        if (mouse.rightButton.wasPressedThisFrame)
        {
            isPinging = true;
            startScreenPos = mouse.position.ReadValue();
            worldPingPos = ScreenToWorld(startScreenPos);
            currentType = PingType.DefendMf; // 기본값

            if (pingWheelUI != null)
                pingWheelUI.Open(startScreenPos);
        }

        if (!isPinging) return;

        // 2) 드래그 방향에 따라 타입 선택
        Vector2 currentPos = mouse.position.ReadValue();
        Vector2 dir = currentPos - startScreenPos;

        if (dir.magnitude >= minDragDistance)
        {
            currentType = GetPingTypeFromDirection(dir);
            if (pingWheelUI != null)
                pingWheelUI.SetSelection(currentType);
        }
        else
        {
            if (pingWheelUI != null)
                pingWheelUI.ClearSelection();
        }

        // 3) 우클릭을 뗐을 때 확정 or 취소
        if (mouse.rightButton.wasReleasedThisFrame)
        {
            if (dir.magnitude >= minDragDistance)
            {
                // 서버에 핑 요청
                CmdSpawnPing(worldPingPos, currentType);
            }

            CancelPing();
        }
    }

    private void CancelPing()
    {
        isPinging = false;
        if (pingWheelUI != null)
            pingWheelUI.Close();
    }

    private Vector3 ScreenToWorld(Vector2 screenPos)
    {
        if (playerCamera == null) return Vector3.zero;

        Vector3 sp = new Vector3(screenPos.x, screenPos.y, -playerCamera.transform.position.z);
        Vector3 world = playerCamera.ScreenToWorldPoint(sp);
        world.z = 0f; // 2D면 0으로 고정
        return world;
    }

    private PingType GetPingTypeFromDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.0001f)
            return PingType.DefendMf; // 기본값(위)

        Vector2 n = dir.normalized;

        float up = Vector2.Dot(n, Vector2.up);    // (0, 1)
        float right = Vector2.Dot(n, Vector2.right); // (1, 0)
        float down = Vector2.Dot(n, Vector2.down);  // (0,-1)
        float left = Vector2.Dot(n, Vector2.left);  // (-1,0)

        float max = Mathf.Max(up, right, down, left);

        // PingType 매핑: 위=DefendMf, 오른쪽=OnMyWay, 아래=AssistMe, 왼쪽=Missing
        if (Mathf.Approximately(max, up)) return PingType.DefendMf;
        if (Mathf.Approximately(max, right)) return PingType.OnMyWay;
        if (Mathf.Approximately(max, down)) return PingType.AssistMe;
        return PingType.Missing;
    }

    // -------- Mirror 통신 ----------

    [Command]
    private void CmdSpawnPing(Vector3 position, PingType type)
    {
        GameObject obj = Instantiate(pingPrefab, position, Quaternion.identity);

        PingMarker marker = obj.GetComponent<PingMarker>();
        if (marker != null)
        {
            marker.Type = type;
        }

        NetworkServer.Spawn(obj);
    }


}
