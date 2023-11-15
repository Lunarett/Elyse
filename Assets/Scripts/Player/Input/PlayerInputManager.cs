using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : InputManager
{
    private bool _fireInputWasHeld;
    private bool _jumpInputWasHeld;
    private bool _viewInputWasHeld;
    private bool _crouchInputWasHeld;

    private void LateUpdate()
    {
        _fireInputWasHeld = GetFireInputHeld();
        _jumpInputWasHeld = GetJumpInputHeld();
        _viewInputWasHeld = GetViewInputHeld();
        _crouchInputWasHeld = GetJumpInputHeld();
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
        return CheckInputActionPhase("Jump", InputActionPhase.Performed);
    }

    public bool GetJumpInputReleased()
    {
        return CheckInputActionPhase("Jump", InputActionPhase.Canceled);
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
        return CheckInputActionPhase("Fire", InputActionPhase.Performed);
    }

    public bool GetAimInputHeld()
    {
        return CheckInputActionPhase("Aim", InputActionPhase.Performed);
    }

    public bool GetSprintInputHeld()
    {
        return CheckInputActionPhase("Sprint", InputActionPhase.Performed);
    }

    public bool GetCrouchInputDown()
    {
        return CanProcessInput() && (GetCrouchInputHeld() && !_crouchInputWasHeld);
    }

    public bool GetCrouchInputHeld()
    {
        return CheckInputActionPhase("Crouch", InputActionPhase.Performed);
    }

    public bool GetCrouchInputReleased()
    {
        return CheckInputActionPhase("Crouch", InputActionPhase.Canceled);
    }

    public bool GetViewInputDown()
    {
        return CanProcessInput() && (GetViewInputHeld() && !_viewInputWasHeld);
    }

    public bool GetViewInputHeld()
    {
        return CheckInputActionPhase("View", InputActionPhase.Performed);
    }

    public bool GetFlyInputHeld()
    {
        return CheckInputActionPhase("Fly", InputActionPhase.Performed);
    }

    public int GetSwitchWeaponInput()
    {
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
        if (!CanProcessInput()) return 0;

        for (int i = 1; i <= 4; i++)
        {
            if (CheckInputActionPhase($"Slot{i}", InputActionPhase.Performed)) return i;
        }

        return 0;
    }
}