using System;
using System.IO;
using UnityEngine;
using Photon.Pun;

public class GameModeBase : MonoBehaviourPunCallbacks
{
    private PhotonView _pv;

    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            return;
        }
        InstantiateController();
    }

    public override void OnJoinedRoom()
    {
        InstantiateController();
    }

    void InstantiateController()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Controllers", "PlayerController"), Vector3.zero, Quaternion.identity);
    }
}