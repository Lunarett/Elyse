using UnityEngine;
using UnityEngine.InputSystem;

public class AutomobileInputManager : InputManager
{
    public Vector2 GetMouseLook()
    {
        return _enableMouseInput && CanProcessInput() ? GetInputActionValue<Vector2>("Look") : Vector2.zero;
    }
    
    public float GetThrottleAxis()
    {
        return _enableMoveInput && CanProcessInput() ? GetInputActionValue<float>("Throttle") : 0;
    }

    public float GetSteerAxis()
    {
        return _enableMoveInput && CanProcessInput() ? GetInputActionValue<float>("Steer") : 0; 
    }

    public bool GetHandbrakeInputHeld()
    {
        return _enableMoveInput && CheckInputActionPhase("Handbrake", InputActionPhase.Performed);
    }
}
