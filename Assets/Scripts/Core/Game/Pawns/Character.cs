using System.Collections;
using System.Collections.Generic;
using Pulsar.Debug;
using UnityEngine;

public abstract class Character : Pawn
{
    protected PlayerMovement _playerMovement;
    protected InputManager _inputManager;
    
    public PlayerMovement PlayerMovement => _playerMovement;

    protected virtual void Awake()
    {
        base.Awake();
        _inputManager = GetComponent<InputManager>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    protected override void Start()
    {
        base.Start();
        if (Owner == null) _inputManager.SetInputActive(false);
    }

    public void EnableInput(bool isEnabled, bool ignoreMouse)
    {
        _inputManager.EnableControl(isEnabled, ignoreMouse);
    }

    public override void SetOwner(Controller newOwner)
    {
        base.SetOwner(newOwner);
        _inputManager.SetInputActive(true);
    }
    
    public override void RemoveOwner()
    {
        _inputManager.SetInputActive(false);
    }
}