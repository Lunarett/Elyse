using System.Collections.Generic;
using UnityEngine;
using Pulsar.Debug;

public class GameModeBase : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private PlayerController _playerControllerPrefab;
    [SerializeField] private Pawn _playerPawn;
    [SerializeField] private Pawn _spectatorPawn;

    protected List<PlayerController> _playerControllers = new List<PlayerController>();

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        InstantiatePlayerController();
    }

    protected void InstantiatePlayerController()
    {
        if(DebugUtils.CheckForNull<PlayerController>(_playerControllerPrefab, "GameModeBase: Controller prefab missing!")) return;
        PlayerController controller = Instantiate(_playerControllerPrefab, Vector3.zero, Quaternion.identity);
        controller.CreatePawn(_playerPawn);
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
            controller.CreatePawn(_playerPawn);
        }
    }

    protected void EnableAllPlayerControl(bool isEnabled)
    {
        foreach (var playerController in _playerControllers)
        {
            if (DebugUtils.CheckForNull<PlayerController>(playerController,
                    "EnableAllPlayerControl() failed because the controller from list returned null!!",
                    20.0f)) return;
            if (DebugUtils.CheckForNull<Pawn>(playerController.ControlledPawn,
                    "EnableAllPlayerControl() failed because the Pawn from the PlayerController returned null!!",
                    20.0f)) return;
            playerController.ControlledPawn.EnableControl(isEnabled, true);
        }
    }
}