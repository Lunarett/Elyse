using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomNameElement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text _roomNameText;
    [SerializeField] private TMP_Text _playerCountText;
    
    private Button _button;
    private string _roomName;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleButtonClick);
    }

    public void SetName(string name)
    {
        _roomNameText.text = name;
        _roomName = name;
    }

    public void SetPlayerCount(int count, int max)
    {
        _playerCountText.text = $"{count}/{max}";
    }
    
    private void HandleButtonClick()
    {
        RoomManager.Instance.JoinRoom(_roomNameText.text);
    }
}
