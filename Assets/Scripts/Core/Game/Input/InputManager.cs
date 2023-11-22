using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    private PlayerInput _playerInput;
    private PhotonView _photonView;
    private bool _isInputActive = true;
    
    protected bool _enableMoveInput = true;
    protected bool _enableMouseInput = true;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _photonView = GetComponent<PhotonView>();
    }

    public void SetInputActive(bool isActive)
    {
        _isInputActive = isActive;
    }

    protected bool CanProcessInput()
    {
        return _isInputActive && _photonView.IsMine;
    }

    protected bool CheckInputActionPhase(string actionName, InputActionPhase phase)
    {
        return CanProcessInput() && _playerInput.actions[actionName].phase == phase;
    }

    protected T GetInputActionValue<T>(string actionName) where T : struct
    {
        return _playerInput.actions[actionName].ReadValue<T>();
    }

    public void EnableControl(bool isEnabled, bool ignoreMouse = false)
    {
        _enableMoveInput = isEnabled;
        if (!ignoreMouse) _enableMouseInput = isEnabled;
    }
}