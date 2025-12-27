
using Mirror;

public enum RoomState { Waiting, InGame }

[System.Serializable]
public struct RoomData
{
    public string roomName;       // 방 이름
    public RoomState roomState;   // 방 상태
    public int currentPlayers;    // 현재 인원
    public int maxPlayers;        // 최대 인원
    public bool isPrivate;        // 프라이빗 서버
    public RoomData(
    string roomName,
    RoomState roomState,
    int currentPlayers,
    int maxPlayers,
    bool isPrivate
)
    {
        this.roomName = roomName;
        this.roomState = roomState;
        this.currentPlayers = currentPlayers;
        this.maxPlayers = maxPlayers;
        this.isPrivate = isPrivate;
    }
}
