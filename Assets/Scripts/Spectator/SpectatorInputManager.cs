using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorInputManager : InputManager
{
    public Vector2 GetMoveInput()
    {
        return _enableMoveInput && CanProcessInput() ? GetInputActionValue<Vector2>("Move") : Vector2.zero;
    }

    public Vector2 GetMouseInput()
    {
        return _enableMouseInput && CanProcessInput() ? GetInputActionValue<Vector2>("Look") : Vector2.zero;
    }
    
    public float GetElevationInput()
    {
        return _enableMouseInput && CanProcessInput() ? GetInputActionValue<float>("Elevate") : 0;
    }
}
