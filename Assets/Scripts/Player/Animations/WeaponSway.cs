using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSway : MonoBehaviour
{
    [SerializeField] private Transform weaponTransform;

    [Header("Sway Properties")] 
    [SerializeField] private float mouseSmoother = 10;
    [SerializeField] private float swayAmount = 0.01f;
    [SerializeField] private float maxSwayAmount = 0.1f;
    [SerializeField] private float swaySmooth = 9f;
    [SerializeField] private AnimationCurve swayCurve;

    [Range(0f, 1f)]
    [SerializeField] private float swaySmoothCounteraction = 1f;

    [Header("Rotation")]
    [SerializeField] private float rotationSwayMultiplier = 1f;

    [Header("Position")]
    [SerializeField] private float positionSwayMultiplier = -1f;

    private ElyseCharacter _character;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector2 sway;
    private Vector2 previousMouseDelta;
    private float smoothingFactor = 1f;

    private void Reset()
    {
        Keyframe[] ks = new Keyframe[] { new Keyframe(0, 0, 0, 2), new Keyframe(1, 1) };
        swayCurve = new AnimationCurve(ks);
    }

    private void Awake()
    {
        _character = transform.root.GetComponent<ElyseCharacter>();
    }

    private void Start()
    {
        if (!weaponTransform)
            weaponTransform = transform;
        initialPosition = weaponTransform.localPosition;
        initialRotation = weaponTransform.localRotation;
    }

    private void Update()
    {
        Vector2 currentMouseDelta = _character.PlayerInputManager.GetMouseInput();
        Vector2 smoothedMouseDelta = Vector2.Lerp(previousMouseDelta, currentMouseDelta, smoothingFactor);
        
        float mouseX = (smoothedMouseDelta.x / mouseSmoother) * swayAmount;  // Dividing by 10 as an example
        float mouseY = (smoothedMouseDelta.y / mouseSmoother) * swayAmount;

        sway = Vector2.MoveTowards(sway, Vector2.zero, swayCurve.Evaluate(Time.deltaTime * swaySmoothCounteraction * sway.magnitude * swaySmooth));
        sway = Vector2.ClampMagnitude(new Vector2(mouseX, mouseY) + sway, maxSwayAmount);

        weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, new Vector3(sway.x, sway.y, 0) * positionSwayMultiplier + initialPosition, swayCurve.Evaluate(Time.deltaTime * swaySmooth));
        weaponTransform.localRotation = Quaternion.Slerp(transform.localRotation, initialRotation * Quaternion.Euler(Mathf.Rad2Deg * rotationSwayMultiplier * new Vector3(-sway.y, sway.x, 0)), swayCurve.Evaluate(Time.deltaTime * swaySmooth));
        
        previousMouseDelta = smoothedMouseDelta;
    }
}