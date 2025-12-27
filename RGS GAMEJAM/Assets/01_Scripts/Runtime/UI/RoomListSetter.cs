using Mirror.Discovery;
using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Cinemachine.CinemachineSplineRoll;

public class RoomListSetter : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameTxt;
    [SerializeField] private TMP_Text roomStateTxt;
    [SerializeField] private TMP_Text playerNumTxt;
    [SerializeField] private Button connectBtn;

    private CCManager CCM;
    private Uri uri;
    private long serverId;
    private void Awake()
    {
        CCM=GameObject.Find("CCManager").GetComponent<CCManager>();
    }
    public void SetRoomList(RoomData roomData, Uri link, long id)
    {
        roomNameTxt.text = roomData.roomName;
        roomStateTxt.text = roomData.roomState.ToString();
        playerNumTxt.text = roomData.currentPlayers.ToString() + "/" + roomData.maxPlayers.ToString();

        uri = link;
        serverId = id;

        connectBtn.onClick.RemoveAllListeners();
        connectBtn.onClick.AddListener(() => JoinRoom(roomData, link, id));
        if (CanJoinServer(roomData))
        {
            connectBtn.interactable = true;
        }
        else
        {
            connectBtn.interactable = false;
        }
    }
    private void JoinRoom(RoomData roomData, Uri link, long id)
    {
        CCM.FoundServer(roomData, link, id);
    }
    private bool CanJoinServer(RoomData roomData)
    {
        if (roomData.currentPlayers >= roomData.maxPlayers) return false;
        return true;
    }

}
