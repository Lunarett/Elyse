using Pulsar.Debug;
using UnityEngine;

public abstract class Pawn : MonoBehaviour
{
    protected InputManager _inputManager;
    public Controller Owner { get; set; }
    
    protected virtual void Awake()
    {
        _inputManager = GetComponent<InputManager>();
        if (DebugUtils.CheckForNull<InputManager>(_inputManager, "Pawn: PhotonView is missing!")) return;
    }
    
    public void ShowMouseCursor(bool isVisible)
    {
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void EnableControl(bool isEnabled, bool ignoreMouse = false)
    {
        _inputManager.EnableControl(isEnabled, ignoreMouse);
    }
}
