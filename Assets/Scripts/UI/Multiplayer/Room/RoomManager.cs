using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Pulsar.Utils;

[RequireComponent(typeof(PanelManager))]
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

        _panelManager = GetComponent<PanelManager>();
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

    public void CreateRoom(string roomName, int maxPlayers, string gameMode, string map)
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)maxPlayers;
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gameMode", gameMode }, { "map", map } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "gameMode", "map" }; // Properties to be listed in the lobby.

        PhotonNetwork.CreateRoom(roomName, roomOptions, null);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnJoinedRoom()
    {
        _panelManager.ShowPanel(_roomPanelIndex);
        AssignTeam();
    }
    
    private void AssignTeam()
    {
        int redTeamCount = 0;
        int blueTeamCount = 0;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            object teamId;
            if (!player.CustomProperties.TryGetValue("team", out teamId))
                continue;
            switch ((int)teamId)
            {
                case 1:
                    redTeamCount++;
                    break;
                case 2:
                    blueTeamCount++;
                    break;
            }
        }

        Player localPlayer = PhotonNetwork.LocalPlayer;
        localPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "team", (redTeamCount <= blueTeamCount) ? 1 : 2 } });
    }

    public override void OnLeftRoom()
    {
        _panelManager.ShowPanel(_returnPanelIndex);
    }
}