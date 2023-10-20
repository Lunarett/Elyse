using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class RoomPanel : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerListItemPrefab;
    [SerializeField] private Transform redTeamContainer;
    [SerializeField] private Transform blueTeamContainer;

    // Store references to the instantiated player list items to manage them.
    private List<GameObject> playerListItems = new List<GameObject>();

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        RefreshPlayerList();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        RefreshPlayerList();
    }

    private void RefreshPlayerList()
    {
        // Clear existing list items.
        foreach (var item in playerListItems)
        {
            Destroy(item);
        }
        playerListItems.Clear();

        // Add each player to the list again.
        foreach (var player in PhotonNetwork.PlayerList)
        {
            AddPlayerToList(player);
        }
    }

    private void AddPlayerToList(Photon.Realtime.Player player)
    {
        int teamId = (int)player.CustomProperties["team"];
        Transform parent = (teamId == 1) ? redTeamContainer : blueTeamContainer;

        GameObject itemGO = Instantiate(playerListItemPrefab, parent);
        itemGO.GetComponent<TMP_Text>().text = player.NickName; // Make sure your prefab has a TextMeshProUGUI component for this to work.
        playerListItems.Add(itemGO);                            // Keep a reference for later.
    }

    // Call this when the RoomPanel is enabled/opened to initialize the player list.
    private void OnEnable()
    {
        RefreshPlayerList();
    }
}