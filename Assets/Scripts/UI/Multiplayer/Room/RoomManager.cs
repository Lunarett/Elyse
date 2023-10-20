using UnityEngine;
using Photon.Pun;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [Header("Panel Indexes")]
    [SerializeField] private int _roomPanelIndex;
    [SerializeField] private int _returnPanelIndex;
    
    private PanelManager _panelManager;
    
    public static RoomManager Instance { get; private set; }

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
    }

    public void JoinRoom(string roomName)
    {
        if (!PhotonNetwork.InLobby)
        {
            Debug.LogWarning("You must be in a lobby to join a room.");
            return;
        }

        PhotonNetwork.JoinRoom(roomName);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnJoinedRoom()
    {
        _panelManager.ShowPanel(_roomPanelIndex);
    }

    public override void OnLeftRoom()
    {
        _panelManager.ShowPanel(_returnPanelIndex);
    }
}