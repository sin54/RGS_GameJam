using Mirror.Discovery;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using static Unity.Cinemachine.CinemachineSplineRoll;
using Unity.VisualScripting;

public class HCManager : MonoBehaviour
{
    private NewNetworkDiscovery networkDiscovery;

    [SerializeField] private TMP_InputField roomNameField;
    [SerializeField] private Slider playerAmountSlider;
    [SerializeField] private TMP_Text playerAmountTxt;
    [SerializeField] private Toggle isPrivateToggle;

    private RoomData createRoomData;

    private void Awake()
    {
        networkDiscovery = GameObject.Find("NetworkManager").GetComponent<NewNetworkDiscovery>();
    }
    private void Update()
    {
        playerAmountTxt.text = ((int)playerAmountSlider.value).ToString();
    }
    public void CreateRoom()
    {
        if (string.IsNullOrWhiteSpace(roomNameField.text))
        {
            createRoomData.roomName = "Room " + Random.Range(1, 100).ToString();
        }
        else
        {
            createRoomData.roomName = roomNameField.text;
        }
        createRoomData.maxPlayers = (int)playerAmountSlider.value;
        createRoomData.isPrivate = isPrivateToggle.isOn;
        createRoomData.currentPlayers = 0;
        createRoomData.roomState = RoomState.Waiting;

        NetworkManager.singleton.StartHost();
        if (!createRoomData.isPrivate)
        {
            networkDiscovery.AdvertiseServer();
        }
        ((CustomNetworkRoomManager)CustomNetworkRoomManager.singleton).SetTempRoomData(createRoomData);
    }

    public void GotoTitle()
    {
        Loader.Load(Loader.Scene.StartScene);
    }
}
