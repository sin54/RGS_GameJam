using UnityEngine;
using Mirror;
using System.Linq;
public class CustomNetworkRoomManager : NetworkRoomManager
{
    private RoomSync roomSync;
    private RoomData tempRoomData;
    public override void Awake()
    {
        base.Awake();

    }
    public override void Start()
    {
        base.Start();
    }

    public void SetTempRoomData(RoomData roomData) {
        tempRoomData = roomData;
    }
    public void SetRoomData(RoomData roomData)
    {
        maxConnections = roomData.maxPlayers;
        roomSync.SetRoomData(roomData);
    }
    public RoomData GetRoomData()
    {
        return roomSync.GetRoomData();
    }
    public override void OnRoomServerSceneChanged(string sceneName)
    {
        base.OnRoomServerSceneChanged(sceneName);

        roomSync = GameObject.Find("RoomManager_Network").GetComponent<RoomSync>();
        SetRoomData(tempRoomData);
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        roomSync.AddRoomPlayer();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        roomSync.RemoveRoomPlayer();
    }
    public void StartGame()
    {
        if (allPlayersReady)
        {
            ServerChangeScene(GameplayScene);
        }
    }
    public override void OnRoomServerPlayersReady()
    {
    }
    public void ChangeCharacter(Character character)
    {

    }
}
