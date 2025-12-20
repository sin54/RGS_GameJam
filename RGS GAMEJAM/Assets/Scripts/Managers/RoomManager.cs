using UnityEngine;
using Mirror;
using Mirror.Discovery;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Mirror.Examples.BilliardsPredicted;
using UnityEngine.TextCore.Text;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private RoomSync roomSync;

    [Header("Ready")]
    [SerializeField] private Button hostStartBtn;
    [SerializeField] private TMP_Text clientReadyTxt;

    [Header("CharacterSelect")]
    [SerializeField] private GameObject charSelPanel;
    [SerializeField] private TMP_Text[] charactSelTexts;

    [Header("Roomsetting")]
    [SerializeField] private GameObject roomSetPanel;
    [SerializeField] private Toggle isPrivateToggle;
    [SerializeField] private TMP_InputField roomNameField;
    [SerializeField] private Slider playerAmountSlider;
    [SerializeField] private TMP_Text playerAmountTxt;
    [SerializeField] private Button roomChangeBtn;
    private void Awake()
    {
    }
    private void Start()
    {
        charSelPanel.SetActive(true);
        roomSetPanel.SetActive(false);
        if (NetworkServer.active)
        {
            hostStartBtn.gameObject.SetActive(true);
            clientReadyTxt.gameObject.SetActive(false);
            playerAmountSlider.enabled = true;
            isPrivateToggle.enabled = true;
            roomNameField.enabled = true;
            roomChangeBtn.enabled = true;
        }
        else
        {
            hostStartBtn.gameObject.SetActive(false);
            clientReadyTxt.gameObject.SetActive(true);
            playerAmountSlider.enabled = false;
            isPrivateToggle.enabled = false;
            roomNameField.enabled = false;
            roomChangeBtn.enabled = false;
        }

    }

    private void Update()
    {
        playerAmountTxt.text = playerAmountSlider.value.ToString();
        if (roomSync.GetRoomData().maxPlayers == 1)
        {
            hostStartBtn.interactable = true;
        }
        else
        {
            if (((NetworkRoomManager)NetworkManager.singleton).allPlayersReady)
            {
                hostStartBtn.interactable = true;
            }
            else
            {
                hostStartBtn.interactable = false;
            }
        }

    }

    public void OnPressedStartButton()
    {
        ((CustomNetworkRoomManager)NetworkManager.singleton).StartGame();
    }
    public void OnChangeReadyState(bool readyState)
    {
        if (readyState)
        {
            clientReadyTxt.text = "준비완료";
            clientReadyTxt.color = Color.blue;
        }
        else
        {
            clientReadyTxt.text = "스페이스키를 눌러 준비하세요";
            clientReadyTxt.color = Color.white;
        }
    }

    public void ChangeRoomSetting()
    {
        RoomData createRoomData = new RoomData();
        if (string.IsNullOrWhiteSpace(roomNameField.text))
        {
            createRoomData.roomName = "Room " + Random.Range(1, 100).ToString();
        }
        else
        {
            createRoomData.roomName = roomNameField.text;
        }
        createRoomData.maxPlayers = (int)playerAmountSlider.value >= roomSync.GetRoomData().currentPlayers ? (int)playerAmountSlider.value : roomSync.GetRoomData().currentPlayers;
        createRoomData.isPrivate = isPrivateToggle.isOn;
        createRoomData.currentPlayers = roomSync.GetRoomData().currentPlayers;
        createRoomData.roomState = RoomState.Waiting;
        roomSync.SetRoomData(createRoomData);
    }

    public void UpdateRoomUI(RoomData roomData)
    {
        roomNameField.text = roomData.roomName;
        isPrivateToggle.isOn = roomData.isPrivate;
        playerAmountSlider.value = roomData.maxPlayers;
    }

    public void ShowCharSelPanel()
    {
        charSelPanel.SetActive(true);
        roomSetPanel.SetActive(false);
    }
    public void LockPlayerInput() => PlayerInputLock.LockInput();
    public void UnlockPlayerInput() => PlayerInputLock.UnlockInput();
    public void ShowRoomSetPanel()
    {
        roomSetPanel.SetActive(true);
        charSelPanel.SetActive(false);
    }
    public void OnCharacterChanged(int charNum)
    {
        for (int i = 0; i < charactSelTexts.Length; i++)
        {
            charactSelTexts[i].text = "";
        }
        charactSelTexts[charNum].text = "O";
        ((CustomNetworkRoomManager)NetworkManager.singleton).SetCharacter((Character)charNum);
        RoomPlayer.localRoomPlayer.CharacterChange((Character)charNum);
    }
}
