using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Pulsar.Debug;  // Add this to use the DebugUtils

public class RoomPanel : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerListItemPrefab;
    [SerializeField] private Transform redTeamContainer;
    [SerializeField] private Transform blueTeamContainer;

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
        foreach (var item in playerListItems)
        {
            Destroy(item);
        }
        playerListItems.Clear();

        DebugUtils.LogAllArrayItems(PhotonNetwork.PlayerList);

        foreach (var player in PhotonNetwork.PlayerList)
        {
            AddPlayerToList(player);
        }
    }

    private void AddPlayerToList(Photon.Realtime.Player player)
    {
        if (DebugUtils.CheckForNull(player)) return;

        int teamId = (int)player.CustomProperties["team"];
        Transform parent = (teamId == 1) ? redTeamContainer : blueTeamContainer;

        if (DebugUtils.CheckForNull(parent)) return;
        if (DebugUtils.CheckForNull(playerListItemPrefab)) return;

        GameObject itemGO = Instantiate(playerListItemPrefab, parent);
        if (DebugUtils.CheckForNull(itemGO)) return;

        TMP_Text textComponent = itemGO.GetComponent<TMP_Text>();
        if (DebugUtils.CheckForNull(textComponent)) return;

        textComponent.text = player.NickName;
        playerListItems.Add(itemGO);
    }

    private void OnEnable()
    {
        RefreshPlayerList();
    }
}