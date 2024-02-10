using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    protected PlayerInput _playerInput;
    private bool _isInputActive = true;

    protected bool _enableMoveInput = true;
    protected bool _enableMouseInput = true;

    protected virtual void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    protected virtual void Update()
    {
        if (Keyboard.current[Key.Backquote].wasPressedThisFrame)
        {
            ToggleInputAndCursor();
        }
    }

    private void ToggleInputAndCursor()
    {
        SetInputActive(!_isInputActive);
        _enableMoveInput = !_enableMoveInput;
        _enableMouseInput = !_enableMouseInput;

        // Toggle mouse cursor visibility and lock state
        Cursor.visible = !_enableMouseInput;
        Cursor.lockState = _enableMouseInput ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void SetInputActive(bool isActive)
    {
        _isInputActive = isActive;
        _playerInput.enabled = isActive;
    }

    protected bool CanProcessInput()
    {
        return _isInputActive && _playerInput.camera.isActiveAndEnabled;
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