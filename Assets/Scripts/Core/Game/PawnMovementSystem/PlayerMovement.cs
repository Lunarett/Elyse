using System;
using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerMovement : CharacterMovement
{
    [Header("SFX")] 
    [SerializeField] private EventReference footstepEvent;
    [SerializeField] private EventReference jumpEvent;
    
    private PlayerInputManager _input;
    private float footstepFrequency;
    private float footstepTimer;
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

        if (_isGrounded)
        {
            UpdateSpeedMultiplier();
            UpdateFootstepTimer();
        }

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
            footstepFrequency = 0.4f;
        }
        else if (_isSprinting && !_isCrouching)
        {
            _speedMultiplier = _sprintMultiplier;
            footstepFrequency = 0.25f;
        }
        else
        {
            _speedMultiplier = 1.0f;
            footstepFrequency = 0.3f;
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
    
    private void UpdateFootstepTimer()
    {
        if (_input.GetMoveInput().sqrMagnitude > 0.1f)
        {
            footstepTimer -= Time.deltaTime;
            if (!(footstepTimer <= 0)) return;
            PlayFootstepSound();
            footstepTimer = footstepFrequency;
        }
        else
        {
            footstepTimer = 0;
        }
    }

    private void PlayFootstepSound()
    {
        RuntimeManager.PlayOneShot(footstepEvent, transform.position);
    }
}