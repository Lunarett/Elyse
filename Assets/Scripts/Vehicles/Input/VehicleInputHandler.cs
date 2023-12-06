using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleInputHandler : InputManager
{
    public float GetThrottleInput()
    {
        return _enableMouseInput && CanProcessInput() ? GetInputActionValue<float>("Throttle") : 0;
    }
    
    public float GetSteerInput()
    {
        return _enableMouseInput && CanProcessInput() ? GetInputActionValue<float>("Steer") : 0;
    }
    
    public bool GetHandbrakeInputHeld()
    {
        return _enableMoveInput && CheckInputActionPhase("Handbrake", InputActionPhase.Performed);
    }
}