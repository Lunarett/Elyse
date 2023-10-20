using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Photon.Pun;

public class DisplayNameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text _errorMessageText;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _confirmButton;

    [Header("Name Properties")]
    [SerializeField] private int _maxCharacters = 20;
    [SerializeField] private string _forbiddenCharacters = "*()?|";

    public event Action OnNameSet;

    private void Awake()
    {
        _confirmButton.interactable = false;
        _errorMessageText.text = "";
        _inputField.onValueChanged.AddListener(ValidateName);
    }

    private void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            ConfirmName();
        }
    }

    private void ValidateName(string name)
    {
        if (name.Length < 1 || name.Length > _maxCharacters)
        {
            _errorMessageText.text = "Name must be between 1 and " + _maxCharacters + " characters.";
            _confirmButton.interactable = false;
            return;
        }

        foreach (char c in _forbiddenCharacters)
        {
            if (!name.Contains(c.ToString())) continue;
            _errorMessageText.text = "Name contains forbidden characters.";
            _confirmButton.interactable = false;
            return;
        }

        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.Players.Values.Any(p => p.NickName == name))
        {
            _errorMessageText.text = "Name is already in use.";
            _confirmButton.interactable = false;
            return;
        }

        _errorMessageText.text = "";
        _confirmButton.interactable = true;
    }

    public void ConfirmName()
    {
        if (!_confirmButton.interactable) return;
        PhotonNetwork.NickName = _inputField.text;
        OnNameSet?.Invoke();
    }
}
