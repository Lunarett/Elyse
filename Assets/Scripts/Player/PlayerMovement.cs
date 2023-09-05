using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Plusar.Player
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInputManager))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject m_cameraFollowTarget;

        [Header("Ground Check")]
        [SerializeField] private float m_groundCheckDistance = 0.05f;
        [SerializeField] private LayerMask m_groundCheckLayers = -1;

        [Header("Movement")]
        [SerializeField] private float m_movementSpeed = 10.0f;
        [SerializeField] private float m_speedInAir = 10.0f;
        [SerializeField] private float m_airAccelerationSpeed = 25.0f;
        [SerializeField] private float m_movementSharpness = 15.0f;
        [SerializeField] private float m_jumpForce = 9.0f;
        [SerializeField] private float m_downForce = 20.0f;
        [Space(20.0f)]
        [Range(0.0f, 1.0f)][SerializeField] private float m_crouchMultiplier = 0.5f;
        [Range(1.0f, 2.0f)][SerializeField] private float m_sprintMultiplier = 2.0f;

        [Header("Stance")]
        [SerializeField] private float m_cameraHeightRatio = 0.9f;
        [SerializeField] private float m_capsuleHeightStanding = 1.8f;
        [SerializeField] private float m_capsuleHeightCrouching = 0.9f;
        [SerializeField] private float m_crouchingSharpness = 10.0f;

        [Header("Audio")]
        [SerializeField] private float m_footstepSfxFrequency = 1.0f;
        [SerializeField] private float m_footstepSfxFrequencyWhileSprinting = 1f;
        [SerializeField] private AudioClip m_footstepSfx;
        [SerializeField] private AudioClip m_jumpSfx;

        private CharacterController m_controller;
        private PlayerInputManager m_inputManager;
        private AudioSource m_audioSource;
        private Camera m_weaponCamera;

        private float m_lastTimeJumped;
        private float m_footstepDistanceCounter;
        private float m_targetCharacterHeight;

        private bool m_isCrouching;
        private bool m_isSprinting;
        private bool m_isJumping;
        private bool m_hasJumped;
        private bool m_isGrounded;

        private Vector3 m_movementAxis;
        private Vector3 m_groundNormal;
        private Vector3 m_characterVelocity;
        private Vector3 m_latestImpactSpeed;

        private const float JUMP_GROUND_PREVENTION_TIME = 0.2f;
        private const float GROUND_CHECK_DIST_IN_AIR = 0.07f;

        public float MovementSpeed => m_movementSpeed;
        public float SprintMultiplier => m_sprintMultiplier;
        public float CrouchMultiplier => m_crouchMultiplier;
        public bool IsGrounded => m_isGrounded;
        public Vector3 CharacterVelocity => m_characterVelocity;
        public bool EnableMovement = true;

        public UnityAction<bool> OnStanceChanged;
        public UnityAction OnLanded;
        private static readonly int Crouch1 = Animator.StringToHash("Crouch");

        private void Awake()
        {
            m_controller = GetComponent<CharacterController>();
            m_inputManager = GetComponent<PlayerInputManager>();
            m_audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (!EnableMovement) return;

            m_controller.enableOverlapRecovery = true;
            SetCrouchingState(false, true);
            UpdateCharacterHeight(true);
        }

        private void Update()
        {
            if (!EnableMovement) return;

            Jump(m_inputManager.GetJumpInputDown());
            Crouch(m_inputManager.GetCrouchInputDown());
            Sprint(m_inputManager.GetSprintInputHeld());

            UpdateMovement(m_inputManager.GetMoveInput());
        }

        public void UpdateMovement(Vector2 rawInputAxis)
        {
            GroundCheck();
            _animator.SetBool("Grounded", m_isGrounded);

            UpdateCharacterHeight(false);
            Move(rawInputAxis);

            Vector3 localVelocity = transform.InverseTransformDirection(m_characterVelocity);

            // Update animator parameters based on local velocity
            _animator.SetFloat("Horizontal", localVelocity.x);
            _animator.SetFloat("Vertical", localVelocity.z);
        }

        private void GroundCheck()
        {
            var chosenGroundCheckDistance =
                m_isGrounded ? (m_controller.skinWidth + m_groundCheckDistance) : GROUND_CHECK_DIST_IN_AIR;

            m_isGrounded = false;
            m_groundNormal = Vector3.up;

            if (Time.time <= m_lastTimeJumped + JUMP_GROUND_PREVENTION_TIME) return;

            if (!Physics.CapsuleCast(GetCapsuleBottom(), GetCapsuleTop(m_controller.height),
                    m_controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance,
                    m_groundCheckLayers,
                    QueryTriggerInteraction.Ignore)) return;

            m_groundNormal = hit.normal;

            if (Vector3.Dot(hit.normal, gameObject.transform.up) < 0f &&
                !IsNormalUnderSlopeLimit(m_groundNormal)) return;

            m_isGrounded = true;

            if (hit.distance < m_controller.skinWidth) return;
            m_controller.Move(Vector3.down * hit.distance);
        }

        private void Move(Vector2 rawInputAxis)
        {
            bool isSprinting = m_isSprinting;
            float speedModifier = isSprinting ? m_sprintMultiplier : 1f;
            Vector3 worldSpaceInput = gameObject.transform.TransformVector(new Vector3(rawInputAxis.x, 0, rawInputAxis.y));

            if (isSprinting) isSprinting = SetCrouchingState(false, false);

            if (m_isGrounded)
            {
                // Handle Grounded Movement
                Vector3 targetVelocity = worldSpaceInput * m_movementSpeed * speedModifier;
                if (m_isCrouching) targetVelocity *= m_crouchMultiplier;

                targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, m_groundNormal) *
                                 targetVelocity.magnitude;

                m_characterVelocity = Vector3.Lerp(m_characterVelocity, targetVelocity,
                    m_movementSharpness * Time.deltaTime);

                HandleJumping();
                PlayFootStepSFX(isSprinting);
            }
            else
            {
                // Handle Air Movement
                m_characterVelocity += worldSpaceInput * m_airAccelerationSpeed * Time.deltaTime;
                float verticalVelocity = m_characterVelocity.y;
                Vector3 horizontalVelocity = Vector3.ProjectOnPlane(m_characterVelocity, Vector3.up);
                horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, m_speedInAir * speedModifier);
                m_characterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);
                m_characterVelocity += Vector3.down * m_downForce * Time.deltaTime;
            }

            // Apply Movement Velocity
            Vector3 capsuleBottomBeforeMove = GetCapsuleBottom();
            Vector3 capsuleTopBeforeMove = GetCapsuleTop(m_controller.height);
            m_controller.Move(m_characterVelocity * Time.deltaTime);

            // Handle Obstructions
            m_latestImpactSpeed = Vector3.zero;
            if (!Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, m_controller.radius,
                    m_characterVelocity.normalized, out RaycastHit hit, m_characterVelocity.magnitude * Time.deltaTime,
                    -1,
                    QueryTriggerInteraction.Ignore)) return;

            m_latestImpactSpeed = m_characterVelocity;
            m_characterVelocity = Vector3.ProjectOnPlane(m_characterVelocity, hit.normal);
        }

        private void HandleJumping()
        {
            if (!m_isJumping) return;
            if (!SetCrouchingState(false, false)) return;

            m_characterVelocity = new Vector3(m_characterVelocity.x, 0f, m_characterVelocity.z);
            m_characterVelocity += Vector3.up * m_jumpForce;
            if (m_jumpSfx) m_audioSource.PlayOneShot(m_jumpSfx);

            m_lastTimeJumped = Time.time;
            m_hasJumped = true;

            m_isGrounded = false;
            m_groundNormal = Vector3.up;
        }

        private void HandleLanding(bool wasGrounded)
        {
            if (!m_isGrounded || wasGrounded) return;
            OnLanded?.Invoke();
        }

        private void PlayFootStepSFX(bool isSprinting)
        {
            if (m_footstepSfx == null)
            {
                return;
            }

            float chosenFootstepSfxFrequency =
                (isSprinting ? m_footstepSfxFrequencyWhileSprinting : m_footstepSfxFrequency);

            if (m_footstepDistanceCounter >= 1f / chosenFootstepSfxFrequency)
            {
                m_footstepDistanceCounter = 0f;
                m_audioSource.PlayOneShot(m_footstepSfx);
            }

            m_footstepDistanceCounter += m_characterVelocity.magnitude * Time.deltaTime;
        }

        private void UpdateCharacterHeight(bool force)
        {
            // Update height instantly
            if (force)
            {
                m_controller.height = m_targetCharacterHeight;
                m_controller.center = Vector3.up * m_controller.height * 0.5f;
                m_cameraFollowTarget.transform.localPosition = Vector3.up * m_targetCharacterHeight * m_cameraHeightRatio;
            }
            // Update smooth height
            else if (m_controller.height != m_targetCharacterHeight)
            {
                // resize the capsule and adjust camera position
                m_controller.height = Mathf.Lerp(m_controller.height, m_targetCharacterHeight,
                    m_crouchingSharpness * Time.deltaTime);
                m_controller.center = Vector3.up * m_controller.height * 0.5f;
                m_cameraFollowTarget.transform.localPosition = Vector3.Lerp(m_cameraFollowTarget.transform.localPosition,
                    Vector3.up * m_targetCharacterHeight * m_cameraHeightRatio, m_crouchingSharpness * Time.deltaTime);
            }
        }

        private bool SetCrouchingState(bool crouched, bool ignoreObstructions)
        {
            // set appropriate heights
            if (crouched)
            {
                m_targetCharacterHeight = m_capsuleHeightCrouching;
            }
            else
            {
                // Detect obstructions
                if (!ignoreObstructions)
                {
                    Collider[] standingOverlaps = Physics.OverlapCapsule(
                        GetCapsuleBottom(),
                        GetCapsuleTop(m_capsuleHeightStanding),
                        m_controller.radius,
                        -1,
                        QueryTriggerInteraction.Ignore);
                    if (standingOverlaps.Any(c => c != m_controller))
                    {
                        return false;
                    }
                }

                m_targetCharacterHeight = m_capsuleHeightStanding;
            }

            OnStanceChanged?.Invoke(crouched);
            m_isCrouching = crouched;
            return true;
        }

        private bool IsNormalUnderSlopeLimit(Vector3 normal)
        {
            return Vector3.Angle(gameObject.transform.up, normal) <= m_controller.slopeLimit;
        }

        private Vector3 GetCapsuleBottom()
        {
            return gameObject.transform.position + (gameObject.transform.up * m_controller.radius);
        }

        private Vector3 GetCapsuleTop(float height)
        {
            return gameObject.transform.position + (gameObject.transform.up * (height - m_controller.radius));
        }

        private Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
        {
            Vector3 directionRight = Vector3.Cross(direction, gameObject.transform.up);
            return Vector3.Cross(slopeNormal, directionRight).normalized;
        }

        public void Jump(bool isJumping)
        {
            m_isJumping = isJumping;
            _animator.SetBool("Jump", m_isJumping);
        }

        public void Crouch(bool isCrouching)
        {
            m_isCrouching = isCrouching;
            SetCrouchingState(m_isCrouching, false);
            _animator.SetBool(Crouch1, m_isCrouching);
        }

        public void Sprint(bool isSprinting)
        {
            m_isSprinting = isSprinting;
        }
    }
}
