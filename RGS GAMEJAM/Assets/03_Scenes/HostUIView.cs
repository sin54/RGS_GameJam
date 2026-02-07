using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HostUIView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField roomNameField;
    [SerializeField] private Slider playerCountSlider;
    [SerializeField] private TMP_Text playerCountText;
    [SerializeField] private Toggle privateToggle;
    [SerializeField] private Button createButton;
    [SerializeField] private Button backButton;

    // UI → 외부로 보내는 이벤트
    public event Action <RoomCreateData> OnCreateRoom;
    public event Action OnBack;

    private void OnEnable()
    {
        UpdatePlayerCountText(playerCountSlider.value);

        playerCountSlider.onValueChanged.AddListener(UpdatePlayerCountText);
        createButton.onClick.AddListener(HandleCreateClicked);
        backButton.onClick.AddListener(HandleBackClicked);
    }

    private void OnDisable()
    {
        playerCountSlider.onValueChanged.RemoveListener(UpdatePlayerCountText);
        createButton.onClick.RemoveListener(HandleCreateClicked);
        backButton.onClick.RemoveListener(HandleBackClicked);
    }

    private void UpdatePlayerCountText(float value)
    {
        playerCountText.text = ((int)value).ToString();
    }

    private void HandleCreateClicked()
    {
        Debug.Log("[HostUIView] Create button clicked");

        OnCreateRoom?.Invoke(CollectData());
    }

    private void HandleBackClicked()
    {
        OnBack?.Invoke();
    }

    private RoomCreateData CollectData()
    {
        return new RoomCreateData
        {
            roomName = roomNameField.text,
            maxPlayers = (int)playerCountSlider.value,
            isPrivate = privateToggle.isOn
        };
    }
}
