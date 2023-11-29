using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerMovement : CharacterMovement
{
    private PlayerInputManager _input;
    private float _speedMultiplier = 1.0f;

    protected override void Awake()
    {
        base.Awake();
        _input = GetComponent<PlayerInputManager>();
    }

    protected override void Update()
    {
        Jump(_input.GetJumpInputDown());
        Crouch(_input.GetCrouchInputDown());
        Sprint(_input.GetSprintInputHeld());
        
        if (_isGrounded) UpdateSpeedMultiplier();
        
        base.Update();
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector2 inputAxis = _input.GetMoveInput();
        Vector3 movementDirection = transform.TransformVector(new Vector3(inputAxis.x, 0, inputAxis.y));
        Move(movementDirection, _speedMultiplier);
    }

    private void UpdateSpeedMultiplier()
    {
        if (_isCrouching && !_isSprinting)
        {
            _speedMultiplier = _crouchMultiplier;
        }
        else if (_isSprinting && !_isCrouching)
        {
            _speedMultiplier = _sprintMultiplier;
        }
        else
        {
            _speedMultiplier = 1.0f;
        }
    }
    
    public void Jump(bool isJumping)
    {
        _isJumping = isJumping;
    }

    public void Crouch(bool isCrouching)
    {
        _isCrouching = isCrouching; 
        SetCrouchingState(_isCrouching, false);
    }

    public void Sprint(bool isSprinting)
    {
        _isSprinting = isSprinting;
    }
}
