using System;
using UnityEngine;

public class ClientSceneController : MonoBehaviour
{
    /*
    역할
    - UI 이벤트 수신 (ClientUIView)
    - 입력 검증 및 UI 상태 제어
    - ClientConnectManager 호출 (네트워크 동작 위임)
    - 씬 흐름(타이틀 등) 제어
    */

    [Header("UI")]
    [SerializeField] private ClientUIView clientUIViewPrefab;
    [SerializeField] private Transform uiRoot;

    [Header("Network")]
    [SerializeField] private ClientConnectManager connectManager;

    private ClientUIView view;

    private void Awake()
    {
        // 필수 참조 체크
        if (clientUIViewPrefab == null)
        {
            Debug.LogError("[ClientSceneController] clientUIViewPrefab is not assigned.");
            return;
        }

        if (uiRoot == null)
        {
            Debug.LogWarning("[ClientSceneController] uiRoot is not assigned. Using this.transform as parent.");
            uiRoot = this.transform;
        }

        if (connectManager == null)
        {
            Debug.LogError("[ClientSceneController] ClientConnectManager is not assigned.");
            return;
        }

        // UI 인스턴스화
        view = Instantiate(clientUIViewPrefab, uiRoot);

        // View 이벤트 구독
        view.OnOpenDirect += HandleOpenDirect;
        view.OnCloseDirect += HandleCloseDirect;
        view.OnConnectByIp += HandleConnectByIp;
        view.OnRefreshDiscovery += HandleRefreshDiscovery;
        view.OnJoinRoom += HandleJoinRoom;

        // ConnectManager 이벤트 구독
        connectManager.OnConnected += HandleConnected;
        connectManager.OnDisconnected += HandleDisconnected;
    }

    private void OnDestroy()
    {
        // 이벤트 정리
        if (view != null)
        {
            view.OnOpenDirect -= HandleOpenDirect;
            view.OnCloseDirect -= HandleCloseDirect;
            view.OnConnectByIp -= HandleConnectByIp;
            view.OnRefreshDiscovery -= HandleRefreshDiscovery;
            view.OnJoinRoom -= HandleJoinRoom;
        }

        if (connectManager != null)
        {
            connectManager.OnConnected -= HandleConnected;
            connectManager.OnDisconnected -= HandleDisconnected;
        }
    }

    // =====================
    // UI Event Handlers
    // =====================

    private void HandleOpenDirect()
    {
        view.ShowDirectPanel(true);
    }

    private void HandleCloseDirect()
    {
        view.ShowDirectPanel(false);
    }

    private void HandleConnectByIp(string ip)
    {
        if (string.IsNullOrEmpty(ip))
        {
            view.SetNowInfo("IP를 입력하세요.", Color.red);
            return;
        }

        view.SetNowInfo("Connecting...", Color.green);
        connectManager.StartClientByIp(ip);
    }

    private void HandleRefreshDiscovery()
    {
        view.SetNowInfo("Searching servers...", Color.gray);
        connectManager.RefreshServerList();
        // ConnectManager가 발견 콜백에서 UI 항목을 직접 생성하는 구조라면
        // Controller는 추가 작업이 필요 없습니다.
        // 만약 Controller가 View에 항목을 직접 추가해야 한다면
        // ConnectManager에 서버 발견 이벤트를 추가하고 여기서 구독하세요.
    }

    private void HandleJoinRoom(RoomData roomData, Uri uri, long serverId)
    {
        // View에서 Join 요청이 들어왔을 때 처리
        view.SetNowInfo("Connecting to server...", Color.green);

        // ConnectManager에 Uri로 연결 요청
        connectManager.ConnectToFoundServer(uri);
    }

    // =====================
    // Network Event Handlers
    // =====================

    private void HandleConnected()
    {
        view.SetNowInfo("Connected!", Color.blue);
        // 씬 전환이나 추가 로직이 필요하면 여기서 처리
    }

    private void HandleDisconnected()
    {
        view.SetNowInfo("Connection failed.", Color.red);
        // 재시도 UI 표시 등 추가 처리 가능
    }

    // =====================
    // Public helpers (선택)
    // =====================

    // 외부에서 컨트롤러를 통해 UI를 강제로 초기화하거나 갱신할 필요가 있을 때 사용
    public void RefreshUI()
    {
        if (view != null)
        {
            view.ClearRoomList();
            view.SetNowInfo(string.Empty, Color.white);
        }
    }
}