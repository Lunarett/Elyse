using System.Collections;
using System.Collections.Generic;
using Pulsar.Debug;
using UnityEngine;

public abstract class Character : Pawn
{
    protected PlayerMovement _playerMovement;
    protected PlayerCameraController _cameraController;
    
    public PlayerMovement DeprecatedPlayerMovement => _playerMovement;
    public PlayerCameraController CameraController => _cameraController;

    protected virtual void Awake()
    {
        base.Awake();
        _playerMovement = GetComponent<PlayerMovement>();
        _cameraController = GetComponent<PlayerCameraController>();
    }

    public void EnableInput(bool isEnabled, bool ignoreMouse)
    {
        _inputManager.EnableControl(isEnabled, ignoreMouse);
    }
}