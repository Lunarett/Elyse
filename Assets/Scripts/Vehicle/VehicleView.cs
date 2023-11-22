using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleView : MonoBehaviour
{
    [SerializeField] private Transform _cameraFollowTarget;
    [Range(0.0f, 0.5f)] [SerializeField] private float _lookSpeed = 0.1f;
    [SerializeField] private Vector2 _pitchClamp = new Vector2(-89.0f, 89.0f);

    private float _CameraVerticalAngle;
    private AutomobileInputManager _input;

    private void Awake()
    {
        _input = GetComponent<AutomobileInputManager>();
    }

    private void Update()
    {
        MouseYAWRotation(_cameraFollowTarget, _input.GetMouseLook());
        MousePitchRotation(_input.GetMouseLook());
    }

    public void MouseYAWRotation(Transform targetTransform, Vector2 mouseInputAxis)
    {
        targetTransform.Rotate
        (
            new Vector3(0f, (mouseInputAxis.x * _lookSpeed),
                0f),
            Space.Self
        );
    }

    public void MousePitchRotation(Vector2 mouseInputAxis)
    {
        _CameraVerticalAngle += mouseInputAxis.y * _lookSpeed;
        _CameraVerticalAngle = Mathf.Clamp(_CameraVerticalAngle, _pitchClamp.x, _pitchClamp.y);
        _cameraFollowTarget.transform.localEulerAngles = new Vector3(_CameraVerticalAngle, 0, 0);
    }
}
