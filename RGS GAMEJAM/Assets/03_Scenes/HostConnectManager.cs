using Mirror;
using Mirror.Discovery;
using UnityEngine;

public class HostConnectManager : MonoBehaviour
{
    /*
    네트워크만 담당
    Mirror / Discovery 접근
    */

    private NewNetworkDiscovery discovery;

    private void Awake()
    {
        Debug.Log("[HostConnectManager] Awake");

        discovery = FindObjectOfType<NewNetworkDiscovery>();
    }

    public void CreateRoom(RoomData roomData)
    {
        Debug.Log("[HostConnectManager] CreateRoom called");
        Debug.Log($"[HostConnectManager] roomData: {roomData.roomName}");
        Debug.Log($"[HostConnectManager] roomData: {roomData.maxPlayers}");
        Debug.Log($"[HostConnectManager] roomData: {roomData.isPrivate}");
        Debug.Log($"[HostConnectManager] roomData: {roomData.currentPlayers}");
        Debug.Log($"[HostConnectManager] roomData: {roomData.roomState}");

        NetworkManager.singleton.StartHost();

        if (!roomData.isPrivate)
        {
            discovery.AdvertiseServer();
        }

        ((CustomNetworkRoomManager)CustomNetworkRoomManager.singleton)
            .SetTempRoomData(roomData);
    }
}