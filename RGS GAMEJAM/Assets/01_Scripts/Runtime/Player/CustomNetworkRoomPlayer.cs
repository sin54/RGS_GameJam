using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
public class CustomNetworkRoomPlayer : NetworkRoomPlayer
{
    [SerializeField] private TMP_Text playerNumTxt;
    [SerializeField] private GameObject isHostImg;
    private PlayerInputHandler playerInputHandler;
    private RoomManager roomManager;

    private void Awake()
    {
        playerInputHandler = GetComponent<PlayerInputHandler>();
        roomManager=GameObject.Find("RoomManager")?.GetComponent<RoomManager>();
    }
    public override void Start()
    {
        base.Start();
        if (isServer && isLocalPlayer)
        {
            CmdChangeReadyState(true);
        }
    }
    private void Update()
    {
        if (readyToBegin)
        {
            playerNumTxt.color = Color.blue;
        }
        else
        {
            playerNumTxt.color = Color.red;
        }
        if (!isLocalPlayer) return;
        if (playerInputHandler.interactionJustPressed && !isServer)
        {
            if (readyToBegin)
            {
                CmdChangeReadyState(false);
                roomManager.OnChangeReadyState(false);
            }
            else
            {
                CmdChangeReadyState(true);
                roomManager.OnChangeReadyState(true);
            }
        }
        
    }
    public override void OnClientEnterRoom()
    {
        base.OnClientEnterRoom();
        playerNumTxt.text = "P" + (1 + index).ToString();
        if (index == 0)
        {
            isHostImg.SetActive(true);
        }
        else
        {
            isHostImg.SetActive(false);
        }
        if (isLocalPlayer)
        {
            ((CustomNetworkRoomManager)NetworkManager.singleton).SetIndex(index + 1);
        }
    }

    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        base.ReadyStateChanged(oldReadyState, newReadyState);
    }




}
