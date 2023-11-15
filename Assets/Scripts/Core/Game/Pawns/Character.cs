using System.Collections;
using System.Collections.Generic;
using Pulsar.Debug;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerView))]
public abstract class Character : Pawn
{
    protected PlayerMovement _playerMovement;
    protected PlayerView _playerView;
    protected InputManager _inputManager;
    
    public PlayerMovement PlayerMovement => _playerMovement;
    public PlayerView PlayerView => _playerView;

    protected virtual void Awake()
    {
        base.Awake();
        _inputManager = GetComponent<InputManager>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerView = GetComponent<PlayerView>();
    }

    public void EnableMovement(bool isEnabled, bool ignoreMouse)
    {
        _inputManager.EnableControl(isEnabled, ignoreMouse);
    }

    public void EnableView(bool isEnabled)
    {
        _playerView.EnableView = isEnabled;
    }
}
