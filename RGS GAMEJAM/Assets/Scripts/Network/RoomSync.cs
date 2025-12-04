using UnityEngine;
using Mirror;

public class RoomSync : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnRoomDataChanged))]
    private RoomData currentRoomData;

    [SerializeField] private RoomManager roomManager;

    private void Awake()
    {
    }
    public RoomData GetRoomData()
    {
        return currentRoomData;
    }

    [Server]
    public void SetRoomName(string newName)
    {
        currentRoomData = new RoomData(
            newName,
            currentRoomData.roomState,
            currentRoomData.currentPlayers,
            currentRoomData.maxPlayers,
            currentRoomData.isPrivate
        );
    }

    [Server]
    public void AddRoomPlayer()
    {
        currentRoomData = new RoomData(
            currentRoomData.roomName,
            currentRoomData.roomState,
            currentRoomData.currentPlayers + 1,
            currentRoomData.maxPlayers,
            currentRoomData.isPrivate
        );
    }

    [Server]
    public void RemoveRoomPlayer()
    {
        currentRoomData = new RoomData(
            currentRoomData.roomName,
            currentRoomData.roomState,
            currentRoomData.currentPlayers - 1,
            currentRoomData.maxPlayers,
            currentRoomData.isPrivate
        );
    }

    [Server]
    public void SetRoomMaxPlayer(int newMax)
    {
        currentRoomData = new RoomData(
            currentRoomData.roomName,
            currentRoomData.roomState,
            currentRoomData.currentPlayers,
            newMax,
            currentRoomData.isPrivate
        );
    }

    [Server]
    public void SetPrivateState(bool newState)
    {
        currentRoomData = new RoomData(
            currentRoomData.roomName,
            currentRoomData.roomState,
            currentRoomData.currentPlayers,
            currentRoomData.maxPlayers,
            newState
        );
    }

    [Server]
    public void SetRoomState(RoomState newState)
    {
        currentRoomData = new RoomData(
            currentRoomData.roomName,
            newState,
            currentRoomData.currentPlayers,
            currentRoomData.maxPlayers,
            currentRoomData.isPrivate
        );
    }

    [Server]
    public void SetRoomData(RoomData roomData)
    {
        currentRoomData = roomData;
    }

    private void OnRoomDataChanged(RoomData oldData, RoomData newData)
    {
        roomManager.UpdateRoomUI(newData);
    }
}
