using System;
using Pulsar.Debug;
using UnityEngine;

public class SpectatorMovement : PawnMovementBase
{
    [SerializeField] private Camera _targetCamera;
    private SpectatorInputManager _input;

    private void Start()
    {
        _input = GetComponent<SpectatorInputManager>();
        _groundNormal = Vector3.up;
    }
    
    private void Update()
    {
        Move(_input.GetMoveInput(), _input.GetElevationInput());
    }

    public void Move(Vector2 inputAxis, float elevation)
    {
        Vector3 direction = _targetCamera.transform.TransformDirection(new Vector3(inputAxis.x, 0, inputAxis.y));
        direction = new Vector3(direction.x, elevation, direction.z);
        direction.Normalize();
        Vector3 targetVelocity = direction * _movementSpeed;
        CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity, _movementSharpness * Time.deltaTime);
        _characterController.Move(CharacterVelocity * Time.deltaTime);
    }
}
