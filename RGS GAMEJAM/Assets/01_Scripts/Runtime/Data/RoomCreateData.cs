using System;

[Serializable]
public struct RoomCreateData
{
    public string roomName;
    public int maxPlayers;
    public bool isPrivate;
}