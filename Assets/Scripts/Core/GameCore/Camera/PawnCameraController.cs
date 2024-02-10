using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PawnCameraController : MonoBehaviour
{
    [Header("Camera Control Settings")]
    [SerializeField] private Camera _camera;
    [Space]
    [SerializeField] private bool _useRootYawRotation = true;
    [SerializeField] private bool _ignoreZRoll;
    [Range(0.0f, 0.5f)] [SerializeField] private float lookSpeed = 0.1f;
    [SerializeField] private Vector2 pitchClamp = new Vector2(-89.0f, 89.0f);

    [Header("Directional Force Settings")]
    [SerializeField] private float forceRecoverySpeed = 1f; // Speed at which the camera recovers from the applied force

    private SpringArm _springArm;
    private Transform _targetTransform;
    private float cameraVerticalAngle;
    private float rotationMultiplier = 1f;
    private Vector3 forceDirection = Vector3.zero; // Direction of the applied force

    public SpringArm SpringArm => _springArm;
    public Camera MainCamera => _camera;

    private void Awake()
    {
        _springArm = GetComponentInChildren<SpringArm>();
        lookSpeed *= 20;
    }

    private void Start()
    {
        if (_ignoreZRoll) _springArm.transform.SetParent(null);
        _targetTransform = _useRootYawRotation ? transform : _springArm.transform;
    }

    private void Update()
    {
        if (_ignoreZRoll) _springArm.transform.position = transform.position;
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        HandleMouseYawRotation(mouseDelta);
        HandleMousePitchRotation(mouseDelta);

        // Gradually reduce directional force to recover back to the original position
        if (forceDirection != Vector3.zero)
        {
            forceDirection = Vector3.MoveTowards(forceDirection, Vector3.zero, forceRecoverySpeed * Time.deltaTime);
            AddDirectionalForce(forceDirection); // Reapply to update camera based on reduced force
        }
    }

    public void SetRotationMultiplier(float multiplier)
    {
        rotationMultiplier = multiplier;
    }

    public void AddDirectionalForce(Vector3 direction)
    {
        // Assuming direction.x affects yaw, direction.y affects pitch, direction.z is ignored or could affect roll
        _targetTransform.Rotate(0f, direction.x * rotationMultiplier, 0f, Space.Self);
        
        cameraVerticalAngle += direction.y * rotationMultiplier;
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, pitchClamp.x, pitchClamp.y);
        _camera.transform.localEulerAngles = new Vector3(-cameraVerticalAngle, 0, 0);

        // Store the directional force to be reduced over time
        forceDirection = direction;
    }

    private void HandleMouseYawRotation(Vector2 mouseDelta)
    {
        _targetTransform.Rotate(0f, mouseDelta.x * lookSpeed * rotationMultiplier * Time.deltaTime, 0f, Space.Self);
    }

    private void HandleMousePitchRotation(Vector2 mouseDelta)
    {
        cameraVerticalAngle += mouseDelta.y * lookSpeed * rotationMultiplier * Time.deltaTime;
        // Clamp without applying the force here as it's handled in AddDirectionalForce now
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, pitchClamp.x, pitchClamp.y);
        _camera.transform.localEulerAngles = new Vector3(-cameraVerticalAngle, 0, 0);
    }
}