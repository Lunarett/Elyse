using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;
using Pulsar.Debug;
using UnityEngine.UI;

public enum CustomEventCode
{
    ForceRefresh = 100,
    CharacterSelectionReady = 101,
    CharacterSelcetionChangedCharacter = 102,
    CharacterSelectionClass = 103
}

public class RoomPanel : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [Header("Panel Properties")]
    [SerializeField] private GameObject _userNamePrefab;
    [Space(5)]
    [SerializeField] private TMP_Text _roomName;
    [SerializeField] private TMP_Text _gameModeText;
    [Space(5)]
    [SerializeField] private Transform _userContainer;
    [SerializeField] private Button _startButton;
    
    private List<GameObject> playerListItems = new List<GameObject>();

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
        if (!PhotonNetwork.InRoom) return;
        UpdatePlayerList();
        _startButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Start()
    {
        _startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player entered room: " + newPlayer.NickName);
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("Player left room: " + otherPlayer.NickName);
        UpdatePlayerList();
    }
    
    public void UpdatePlayerList()
    {
        ClearPlayerList();
        
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject roomNameElement = Instantiate(_userNamePrefab, _userContainer);
            playerListItems.Add(roomNameElement);
            DebugUtils.CheckForNull<GameObject>(roomNameElement);
            TMP_Text name = roomNameElement.GetComponent<TMP_Text>();
            DebugUtils.CheckForNull<TMP_Text>(name);
            name.text = player.NickName;
        }
        _startButton.interactable = StartIsValid();
    }

    private void ClearPlayerList()
    {
        foreach (var item in playerListItems) Destroy(item);
        playerListItems.Clear();
    }
    
    private bool StartIsValid()
    {
        return playerListItems.Count > 2;
    }
    
    public void SetRoomName(string name)
    {
        _roomName.text = name;
    }

    public void SetGameModeName(string name)
    {
        _gameModeText.text = $"GameMode: {name}";
    }

    public void OnStartButtonClicked()
    {
        MultiplayerManager.Instance.StartMatch();
    }
    
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte) CustomEventCode.ForceRefresh)
        {
            //UpdatePlayerList();
        }
    }
}