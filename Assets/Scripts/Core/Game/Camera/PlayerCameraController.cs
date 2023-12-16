using System;
using Pulsar.Debug;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Camera Control Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool _useRootYawRotation = true;
    [SerializeField] private bool _ignoreZRoll;
    [Range(0.0f, 0.5f)] [SerializeField] private float lookSpeed = 0.1f;
    [SerializeField] private Vector2 pitchClamp = new Vector2(-89.0f, 89.0f);

    private SpringArm _springArm;
    private Transform _targetTransform;
    
    private float cameraVerticalAngle;
    private float rotationMultiplier = 1f;

    public SpringArm SpringArm => _springArm;

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
        HandleMouseYAWRotation(mouseDelta);
        HandleMousePitchRotation(mouseDelta);
    }

    public void SetRotationMultiplier(float multiplier)
    {
        rotationMultiplier = multiplier;
    }

    private void HandleMouseYAWRotation(Vector2 mouseDelta)
    {
        _targetTransform.Rotate(0f, mouseDelta.x * lookSpeed * rotationMultiplier * Time.deltaTime, 0f, Space.Self);
    }

    private void HandleMousePitchRotation(Vector2 mouseDelta)
    {
        cameraVerticalAngle += mouseDelta.y * lookSpeed * rotationMultiplier * Time.deltaTime;
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, pitchClamp.x, pitchClamp.y);
        cameraTransform.localEulerAngles = new Vector3(-cameraVerticalAngle, 0, 0);
    }
}