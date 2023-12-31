using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Pulsar.Debug;
using TMPro;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text _connectionStatusText;
    [SerializeField] private int _levelIndex = 1;

    [Header("Room Properties")]
    [SerializeField] private RoomPanel _roomPanel;
    [Space]
    [SerializeField] private int _roomPanelIndex;
    [SerializeField] private int _returnPanelIndex;

    [Header("Room List Properties")]
    [SerializeField] private Transform _roomListParent;
    [SerializeField] private GameObject _roomListItemPrefab;

    private PanelManager _panelManager;
    private List<RoomInfo> _availableRooms = new List<RoomInfo>();
    private TypedLobby sqlLobby = new TypedLobby("mySqlLobby", LobbyType.SqlLobby);

    public TypedLobby SqlLobby => sqlLobby;

    private const int maxConnectionAttempts = 5;
    private int currentConnectionAttempts = 0;

    public static MultiplayerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        ConnectToServer();

        _panelManager = GetComponent<PanelManager>();
        DebugUtils.CheckForNull<PanelManager>(_panelManager);
    }

    public void ConnectToServer()
    {
        if (currentConnectionAttempts < maxConnectionAttempts)
        {
            _connectionStatusText.text = "Connecting to Server... (Attempt " + (currentConnectionAttempts + 1) + ")";
            PhotonNetwork.ConnectUsingSettings();
            currentConnectionAttempts++;
        }
        else
        {
            _connectionStatusText.text =
                "Failed to connect after multiple attempts. Please check your connection and try again.";
            currentConnectionAttempts = 0;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        ConnectToServer();
    }

    public override void OnConnectedToMaster()
    {
        _connectionStatusText.text = "Joining Lobby...";
        currentConnectionAttempts = 0;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.JoinLobby(sqlLobby);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby: " + PhotonNetwork.CurrentLobby.Name + ", Type: " + PhotonNetwork.CurrentLobby.Type);
        _panelManager.ShowPanel(1);
        DisplayAvailableRooms();
    }

    public void JoinRoom(string roomName)
    {
        if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        _panelManager.ShowPanel(_roomPanelIndex);
        _roomPanel.SetRoomName(PhotonNetwork.CurrentRoom.Name);
    }

    public void CreateRoom(string roomName, byte maxPlayers)
    {
        if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;

        // Set custom room properties here
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties.Add("C0", "desert"); // The key for the custom property
        roomOptions.CustomRoomProperties = customProperties;

        // Define which properties are visible in the lobby
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "C0" };

        PhotonNetwork.CreateRoom(roomName, roomOptions, sqlLobby);
    }


    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        _availableRooms = roomList;
        DisplayAvailableRooms();
    }

    private void DisplayAvailableRooms()
    {
        foreach (Transform child in _roomListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (RoomInfo room in _availableRooms)
        {
            GameObject roomListItem = Instantiate(_roomListItemPrefab, _roomListParent);
            RoomNameElement roomNameElement = roomListItem.GetComponent<RoomNameElement>();
            if (DebugUtils.CheckForNull<RoomNameElement>(roomNameElement)) return;
            roomNameElement.SetName(room.Name);
            roomNameElement.SetPlayerCount(room.PlayerCount, room.MaxPlayers);
        }
    }

    public void StartMatch()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Master Client is loading the level.");
            PhotonNetwork.LoadLevel(1);
        }
        else
        {
            Debug.Log("Waiting for Master Client to load the level.");
        }
    }
}