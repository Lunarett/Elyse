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

    [Header("Recoil Settings")]
    [SerializeField] private float recoilRecoverySpeed = 1f; // Speed at which recoil is recovered

    private SpringArm _springArm;
    private Transform _targetTransform;
    private float cameraVerticalAngle;
    private float rotationMultiplier = 1f;
    private float recoilOffset = 0f; // Additional recoil offset to be applied to pitch

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

        // Gradually reduce recoil offset to recover back to the original position
        if (recoilOffset != 0f)
        {
            recoilOffset = Mathf.MoveTowards(recoilOffset, 0f, recoilRecoverySpeed * Time.deltaTime);
        }
    }

    public void SetRotationMultiplier(float multiplier)
    {
        rotationMultiplier = multiplier;
    }

    public void AddRecoilOffset(float offset)
    {
        recoilOffset += offset;
        recoilOffset = Mathf.Clamp(recoilOffset, -pitchClamp.y, pitchClamp.y);
    }

    private void HandleMouseYawRotation(Vector2 mouseDelta)
    {
        _targetTransform.Rotate(0f, mouseDelta.x * lookSpeed * rotationMultiplier * Time.deltaTime, 0f, Space.Self);
    }

    private void HandleMousePitchRotation(Vector2 mouseDelta)
    {
        cameraVerticalAngle += mouseDelta.y * lookSpeed * rotationMultiplier * Time.deltaTime;
        // Apply recoilOffset when calculating the new vertical angle
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle + recoilOffset, pitchClamp.x, pitchClamp.y);
        _camera.transform.localEulerAngles = new Vector3(-cameraVerticalAngle, 0, 0);
    }
}