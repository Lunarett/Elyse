using System.Collections;
using FMODUnity;
using UnityEngine;

public class AutomobileMovement : MonoBehaviour
{
    [Header("Vehicle Settings")]
    [SerializeField] private float _acceleration = 500f;
    [SerializeField] private float _steeringAngle = 30f;
    [SerializeField] private float _brakeForce = 300f;
    [SerializeField] private float _reverseMultiplier = 0.5f;
    [SerializeField] private DriveMode _driveMode = DriveMode.RWD;

    [Header("Motor and Gears")]
    [SerializeField] private float MaxForce = 1000f;
    [SerializeField] private float ForceTransitionSpeed = 0.1f;
    [SerializeField] private float ForceTransitionThreshold = 10f;
    [SerializeField] private float[] _gearRatios = { 3.2f, 2.1f, 1.4f, 1.0f, 0.8f };

    [Header("Wheel Configuration")]
    [SerializeField] private WheelConfig[] _wheelConfigs = new WheelConfig[4];

    private StudioEventEmitter _engineSoundEmitter;
    private VehicleInputHandler _inputHandler;
    private float _throttleInput;
    private bool _handbrakeHeld;
    private int _currentGear;
    private float _engineRPM;
    private float _currentAcceleration;
    
    private float _targetRPM;
    private float _rpmTransitionSpeed = 5f;

    private enum DriveMode { RWD, FWD, AWD }
    private enum WheelAxis { Front, Rear }

    [System.Serializable]
    private struct WheelConfig
    {
        public string name;
        public bool canSteer;
        public WheelAxis axis;
        public Transform wheelMesh;
        public WheelCollider wheelCollider;
    }

    private void Awake()
    {
        _engineSoundEmitter = GetComponent<StudioEventEmitter>();
        _inputHandler = GetComponent<VehicleInputHandler>();
        _currentAcceleration = _acceleration;
        _currentGear = 0;
    }

    private void FixedUpdate()
    {
        GetInputs();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        HandleGearShift();
        SmoothRPMTransition();
        UpdateEngineSound();
    }

    private void GetInputs()
    {
        _throttleInput = _inputHandler.GetThrottleInput();
        _handbrakeHeld = _inputHandler.GetHandbrakeInputHeld();
    }

    private void HandleMotor()
    {
        float currentForce = _throttleInput * _currentAcceleration;
        if (_throttleInput < 0) // Reverse gear multiplier
        {
            currentForce *= _reverseMultiplier;
        }

        // Limit the force to avoid excessive torque
        currentForce = Mathf.Clamp(currentForce, -MaxForce, MaxForce);

        foreach (var wheelConfig in _wheelConfigs)
        {
            // Apply motor torque
            if ((_driveMode == DriveMode.AWD) ||
                (_driveMode == DriveMode.FWD && wheelConfig.axis == WheelAxis.Front) ||
                (_driveMode == DriveMode.RWD && wheelConfig.axis == WheelAxis.Rear))
            {
                wheelConfig.wheelCollider.motorTorque = currentForce;
            }

            // Apply brake force
            if (_handbrakeHeld)
            {
                wheelConfig.wheelCollider.brakeTorque = _brakeForce;
            }
            else
            {
                wheelConfig.wheelCollider.brakeTorque = 0;
            }
        }

        _targetRPM = _wheelConfigs[0].wheelCollider.rpm * _gearRatios[_currentGear] * 100;
    }

    
    private void HandleGearShift()
    {
        if (_engineRPM > 4000 && _currentGear < _gearRatios.Length - 1) // Example shift-up RPM
        {
            _currentGear++;
            Debug.Log("ShiftUp");
            StartCoroutine(SmoothGearTransition());
        }
        else if (_engineRPM < 2000 && _currentGear > 0) // Example shift-down RPM
        {
            Debug.Log("ShiftDown");
            _currentGear--;
            StartCoroutine(SmoothGearTransition());
        }
    }
    
    private IEnumerator SmoothGearTransition()
    {
        float targetForce = _acceleration * _gearRatios[_currentGear];
        while (Mathf.Abs(_currentAcceleration - targetForce) > ForceTransitionThreshold)
        {
            _currentAcceleration = Mathf.Lerp(_currentAcceleration, targetForce, ForceTransitionSpeed * Time.deltaTime);
            yield return null;
        }
        _currentAcceleration = targetForce;
    }

    private void HandleSteering()
    {
        float steering = _inputHandler.GetSteerInput() * _steeringAngle;
        foreach (var wheelConfig in _wheelConfigs)
        {
            if (wheelConfig.canSteer)
            {
                wheelConfig.wheelCollider.steerAngle = steering;
            }
        }
    }

    private void UpdateWheels()
    {
        foreach (var wheelConfig in _wheelConfigs)
        {
            if (wheelConfig.wheelMesh != null)
            {
                UpdateWheelPose(wheelConfig.wheelCollider, wheelConfig.wheelMesh);
            }
        }
    }

    private void UpdateWheelPose(WheelCollider wheelCollider, Transform wheelMesh)
    {
        Vector3 pos = wheelMesh.position;
        Quaternion quat = wheelMesh.rotation;

        wheelCollider.GetWorldPose(out pos, out quat);

        wheelMesh.position = pos;
        wheelMesh.rotation = quat;
    }
    
    private void UpdateEngineSound()
    {
        if (_engineSoundEmitter != null)
        {
            float rpm = Mathf.Clamp(_engineRPM, 800, 8000.0f);
            _engineSoundEmitter.SetParameter("RPM", rpm);
        }
        else
        {
            Debug.LogError("Sound is missing!");
        }
    }
    
    private void SmoothRPMTransition()
    {
        // Smoothly interpolate the current RPM value towards the target RPM
        _engineRPM = Mathf.Lerp(_engineRPM, _targetRPM, _rpmTransitionSpeed * Time.deltaTime);
    }
}
