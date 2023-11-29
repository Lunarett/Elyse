using System;
using System.Linq;
using UnityEngine;

public class CharacterMovement : PawnMovementBase
{
    [SerializeField] private float _movementSpeedInAir = 10.0f;
    [SerializeField] private float _airAccelerationSpeed = 25.0f;
    [SerializeField] private float _jumpForce = 9.0f;
    [SerializeField] private float _downForce = 20.0f;
    
    [Space(20.0f)]
    
    [SerializeField] private float _crouchingSharpness = 10.0f;
    [Range(0.0f, 1.0f)] [SerializeField] protected float _crouchMultiplier = 0.5f;
    [Range(1.0f, 2.0f)] [SerializeField] protected float _sprintMultiplier = 2.0f;

    [Header("Pawn Height Properties")]
    [SerializeField] private Transform _eyeTransform;
    [SerializeField] private float _eyeHeightRatio = 0.9f;
    [SerializeField] private float _capsuleHeightStanding = 1.8f;
    [SerializeField] private float _capsuleHeightCrouching = 0.9f;
    
    [Header("Ground Check")]
    [SerializeField] private LayerMask _groundCheckLayers = -1;
    [SerializeField] private float _groundCheckDistance = 0.05f;
    
    private float _lastTimeJumped;
    private float _targetCharacterHeight;

    protected bool _isCrouching;
    protected bool _isSprinting;
    protected bool _isJumping;
    protected bool _hasJumped;
    protected bool _isGrounded;
    protected bool _wasGrounded;
    
    private Vector3 _latestImpactSpeed;

    private const float JUMP_GROUND_PREVENTION_TIME = 0.2f;
    private const float GROUND_CHECK_DIST_IN_AIR = 0.07f;
    
    public float SprintMultiplier => _sprintMultiplier;
    public float CrouchMultiplier => _crouchMultiplier;
    public bool WasGrounded => _wasGrounded;
    public bool IsGrounded => _isGrounded;
    public Vector3 LatestImpactSeed => _latestImpactSpeed;
    
    public event Action<bool> OnStanceChanged;
    public event Action OnJumped;
    public event Action OnLanded;
    
    private void Start()
    {
        _characterController.enableOverlapRecovery = true;
        SetCrouchingState(false, true);
        AdjustCharacterHeight(true);
    }

    protected virtual void Update()
    {
        _wasGrounded = _isGrounded;
        GroundCheck();
        AdjustCharacterHeight(false);
    }

    private void GroundCheck()
    {
        float checkDistance = _isGrounded ? (_characterController.skinWidth + _groundCheckDistance) : GROUND_CHECK_DIST_IN_AIR;

        _isGrounded = false;
        _groundNormal = Vector3.up;

        if (!Physics.CapsuleCast(CalculateCapsuleBottomPosition(),
                CalculateCapsuleTopPosition(_characterController.height),
                _characterController.radius, Vector3.down, out RaycastHit hit, checkDistance,
                _groundCheckLayers, QueryTriggerInteraction.Ignore)) return;
        _groundNormal = hit.normal;

        if (!IsSurfaceWalkable(hit.normal)) return;
        _isGrounded = true;
        AdjustToSurface(hit.distance);
    }

    protected void Move(Vector3 direction, float multiplier)
    {
        if (_isGrounded)
        {
            MovePawn(direction, multiplier);
            HandleJumping();
        }
        else
        {
            MoveInAir(direction, multiplier);
        }

        _latestImpactSpeed = Vector3.zero;
        
        if (!Physics.CapsuleCast(CalculateCapsuleBottomPosition(),
                CalculateCapsuleTopPosition(_characterController.height),
                _characterController.radius, CharacterVelocity.normalized, out RaycastHit hit,
                CharacterVelocity.magnitude * Time.deltaTime, -1, QueryTriggerInteraction.Ignore)) return;
        
        _latestImpactSpeed = CharacterVelocity;
        CharacterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, hit.normal);
    }

    private void MoveInAir(Vector3 direction, float speedMultiplier)
    {
        // Apply air acceleration
        Vector3 acceleration = direction * (_airAccelerationSpeed * Time.deltaTime);
        CharacterVelocity += acceleration;

        // Separate horizontal and vertical components of velocity
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(CharacterVelocity, Vector3.up);
        float verticalVelocity = CharacterVelocity.y;

        // Clamp horizontal velocity and recombine with vertical velocity
        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, _movementSpeedInAir * speedMultiplier);
        CharacterVelocity = horizontalVelocity + Vector3.up * verticalVelocity;

        ApplyDownForce();

        _characterController.Move(CharacterVelocity * Time.deltaTime);
    }
    
    private void ApplyDownForce()
    {
        CharacterVelocity += Vector3.down * (_downForce * Time.deltaTime);
    }
    
    private void HandleJumping()
    {
        if (!CanJump()) return;

        ApplyJumpForce();
        UpdateJumpState();
        OnJumped?.Invoke();
    }
    
    private bool CanJump()
    {
        return Time.time > _lastTimeJumped + JUMP_GROUND_PREVENTION_TIME &&
               _isJumping &&
               SetCrouchingState(false, false);
    }

    private void ApplyJumpForce()
    {
        // Reset vertical velocity and apply jump force
        CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);
        CharacterVelocity += Vector3.up * _jumpForce;
    }

    private void UpdateJumpState()
    {
        _lastTimeJumped = Time.time;
        _hasJumped = true;
        _isGrounded = false;
        _groundNormal = Vector3.up;
    }
    
    private bool IsSurfaceWalkable(Vector3 surfaceNormal)
    {
        return Vector3.Dot(surfaceNormal, transform.up) > 0f &&
               Vector3.Angle(Vector3.up, surfaceNormal) <= _characterController.slopeLimit;
    }

    private void AdjustToSurface(float hitDistance)
    {
        if (!(hitDistance > _characterController.skinWidth)) return;
        _characterController.Move(Vector3.down * hitDistance);
    }
    
    private void AdjustCharacterHeight(bool immediateUpdate)
    {
        if (immediateUpdate)
        {
            SetCharacterHeight(_targetCharacterHeight);
        }
        else if (_characterController.height != _targetCharacterHeight)
        {
            SmoothlyTransitionCharacterHeight();
        }
    }

    private void SetCharacterHeight(float height)
    {
        _characterController.height = height;
        _characterController.center = Vector3.up * (height * 0.5f);
        _eyeTransform.localPosition = Vector3.up * (height * _eyeHeightRatio);
    }

    private void SmoothlyTransitionCharacterHeight()
    {
        _characterController.height = Mathf.Lerp(_characterController.height, _targetCharacterHeight, 
            _crouchingSharpness * Time.deltaTime);
        _characterController.center = Vector3.up * (_characterController.height * 0.5f);
        _eyeTransform.localPosition = Vector3.Lerp(_eyeTransform.localPosition,
            Vector3.up * (_targetCharacterHeight * _eyeHeightRatio),
            _crouchingSharpness * Time.deltaTime);
    }

    
    protected bool SetCrouchingState(bool crouched, bool ignoreObstructions)
    {
        if (crouched)
        {
            _targetCharacterHeight = _capsuleHeightCrouching;
        }
        else
        {
            if (!CanStandUp(ignoreObstructions))
            {
                return false;
            }
            _targetCharacterHeight = _capsuleHeightStanding;
        }

        UpdateCrouchingStatus(crouched);
        return true;
    }
    
    private bool CanStandUp(bool ignoreObstructions)
    {
        if (ignoreObstructions)
        {
            return true;
        }

        Collider[] overlaps = CheckForOverlaps();
        return !overlaps.Any(collider => collider != _characterController);
    }

    private Collider[] CheckForOverlaps()
    {
        Vector3 bottom = CalculateCapsuleBottomPosition();
        Vector3 top = CalculateCapsuleTopPosition(_capsuleHeightStanding);
        return Physics.OverlapCapsule(bottom, top, _characterController.radius, -1, QueryTriggerInteraction.Ignore);
    }

    private void UpdateCrouchingStatus(bool isCrouching)
    {
        OnStanceChanged?.Invoke(isCrouching);
        _isCrouching = isCrouching;
    }
}