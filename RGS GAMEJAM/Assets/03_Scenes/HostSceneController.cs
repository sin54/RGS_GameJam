using UnityEngine;

public class HostSceneController : MonoBehaviour
{
    /*
    UI 이벤트 수신
    RoomData 생성
    HostConnectManager 호출
    SceneFlowManager 호출
    */

    [Header("UI")]
    [SerializeField] private HostUIView hostUIViewPrefab;
    [SerializeField] private Transform uiRoot;

    [Header("Network")]
    [SerializeField] private HostConnectManager connectManager;

    private HostUIView view;

    private void Awake()
    {
        // 필수 참조 체크 (디버그용)
        if (hostUIViewPrefab == null)
        {
            Debug.LogError("[HostSceneController] HostUIViewPrefab is not assigned.");
            return;
        }

        if (connectManager == null)
        {
            Debug.LogError("[HostSceneController] HostConnectManager is not assigned");
            return; 
        }

        // UI 생성
        view = Instantiate(hostUIViewPrefab, uiRoot);

        // UI -> Logic 연결
        view.OnCreateRoom += HandleCreateRoom;
        view.OnBack += HandleBack;
    }

    private void OnDestroy()
    {
        // 씬 종료 시 이벤트 정리 (중요)
        if (view == null) return;

        view.OnCreateRoom -= HandleCreateRoom;
        view.OnBack -= HandleBack;
    }

    // =====================
    // UI Event Handlers
    // =====================

    private void HandleCreateRoom(RoomCreateData createdata)
    {
        Debug.Log("[HostSceneController] HandleCreateRoom called");
        Debug.Log($"[HostSceneController] createdata_roomName = {createdata.roomName}");

        RoomData roomData = new RoomData
        {
            roomName = createdata.roomName,
            maxPlayers = createdata.maxPlayers,
            isPrivate = createdata.isPrivate,
            currentPlayers = 0,
            roomState = RoomState.Waiting
        };

        connectManager.CreateRoom(roomData);
    }

    private void HandleBack()
    {
        SceneFlowManager.Instance.LoadScene(SceneType.Title);
    }
}

