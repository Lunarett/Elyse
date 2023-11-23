using System;
using UnityEngine;

public enum EAxel
{
    Front,
    Rear
}

public enum EDriveMode
{
    FWD,
    RWD,
    AWD
}

[System.Serializable]
public struct Wheel
{
    public string name;
    public GameObject meshObject;
    public WheelCollider collider;
    public EAxel axel;
}

[RequireComponent(typeof(Rigidbody))]
public class Automobile : MonoBehaviour
{
    [SerializeField] private EDriveMode _driveMode;

    [Header("Vehicle Properties")]
    [SerializeField] private float _torque = 1000.0f;
    [SerializeField] private float _brakeTorque = 1000.0f;
    [SerializeField] private AnimationCurve _steerCurve;
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private Vector3 _centerOfMass;
    
    
    [Header("Wheel Properties")]
    [SerializeField] private Wheel[] _wheels;

    private Rigidbody _rigidbody;
    private AutomobileInputManager _input;
    private float currentSteerAngle = 0f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _input = GetComponent<AutomobileInputManager>();
    }

    private void Start()
    {
        _rigidbody.centerOfMass = _centerOfMass;
    }

    private void FixedUpdate()
    {
        UpdateWheelAnimation();
        Engine();
        Steer();
    }

    private void Steer()
    {
        float targetSteerAngle = _input.GetSteerAxis() * maxSteerAngle;
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteerAngle, _steerCurve.Evaluate(Time.deltaTime));

        foreach (Wheel wheel in _wheels)
        {
            if (wheel.axel == EAxel.Front)
            {
                wheel.collider.steerAngle = currentSteerAngle;
            }
        }
    }


    private void UpdateWheelAnimation()
    {
        foreach (Wheel wheel in _wheels)
        {
            Quaternion rotation = new Quaternion();
            Vector3 position = new Vector3();
            wheel.collider.GetWorldPose(out position, out rotation);
            wheel.meshObject.transform.position = position;
            wheel.meshObject.transform.rotation = rotation;
        }
    }

    private void Engine()
    {
        float throttleInput = _input.GetThrottleAxis();
        float handbrakeTorque = _input.GetHandbrakeInputHeld() ? _brakeTorque : 0f;
        bool isMovingForward = _rigidbody.velocity.z > 0.1f; // Adjust threshold as needed
        bool isReversing = throttleInput < 0 && isMovingForward;

        foreach (Wheel wheel in _wheels)
        {
            // Manage braking for reversing
            if (isReversing)
            {
                wheel.collider.brakeTorque = _brakeTorque; // Apply brakes
                wheel.collider.motorTorque = 0; // Stop motor torque
            }
            else
            {
                wheel.collider.brakeTorque = (wheel.axel == EAxel.Rear && !isReversing) ? handbrakeTorque : 0f; // Release brakes

                // Apply motor torque based on drive mode
                if (_driveMode == EDriveMode.AWD || 
                    (_driveMode == EDriveMode.FWD && wheel.axel == EAxel.Front) ||
                    (_driveMode == EDriveMode.RWD && wheel.axel == EAxel.Rear))
                {
                    wheel.collider.motorTorque = _torque * throttleInput; // Apply throttle
                }
            }
        }
    }
}
