using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections.Generic;

public class NetworkingManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _playButton;
    [SerializeField] private byte _maxPlayers = 4;
    [SerializeField] private byte _levelIndex = 1;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        _playButton.interactable = PhotonNetwork.IsConnectedAndReady;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        _playButton.interactable = true;
        Debug.Log("Connected to Master.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        _playButton.interactable = false;
        Debug.LogError("Disconnected: " + cause.ToString());
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Join random room failed, creating a new room.");
        int randomName = Random.Range(0, 5000);
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = _maxPlayers
        };

        PhotonNetwork.CreateRoom($"RoomName_{randomName}", roomOptions);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Successfully created a room.");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined a room.");
        PhotonNetwork.LoadLevel(_levelIndex);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            Debug.Log("Room: " + room.Name + ", Open: " + room.IsOpen + ", Visible: " + room.IsVisible);
        }
    }

    public void FindGame()
    {
        Debug.Log("Attempting to join a random room.");
        PhotonNetwork.JoinRandomRoom();
    }
}