using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomList : MonoBehaviourPunCallbacks
{
    [SerializeField] private RoomNameElement _roomNameElementPrefab;

    [Header("References")]
    [SerializeField] private Transform _content;

    private Dictionary<string, RoomNameElement> _currentRoomElements = new Dictionary<string, RoomNameElement>();

    private void OnEnable()
    {
        RefreshRoomList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateRoomList(roomList);
    }

    private void RefreshRoomList()
    {
        if (PhotonNetwork.CurrentLobby.Type == LobbyType.SqlLobby)
        {
            string sqlFilter = "C0 <> ''"; // Assuming 'C0' is a non-empty string property in all rooms
            PhotonNetwork.GetCustomRoomList(MultiplayerManager.Instance.SqlLobby, sqlFilter);
        }
        else
        {
            Debug.LogError("Not in a SqlLobby. Cannot fetch custom game list.");
        }
    }


    private void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo roomInfo in roomList)
        {
            // Room removed or not visible anymore
            if (!roomInfo.IsOpen || !roomInfo.IsVisible || roomInfo.RemovedFromList)
            {
                if (_currentRoomElements.ContainsKey(roomInfo.Name))
                {
                    Destroy(_currentRoomElements[roomInfo.Name].gameObject);
                    _currentRoomElements.Remove(roomInfo.Name);
                }
                continue;
            }

            // Room added or updated
            if (_currentRoomElements.ContainsKey(roomInfo.Name))
            {
                _currentRoomElements[roomInfo.Name].SetName(roomInfo.Name);
                _currentRoomElements[roomInfo.Name].SetPlayerCount(roomInfo.PlayerCount, roomInfo.MaxPlayers);
            }
            else
            {
                RoomNameElement newRoomElement = Instantiate(_roomNameElementPrefab, _content);
                newRoomElement.SetName(roomInfo.Name);
                newRoomElement.SetPlayerCount(roomInfo.PlayerCount, roomInfo.MaxPlayers);
                _currentRoomElements[roomInfo.Name] = newRoomElement;
            }
        }
    }
}
