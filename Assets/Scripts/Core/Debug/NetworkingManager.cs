using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

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
        PhotonNetwork.NickName = $"Player_{Random.Range(0, 99)}";
        _playButton.interactable = true;
    }
    
    public void FindGame()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        int randomName = Random.Range(0, 5000);
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = _maxPlayers
        };

        PhotonNetwork.CreateRoom($"RoomName_{randomName}", roomOptions);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(_levelIndex);
    }
    
    public override void OnDisconnected(DisconnectCause cause)
    {
        _playButton.interactable = false;
        Debug.LogWarning("Disconnected: " + cause.ToString());
    }
}