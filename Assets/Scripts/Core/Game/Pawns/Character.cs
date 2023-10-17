using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerView))]
public abstract class Character : Pawn
{
    protected PlayerMovement _playerMovement;
    protected PlayerView _playerView;
    
    public PlayerMovement PlayerMovement => _playerMovement;
    public PlayerView PlayerView => _playerView;

    protected virtual void Awake()
    {
        base.Awake();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerView = GetComponent<PlayerView>();
    }

    public void EnableMovement(bool isEnabled)
    {
        _playerMovement.EnableMovement = isEnabled;
    }

    public void EnableView(bool isEnabled)
    {
        _playerView.EnableView = isEnabled;
    }
}
