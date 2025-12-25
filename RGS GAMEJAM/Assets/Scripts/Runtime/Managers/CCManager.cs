using Mirror;
using Mirror.Discovery;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CCManager : MonoBehaviour
{
    [SerializeField] private GameObject directPanel;
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TMP_Text nowInfo;
    [SerializeField] private GameObject roomListPrefab;
    [SerializeField] private Transform content;

    private NewNetworkDiscovery networkDiscovery;

    readonly Dictionary<long, DiscoveryResponse> discoveredServers = new Dictionary<long, DiscoveryResponse>();
    private Dictionary<long, GameObject> discoveredServersUI = new Dictionary<long, GameObject>();
    private void Awake()
    {
        networkDiscovery = GameObject.Find("NetworkManager").GetComponent<NewNetworkDiscovery>();
        networkDiscovery.OnServerFound.AddListener(OnDiscoveredServer);
    }

    private void Start()
    {
        nowInfo.text = "";
        directPanel.SetActive(false);
        RefreshServerList();
    }
    private void OnEnable()
    {
        NetworkClient.OnConnectedEvent += HandleConnect;
        NetworkClient.OnDisconnectedEvent += HandleDisconnect;
    }

    private void OnDisable()
    {
        NetworkClient.OnDisconnectedEvent -= HandleConnect;
        NetworkClient.OnDisconnectedEvent -= HandleDisconnect;
    }
    public void OpenDirectPanel()
    {
        directPanel.SetActive(true);
    }
    public void CloseDirectPanel()
    {
        directPanel.SetActive(false);
    }
    public void ClientStart()
    {
        nowInfo.text = "Connecting...";
        nowInfo.color = Color.green;
        string ip = ipInputField.text;
        if (!string.IsNullOrEmpty(ip))
        {
            NetworkManager.singleton.networkAddress = ip;
            NetworkManager.singleton.StartClient();
        }
    }
    private void HandleConnect()
    {
        nowInfo.text = "Connected!";
        nowInfo.color = Color.blue;
    }

    private void HandleDisconnect()
    {
        nowInfo.text = "Failed!";
        nowInfo.color = Color.red;
    }

    public void GotoTitle()
    {
        Loader.Load(Loader.Scene.StartScene);
    }

    private void RefreshServerList()
    {
        discoveredServers.Clear();
        networkDiscovery.StartDiscovery();
    }

    private void OnDiscoveredServer(DiscoveryResponse info)
    {
        discoveredServers[info.serverId] = info;

        if (discoveredServersUI.TryGetValue(info.serverId, out GameObject roomUI))
        {
            roomUI.GetComponent<RoomListSetter>().SetRoomList(info.roomData, info.uri, info.serverId);
        }
        else
        {
            roomUI = Instantiate(roomListPrefab, content);
            discoveredServersUI[info.serverId] = roomUI;
            roomUI.GetComponent<RoomListSetter>().SetRoomList(info.roomData, info.uri, info.serverId);
        }

        Debug.Log($"서버 발견: {info.serverId}, {info.EndPoint}");
    }

    public void FoundServer(RoomData roomData, Uri link, long id)
    {
        networkDiscovery.StopDiscovery();

        NetworkManager.singleton.StartClient(link);
    }

}
