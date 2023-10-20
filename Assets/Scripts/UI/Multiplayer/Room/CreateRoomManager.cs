using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateRoomManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Dropdown gameModeDropdown;
    [SerializeField] private TMP_Dropdown mapDropdown;
    [SerializeField] private Slider maxPlayersSlider;
    [SerializeField] private TMP_InputField roomNameInputField;

    public void OnCreateRoomButtonClicked()
    {
        string roomName = roomNameInputField.text;

        if (!this.IsRoomNameValid(roomName)) return;
        string gameMode = this.gameModeDropdown.options[this.gameModeDropdown.value].text;
        string map = this.mapDropdown.options[this.mapDropdown.value].text;
        int maxPlayers = (int)this.maxPlayersSlider.value;

        RoomManager.Instance.CreateRoom(roomName, maxPlayers, gameMode, map);
    }

    private bool IsRoomNameValid(string roomName)
    {
        if (!string.IsNullOrEmpty(roomName))
        {
            return true;
        }

        Debug.LogError("Room Name is required.");
        return false;
    }
}