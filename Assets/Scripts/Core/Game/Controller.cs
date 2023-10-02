using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[RequireComponent(typeof(PhotonView))]
public class Controller : MonoBehaviour
{
    protected PhotonView _photonView;
    private GameObject pawnOobject;

    private int deaths = 0;
    private int kills = 0;
    
    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (!_photonView.IsMine) return; 
        CreatePawn();
    }

    private void CreatePawn()
    {
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        pawnOobject = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Pawns", "PlayerCharacter"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { _photonView.ViewID });
    }
    
    public void Die()
    {
        PhotonNetwork.Destroy(pawnOobject);
        CreatePawn();

        deaths++;

        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void GetKill()
    {
        _photonView.RPC(nameof(RPC_GetKill), _photonView.Owner);
    }

    [PunRPC]
    void RPC_GetKill()
    {
        kills++;

        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public static Controller Find(Player player)
    {
        return FindObjectsOfType<Controller>().SingleOrDefault(x => x._photonView.Owner == player);
    }
}
