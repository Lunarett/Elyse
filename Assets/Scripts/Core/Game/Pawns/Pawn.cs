using System;
using Pulsar.Debug;
using UnityEngine;

public abstract class Pawn : MonoBehaviour
{
    public Controller Owner { get; set; }
    protected PawnCameraController _pawnCameraController;
    public PawnCameraController PawnCameraController => _pawnCameraController;
    
    protected virtual void Awake()
    {
        _pawnCameraController = GetComponent<PawnCameraController>();
    }

    protected virtual void Start()
    {
    }

    public virtual void SetOwner(Controller newOwner)
    {
        Owner = newOwner;
    }

    public virtual void RemoveOwner()
    {
        Owner = null;
    }
    
    public void ShowMouseCursor(bool isVisible)
    {
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public virtual void EnableControl(bool isEnabled, bool ignoreMouse = false)
    {
    }
}
