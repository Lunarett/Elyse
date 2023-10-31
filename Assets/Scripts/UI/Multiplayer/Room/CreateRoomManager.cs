using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateRoomManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Dropdown gameModeDropdown;
    [SerializeField] private TMP_Dropdown mapDropdown;
    [SerializeField] private Slider maxPlayersSlider;
    [SerializeField] private TMP_Text maxPlayersText;
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private TMP_InputField roomNameInputField;
    [SerializeField] private Button createRoomButton;

    private void Awake()
    {
        maxPlayersSlider.onValueChanged.AddListener(SetSliderText);
        roomNameInputField.onValueChanged.AddListener(OnInputValueChanged);
    }

    private void Start()
    {
        SetSliderText(maxPlayersSlider.value);
    }
    
    public void TryCreateRoom()
    {
        MultiplayerManager.Instance.CreateRoom(roomNameInputField.text, (byte)maxPlayersSlider.value);
    }
    
    public void OnCreateRoomButtonClicked()
    {
        string roomName = roomNameInputField.text;

        if (!this.IsRoomNameValid(roomName)) return;
        string gameMode = this.gameModeDropdown.options[this.gameModeDropdown.value].text;
        string map = this.mapDropdown.options[this.mapDropdown.value].text;
        byte maxPlayers = (byte)maxPlayersSlider.value;

        MultiplayerManager.Instance.CreateRoom(roomName, maxPlayers);
    }

    private bool IsRoomNameValid(string roomName)
    {
        if (!string.IsNullOrEmpty(roomName)) return true;
        errorText.text = "You need a name for the room!";
        return false;
    }
    
    private void OnInputValueChanged(string name)
    {
        createRoomButton.interactable = IsRoomNameValid(name);
    }

    private void SetSliderText(float value)
    {
        maxPlayersText.text = Mathf.RoundToInt(value).ToString();
    }
}