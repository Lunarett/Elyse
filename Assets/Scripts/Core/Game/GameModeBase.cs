using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;
using Pulsar.Debug;

public class GameModeBase : MonoBehaviourPunCallbacks
{
    [Header("Player Settings")]
    [SerializeField] private PlayerController _playerControllerPrefab;

    [SerializeField] private Pawn _playerPawn;
    [SerializeField] private Pawn _spectatorPawn;

    protected List<PlayerController> _playerControllers = new List<PlayerController>();
    private PhotonView _photonView;

    protected virtual void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        DebugUtils.CheckForNull<PhotonView>(_photonView, "GameMode: PhotonView is missing!");
    }

    protected virtual void Start()
    {
        InstantiatePlayerController();
    }

    public override void OnJoinedRoom()
    {
        InstantiatePlayerController();
    }

    protected void InstantiatePlayerController()
    {
        if (DebugUtils.CheckForNull(_playerControllerPrefab, "GameModeBase: PlayerController prefab is null!"))
            return;

        PlayerController controller = PhotonNetwork.Instantiate(
            Path.Combine("PhotonPrefabs", "Controllers", _playerControllerPrefab.gameObject.name),
            Vector3.zero,
            Quaternion.identity
        ).GetComponent<PlayerController>();

        if (DebugUtils.CheckForNull(controller,
                $"GameModeBase: Failed to find {_playerControllerPrefab.gameObject.name} prefab in 'Resources/PhotonPrefabs/Controllers' directory!"))
            return;

        _playerControllers.Add(controller);
    }

    protected void DestroyAllPawns()
    {
        foreach (var controller in _playerControllers)
        {
            if (DebugUtils.CheckForNull(controller)) return;
            controller.DestroyPawn();
        }
    }

    protected void InstantiateAllPawns()
    {
        foreach (var controller in _playerControllers)
        {
            if (DebugUtils.CheckForNull(controller)) return;
            controller.CreatePawn();
        }
    }

    protected void EnableAllPlayerControl(bool isEnabled)
    {
        foreach (var playerController in _playerControllers)
        {
            if (DebugUtils.CheckForNull<PlayerController>(playerController,
                    "EnableAllPlayerControl() failed because the controller from list returned null!!",
                    20.0f)) return;
            if (DebugUtils.CheckForNull<Pawn>(playerController.Pawn,
                    "EnableAllPlayerControl() failed because the Pawn from the PlayerController returned null!!",
                    20.0f)) return;
            playerController.Pawn.EnableControl(isEnabled, true);
        }
    }
}