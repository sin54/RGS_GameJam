using Mirror;
using System;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientUIView : MonoBehaviour
{
    [Header("Direct Connect Panel Prefab")]
    [SerializeField] private GameObject directPanelPrefab;
    private GameObject directPanelInstance;

    [Header("UI Elements")]
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TMP_Text nowInfo;
    [SerializeField] private Button openDirectButton;
    [SerializeField] private Button closeDirectButton;
    [SerializeField] private Button connectButton;
    [SerializeField] private Button refreshButton;

    [Header("Server List")]
    [SerializeField] private GameObject roomListItemPrefab;
    [SerializeField] private Transform content;

    // View -> 외부로 보내는 이벤트
    public event Action OnOpenDirect;
    public event Action OnCloseDirect;
    public event Action <string> OnConnectByIp;
    public event Action OnRefreshDiscovery;
    public event Action <RoomData, Uri, long> OnJoinRoom;

    // 내부에서 관리하는 UI 인스턴스들
    private readonly Dictionary<long, GameObject> roomItems = new Dictionary<long, GameObject>();

    private void Awake()
    {
        if (directPanelPrefab != null)
        {
            directPanelInstance = Instantiate(directPanelPrefab, transform);
            directPanelInstance.SetActive(false);
        }
    }

    private void OnEnable()
    {
        openDirectButton?.onClick.AddListener(HandleOpenDirectClicked);
        closeDirectButton?.onClick.AddListener(HandleCloseDirectClicked);
        connectButton?.onClick.AddListener(HandleConnectClicked);
        refreshButton?.onClick.AddListener(HandleRefreshClicked);
    }

    private void OnDisable()
    {
        openDirectButton?.onClick.RemoveListener(HandleOpenDirectClicked);
        closeDirectButton?.onClick.RemoveListener(HandleCloseDirectClicked);
        connectButton?.onClick.RemoveListener(HandleConnectClicked);
        refreshButton?.onClick.RemoveListener(HandleRefreshClicked);
    }

    private void HandleOpenDirectClicked()
    {
        ShowDirectPanel(true);
        OnOpenDirect?.Invoke();
    }

    private void HandleCloseDirectClicked()
    {
        ShowDirectPanel(false);
        OnCloseDirect?.Invoke();
    }

    private void HandleConnectClicked()
    {
        string ip = GetIpInput();
        OnConnectByIp?.Invoke(ip);
    }

    private void HandleRefreshClicked()
    {
        OnRefreshDiscovery?.Invoke();
    }

    public string GetIpInput()
    {
        return ipInputField != null ? ipInputField.text : string.Empty;
    }

    public void SetNowInfo(string text, Color color)
    {
        if (nowInfo != null)
        {
            nowInfo.text = text;
            nowInfo.color = color;
        }
    }

    public void ShowDirectPanel(bool show)
    {
        if (directPanelInstance != null) directPanelInstance.SetActive(show);
    }

    // 서버 리스트 항목 추가 또는 갱신
    public void AddOrUpdateRoomItem(RoomData roomData, Uri uri, long serverId)
    {
        if (roomListItemPrefab == null || content == null) return;

        if (roomItems.TryGetValue(serverId, out GameObject existing))
        {
            var setter = existing.GetComponent<RoomListSetter>();
            if (setter != null) setter.SetRoomList(roomData, uri, serverId);
            return;
        }

        GameObject item = Instantiate(roomListItemPrefab, content);
        roomItems[serverId] = item;

        var roomSetter = item.GetComponent<RoomListSetter>();
        if (roomSetter != null)
        {
            roomSetter.SetRoomList(roomData, uri, serverId);
        }

        // 버튼이 prefab 루트에 있거나 RoomListSetter가 버튼을 노출하지 않는 경우를 대비
        var btn = item.GetComponentInChildren<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() => HandleRoomItemClicked(roomData, uri, serverId));
        }
    }

    // 서버 항목 제거
    public void RemoveRoomItem(long serverId)
    {
        if (roomItems.TryGetValue(serverId, out GameObject item))
        {
            Destroy(item);
            roomItems.Remove(serverId);
        }
    }

    private void HandleRoomItemClicked(RoomData roomData, Uri uri, long serverId)
    {
        OnJoinRoom?.Invoke(roomData, uri, serverId);
    }

    // 전체 리스트 초기화
    public void ClearRoomList()
    {
        foreach (var kv in roomItems)
        {
            if (kv.Value != null) Destroy(kv.Value);
        }
        roomItems.Clear();
    }

}
