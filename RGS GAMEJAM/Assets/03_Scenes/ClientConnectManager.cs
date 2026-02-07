using Mirror;
using Mirror.Discovery;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ClientConnectManager : MonoBehaviour
{
    [Header("UI Prefab")]
    [SerializeField] private GameObject roomListPrefab;    // Room list item prefab (RoomListSetter 포함)
    [SerializeField] private Transform content;            // 반드시 Canvas 하위의 RectTransform

    [Header("Discovery")]
    [SerializeField] private string networkManagerObjectName = "NetworkManager";

    private NewNetworkDiscovery networkDiscovery;

    // 발견된 서버 데이터 저장
    private readonly Dictionary<long, DiscoveryResponse> discoveredServers = new Dictionary<long, DiscoveryResponse>();
    private readonly Dictionary<long, GameObject> discoveredServersUI = new Dictionary<long, GameObject>();

    // 외부(Controller)에서 구독할 이벤트
    public event Action OnConnected;
    public event Action OnDisconnected;

    private void Awake()
    {
        var nmObj = GameObject.Find(networkManagerObjectName);
        if (nmObj == null)
        {
            Debug.LogError($"[{nameof(ClientConnectManager)}] '{networkManagerObjectName}' 오브젝트를 찾을 수 없습니다.");
            return;
        }

        networkDiscovery = nmObj.GetComponent<NewNetworkDiscovery>();
        if (networkDiscovery == null)
        {
            Debug.LogError($"[{nameof(ClientConnectManager)}] NewNetworkDiscovery 컴포넌트가 없습니다.");
        }
        else
        {
            networkDiscovery.OnServerFound.AddListener(OnDiscoveredServer);
        }

        if (content == null)
        {
            Debug.LogError($"[{nameof(ClientConnectManager)}] content(서버 리스트 부모)가 할당되지 않았습니다.");
        }

        if (roomListPrefab == null)
        {
            Debug.LogError($"[{nameof(ClientConnectManager)}] roomListPrefab이 할당되지 않았습니다.");
        }
    }

    private void OnEnable()
    {
        NetworkClient.OnConnectedEvent += HandleConnect;
        NetworkClient.OnDisconnectedEvent += HandleDisconnect;
    }

    private void OnDisable()
    {
        NetworkClient.OnConnectedEvent -= HandleConnect;
        NetworkClient.OnDisconnectedEvent -= HandleDisconnect;
    }

    // IP로 직접 연결
    public void StartClientByIp(string ip)
    {
        if (string.IsNullOrEmpty(ip))
        {
            Debug.LogWarning($"[{nameof(ClientConnectManager)}] StartClientByIp: ip가 비어있음");
            return;
        }

        if (NetworkManager.singleton == null)
        {
            Debug.LogError($"[{nameof(ClientConnectManager)}] NetworkManager.singleton이 null입니다.");
            return;
        }

        NetworkManager.singleton.networkAddress = ip;
        NetworkManager.singleton.StartClient();
    }

    // Discovery 재시작 (Controller에서 호출)
    public void RefreshServerList()
    {
        ClearServerListCache();
        if (networkDiscovery == null)
        {
            Debug.LogWarning($"[{nameof(ClientConnectManager)}] networkDiscovery가 없음. Discovery를 시작할 수 없습니다.");
            return;
        }
        networkDiscovery.StartDiscovery();
    }

    // Discovery 콜백: 서버 발견 시 UI 생성/갱신
    private void OnDiscoveredServer(DiscoveryResponse info)
    {
        if (info.serverId == null) return;

        discoveredServers[info.serverId] = info;

        if (discoveredServersUI.TryGetValue(info.serverId, out GameObject existing))
        {
            var setter = existing.GetComponent<RoomListSetter>();
            if (setter != null) setter.SetRoomList(info.roomData, info.uri, info.serverId);
        }
        else
        {
            if (roomListPrefab == null || content == null) return;

            GameObject item = Instantiate(roomListPrefab, content);
            discoveredServersUI[info.serverId] = item;

            var setter = item.GetComponent<RoomListSetter>();
            if (setter != null)
            {
                setter.SetRoomList(info.roomData, info.uri, info.serverId);

                // RoomListSetter 내부에서 버튼 콜백을 제공하지 않으면 여기서 연결
                var joinBtn = item.GetComponentInChildren<UnityEngine.UI.Button>();
                if (joinBtn != null)
                {
                    // 캡처 주의: 로컬 변수로 복사
                    var capturedInfo = info;
                    joinBtn.onClick.AddListener(() => ConnectToFoundServer(capturedInfo.uri));
                }
            }
        }

        Debug.Log($"[{nameof(ClientConnectManager)}] 서버 발견: {info.serverId} / {info.EndPoint}");
    }

    // 발견된 서버로 연결 (Discovery에서 얻은 Uri 사용)
    public void ConnectToFoundServer(Uri link)
    {
        if (link == null)
        {
            Debug.LogWarning($"[{nameof(ClientConnectManager)}] ConnectToFoundServer: link가 null");
            return;
        }

        networkDiscovery?.StopDiscovery();

        if (NetworkManager.singleton == null)
        {
            Debug.LogError($"[{nameof(ClientConnectManager)}] NetworkManager.singleton이 null입니다.");
            return;
        }

        NetworkManager.singleton.StartClient(link);
    }

    // 연결/끊김 이벤트 처리
    private void HandleConnect()
    {
        Debug.Log($"[{nameof(ClientConnectManager)}] 클라이언트 연결됨");
        OnConnected?.Invoke();
    }

    private void HandleDisconnect()
    {
        Debug.Log($"[{nameof(ClientConnectManager)}] 클라이언트 연결 끊김");
        OnDisconnected?.Invoke();
    }

    // 내부 캐시 및 UI 정리
    public void ClearServerListCache()
    {
        discoveredServers.Clear();

        foreach (var kv in discoveredServersUI)
        {
            if (kv.Value != null) Destroy(kv.Value);
        }
        discoveredServersUI.Clear();
    }
}
