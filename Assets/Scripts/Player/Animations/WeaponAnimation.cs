using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plusar.Player;
using Photon.Realtime;

public class WeaponAnimation : MonoBehaviour
{
    [Header("Sway")]
    [SerializeField] private float _step = 0.01f;
    [SerializeField] private float _maxStepDistance = 0.06f;

    [Header("Sway Rotation")]
    [SerializeField] private float _rotationStep = 4f;
    [SerializeField] private float _maxRotationStep = 5f;
    [SerializeField] private float _smooth = 10f;

    [Header("Bobbing")]
    [SerializeField] private float _speedCurve;
    [SerializeField] private Vector3 _travelLimit = Vector3.one * 0.025f;
    [SerializeField] private Vector3 _bobLimit = Vector3.one * 0.01f;
    [SerializeField] private float _bobExaggeration;

    [Header("Bob Rotation")]
    [SerializeField] private Vector3 _multiplier;


    // Private Member Variables
    private Vector3 _swayPos;
    private Vector3 _swayEulerRot;
    private Vector3 _bobPosition;
    private Vector3 _bobEulerRotation;

    private Vector2 _walkInput;
    private Vector2 _lookInput;

    private float _smoothRot = 12f;
    private float CurveSin => Mathf.Sin(_speedCurve);
    private float CurveCos => Mathf.Cos(_speedCurve);

    public Plusar.Player.PlayerController PlayerController { get; set; }
    private Vector2 _previousWalkInput;
    private float _inputSmoothing = 0.1f;

    private void Update()
    {
        if (!PlayerController) return;

        GetInput();

        Sway();
        SwayRotation();
        BobOffset();
        BobRotation();

        CompositePositionRotation();
    }

    private void GetInput()
    {
        Vector2 rawWalkInput = PlayerController.InputManager.GetMoveInput();
        Vector2 rawLookInput = PlayerController.InputManager.GetMouseInput();

        // Normalize and smooth the walk input
        _walkInput = Vector2.Lerp(_previousWalkInput, Vector2.ClampMagnitude(rawWalkInput, 1f), _inputSmoothing);
        _previousWalkInput = _walkInput;

        // Use the raw look input directly (or apply smoothing if needed)
        _lookInput = rawLookInput;
    }

    private void Sway()
    {
        Vector3 invertLook = _lookInput * -_step;
        invertLook.x = Mathf.Clamp(invertLook.x, -_maxStepDistance, _maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -_maxStepDistance, _maxStepDistance);

        _swayPos = invertLook;
    }

    private void SwayRotation()
    {
        Vector2 invertLook = _lookInput * -_rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -_maxRotationStep, _maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -_maxRotationStep, _maxRotationStep);
        _swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }

    private void CompositePositionRotation()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, _swayPos + _bobPosition, Time.deltaTime * _smooth);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(_swayEulerRot) * Quaternion.Euler(_bobEulerRotation), Time.deltaTime * _smoothRot);
    }

    private void BobOffset()
    {
        _speedCurve += Time.deltaTime * (PlayerController.PlayerMovement.IsGrounded ? (_walkInput.x + _walkInput.y) * _bobExaggeration : 1f) + 0.01f;

        _bobPosition.x = (CurveCos * _bobLimit.x * (PlayerController.PlayerMovement.IsGrounded ? 1 : 0)) - (_walkInput.x * _travelLimit.x);
        _bobPosition.y = (CurveSin * _bobLimit.y) - (_walkInput.y * _travelLimit.y);
        _bobPosition.z = -(_walkInput.y * _travelLimit.z);
    }

    private void BobRotation()
    {
        _bobEulerRotation.x = (_walkInput != Vector2.zero ? _multiplier.x * (Mathf.Sin(2 * _speedCurve)) : _multiplier.x * (Mathf.Sin(2 * _speedCurve) / 2));
        _bobEulerRotation.y = (_walkInput != Vector2.zero ? _multiplier.y * CurveCos : 0);
        _bobEulerRotation.z = (_walkInput != Vector2.zero ? _multiplier.z * CurveCos * _walkInput.x : 0);
    }
}