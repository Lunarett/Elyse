using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : InputManager
{
    private bool _fireInputWasHeld;
    private bool _jumpInputWasHeld;
    private bool _viewInputWasHeld;
    private bool _crouchInputWasHeld;
    private bool _pauseInputWasHeld;
    private bool _interactInputWasHeld;

    private void LateUpdate()
    {
        _fireInputWasHeld = GetFireInputHeld();
        _jumpInputWasHeld = GetJumpInputHeld();
        _viewInputWasHeld = GetViewInputHeld();
        _crouchInputWasHeld = GetJumpInputHeld();
        _pauseInputWasHeld = GetPauseInputHeld();
        _interactInputWasHeld = GetInteractInputHeld();
    }

    public Vector3 GetMoveInputVector3()
    {
        if (!_enableMoveInput && !CanProcessInput()) return Vector3.zero;
        Vector2 inputAxis = GetInputActionValue<Vector2>("Movement");
        return new Vector3(inputAxis.x, 0, inputAxis.y);
    }

    public Vector2 GetMoveInput()
    {
        return _enableMoveInput && CanProcessInput() ? GetInputActionValue<Vector2>("Movement") : Vector2.zero;
    }

    public Vector2 GetMouseInput()
    {
        return _enableMouseInput && CanProcessInput() ? GetInputActionValue<Vector2>("Look") : Vector2.zero;
    }

    public bool GetJumpInputDown()
    {
        return _enableMoveInput && CanProcessInput() && (GetJumpInputHeld() && !_jumpInputWasHeld);
    }


    public bool GetJumpInputHeld()
    {
        return _enableMoveInput && CheckInputActionPhase("Jump", InputActionPhase.Performed);
    }

    public bool GetJumpInputReleased()
    {
        return _enableMoveInput && CheckInputActionPhase("Jump", InputActionPhase.Canceled);
    }

    public bool GetFireInputDown()
    {
        return _enableMoveInput && CanProcessInput() && (GetFireInputHeld() && !_fireInputWasHeld);
    }

    public bool GetFireInputReleased()
    {
        return _enableMoveInput && CanProcessInput() && !GetFireInputHeld() && _fireInputWasHeld;
    }

    public bool GetFireInputHeld()
    {
        return _enableMoveInput && CheckInputActionPhase("Fire", InputActionPhase.Performed);
    }

    public bool GetAimInputHeld()
    {
        return _enableMoveInput && CheckInputActionPhase("Aim", InputActionPhase.Performed);
    }

    public bool GetSprintInputHeld()
    {
        return _enableMoveInput && CheckInputActionPhase("Sprint", InputActionPhase.Performed);
    }

    public bool GetCrouchInputDown()
    {
        return _enableMoveInput && CanProcessInput() && (GetCrouchInputHeld() && !_crouchInputWasHeld);
    }

    public bool GetCrouchInputHeld()
    {
        return _enableMoveInput && CheckInputActionPhase("Crouch", InputActionPhase.Performed);
    }

    public bool GetCrouchInputReleased()
    {
        return _enableMoveInput && CheckInputActionPhase("Crouch", InputActionPhase.Canceled);
    }

    public bool GetViewInputDown()
    {
        return _enableMoveInput && CanProcessInput() && (GetViewInputHeld() && !_viewInputWasHeld);
    }

    public bool GetViewInputHeld()
    {
        return _enableMoveInput && CheckInputActionPhase("View", InputActionPhase.Performed);
    }

    public bool GetFlyInputHeld()
    {
        return _enableMoveInput && CheckInputActionPhase("Fly", InputActionPhase.Performed);
    }
    
    private bool GetPauseInputHeld()
    {
        return CheckInputActionPhase("Pause", InputActionPhase.Performed);
    }
    
    public bool GetPauseInputDown()
    {
        return CanProcessInput() && (GetPauseInputHeld() && !_pauseInputWasHeld);
    }

    public bool GetInteractInputDown()
    {
        return _enableMoveInput && CanProcessInput() && (GetInteractInputHeld() && !_interactInputWasHeld);
    }

    public bool GetInteractInputHeld()
    {
        return _enableMoveInput && CheckInputActionPhase("Interact", InputActionPhase.Performed);
    }

    public int GetSwitchWeaponInput()
    {
        if (!_enableMoveInput) return 0;
        if (!CanProcessInput()) return 0;
        float scrollVal = GetInputActionValue<float>("Switch");

        return scrollVal switch
        {
            > 0 => -1,
            < 0 => 1,
            _ => 0
        };
    }

    public int GetSelectWeaponInput()
    {
        if (!_enableMoveInput) return 0;
        if (!CanProcessInput()) return 0;

        for (int i = 1; i <= 4; i++)
        {
            if (CheckInputActionPhase($"Slot{i}", InputActionPhase.Performed)) return i;
        }

        return 0;
    }
}