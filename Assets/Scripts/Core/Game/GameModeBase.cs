using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;

public abstract class GameModeBase : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerController _playerController;

    protected PhotonView _photonView;
    protected List<PlayerController> _playerControllerList = new List<PlayerController>();

    protected virtual void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    protected virtual void Start()
    {
        if (!PhotonNetwork.IsConnectedAndReady) return;
        InstantiateController();
    }

    public override void OnJoinedRoom()
    {
        InstantiateController();
    }

    protected void InstantiateController()
    {
        if (_playerController == null)
        {
            Debug.LogError("GameModeBase: Cannot instantiate PlayerController! Missing reference!");
            return;
        }

        PlayerController controller = PhotonNetwork.Instantiate(
            Path.Combine("PhotonPrefabs", "Controllers", _playerController.gameObject.name),
            Vector3.zero,
            Quaternion.identity
        ).GetComponent<PlayerController>();

        if (controller == null)
        {
            Debug.LogError($"GameModeBase: Failed to find {_playerController.gameObject.name} prefab at 'Resources/PhotonPrefabs/Controllers' directory!");
            return;
        }

        _playerControllerList.Add(controller);
    }
}