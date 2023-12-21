using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Wheel References")]
    public MeshFilter mesh;
    public WheelCollider collider;
    
    [Header("Wheel Properties")]
    public bool canSteer;
    public EAxel axel;

    [Header("VFX")]
    public ParticleSystem _tireParticles;
}

[RequireComponent(typeof(Rigidbody))]
public class Automobile : MonoBehaviour
{
    [SerializeField] private bool _debug;
    
    [Header("Drive Properties")]
    [SerializeField] private EDriveMode _driveMode = EDriveMode.RWD;
    [SerializeField] private float _vehicleMass = 1500.0f;
    [Space(5)]
    [SerializeField] private Vector3 _centerOfMass = new Vector3(0.0f, -0.8f, 0.0f);
    [Space(5)]
    [SerializeField] private AnimationCurve _torqueCurve = new AnimationCurve(
        new Keyframe(0, 100),
        new Keyframe(1500, 250),
        new Keyframe(3000, 300),
        new Keyframe(4500, 250),
        new Keyframe(6000, 200)
    );
    [SerializeField] private float _forwardSpeedLimit = 220.0f;
    [SerializeField] private float _brakePower = 1000.0f;
    [SerializeField] private float _handBrakeFriction = 0.05f;
    [SerializeField] private float _handBrakeFrictionMultiplier = 2.0f;
    [SerializeField] private float _reverseSpeedLimit = 60.0f;

    [Header("Steer Properties")]
    [SerializeField] private float _steerAngle = 30.0f;
    [SerializeField] private float _minSteerAngleAtMaxSpeed = 10.0f;
    [SerializeField] private float _steerSmoothSpeed = 5.0f;

    [Header("Wheel Properties")]
    [SerializeField] private float _wheelMass = 50.0f;
    [SerializeField] private float _wheelRadius = 30.0f;
    [SerializeField] private float _wheelDampingRate = 0.25f;
    [SerializeField] private float _suspensionDistance = 0.3f;
    [SerializeField] private float _forceAppPointDistance;
    [Space(5)]
    [SerializeField] private Vector3 _wheelCenter;
    [Space(5)]
    [SerializeField] private float _springTention = 35000.0f;
    [SerializeField] private float _springDamper = 4500.0f;
    [SerializeField] private float _springTargetPosition = 0.5f;
    [Space(5)]
    [SerializeField] private float _forwardExtremumSlip = 0.4f;
    [SerializeField] private float _forwardExtremumValue = 1.0f;
    [SerializeField] private float _forwardAsymptoteSlip = 0.8f;
    [SerializeField] private float _forwardAsymptoteValue = 0.5f;
    [SerializeField] private float _forwardStiffness = 1.0f;
    [Space(5)]
    [SerializeField] private float _sidewaysExtremumSlip = 0.2f;
    [SerializeField] private float _sidewaysExtremumValue = 1.0f;
    [SerializeField] private float _sidewaysAsymptoteSlip = 0.5f;
    [SerializeField] private float _sidewaysAsymptoteValue = 0.75f;
    [SerializeField] private float _sidewaysStiffness = 1.0f;

    [SerializeField] private float _frictionMultiplier = 3.0f;
    
    [SerializeField] private Wheel[] _wheels;

    [Header("Gear Properties")]
    [SerializeField] private bool _automatic = true;
    [SerializeField] private float _upShiftRPM = 4500.0f;
    [SerializeField] private float _downShiftRPM = 1000.0f;
    [SerializeField] private AnimationCurve _gearRatios = new AnimationCurve(
        new Keyframe(0, 3.5f),
        new Keyframe(1, 2.5f),
        new Keyframe(2, 1.8f),
        new Keyframe(3, 1.2f),
        new Keyframe(4, 0.8f)
    );

    [SerializeField] private float[] _gearSpeeds = {30, 60, 90, 120, 150};
    
    private Rigidbody _rigidBody;
    private VehicleInputHandler _input;

    private WheelFrictionCurve _forwardFriction;
    private WheelFrictionCurve _sidewaysFriction;
    
    private float _currentSpeed;
    private float _currentTorque;
    private float _totalTorque;
    private float _outputTorque;
    private float _currentSteerAngle;
    private float _radius;
    private bool _checkSpeeds = true;
    private float _targetSteerAngle = 0.0f;

    private float _acceleration = 0.0f;
    private float _throttlePower = 0.8f;
    private float _throttleAmount;
    private int _currentGear = 1;
    private float _finalDrive;
    private float _downForce;
    private float _tempo;
    private float _finalDriveRatio1 = 4.8f, _finalDriveRatio2 = 3.9f;
    private float _topSpeedDrag, _idleDrag = 0.05f;
    private float _reverseDrag = 0.6f;
    private float _runningDrag = 0.02f;

    private float _engineRPM = 0.0f;
    private float _idleRPM = 0.0f;
    private float _wheelRPM = 0.0f;
    
    private float _engineSmoothTime = 0.1f;
    
    private float _throttleInputAxis = 0.0f;
    private float _reverseInputAxis = 0.0f;
    private float _steerInputAxis = 0.0f;
    private bool _isBrakeInputHeld;

    public Wheel[] Wheels => _wheels;
    
    // Debug variables
    private Font _debugFont;
    private GUIStyle _debugStyle;
    private GUIStyle _debugBackgroundStyle;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        SetDebugStyle();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _rigidBody.mass = _vehicleMass;
        _rigidBody.centerOfMass = _centerOfMass;

        foreach (Wheel w in _wheels)
        {
            if (w.collider == null) continue;
            
            w.collider.mass = _wheelMass;
            w.collider.radius = _wheelRadius;
            w.collider.wheelDampingRate = _wheelDampingRate;
            w.collider.suspensionDistance = _suspensionDistance;
            w.collider.center = _wheelCenter;
            
            JointSpring colliderSuspensionSpring = w.collider.suspensionSpring;
            colliderSuspensionSpring.spring = _springTention;
            colliderSuspensionSpring.damper = _springDamper;
            colliderSuspensionSpring.targetPosition = _springTargetPosition;

            _forwardFriction = w.collider.forwardFriction;
            _forwardFriction.extremumSlip = _forwardExtremumSlip;
            _forwardFriction.extremumValue = _forwardExtremumValue;
            _forwardFriction.asymptoteSlip = _forwardAsymptoteSlip;
            _forwardFriction.asymptoteValue = _forwardAsymptoteValue;
            
            _sidewaysFriction = w.collider.sidewaysFriction;
            _sidewaysFriction.extremumSlip = _sidewaysExtremumSlip;
            _sidewaysFriction.extremumValue = _sidewaysExtremumValue;
            _sidewaysFriction.asymptoteSlip = _sidewaysAsymptoteSlip;
            _sidewaysFriction.asymptoteValue = _sidewaysAsymptoteValue;
        }

        StartCoroutine(CustomTick(0.1f));
    }

    private void Update()
    {
        _throttleInputAxis = Keyboard.current.wKey.isPressed ? 1.0f : 0.0f;
        _reverseInputAxis = Keyboard.current.sKey.isPressed ? -1.0f : 0.0f;
        _steerInputAxis = Keyboard.current.dKey.isPressed ? 1.0f : 0.0f;
        _steerInputAxis -= Keyboard.current.aKey.isPressed ? 1.0f : 0.0f;
        _isBrakeInputHeld = Keyboard.current.spaceKey.isPressed;
    }
    
    private IEnumerator CustomTick(float updateRate)
    {
        while (true)
        {
            yield return new WaitForSeconds(updateRate);
            AdjustFinalDrive();
            AdjustDrag();
            AddDownForce();
            _radius = (_currentSpeed > 50) ? 6 + (_currentGear / _forwardSpeedLimit) * 40 : 6;
        }
    }

    private void FixedUpdate()
    {
        AdjustTraction();
        CalculateTorque();
        ApplyDriveTorque();
        AckermanSteering();
        UpdateWheelAnimation();
        Brake();
        CheckWheelSpin();
        UpdateWheelParticles();
    }

    private void UpdateWheelParticles()
    {
        bool isCarMoving = _currentSpeed > 1.0f; // Adjust this threshold as needed

        foreach (Wheel w in _wheels)
        {
            if (w.collider.isGrounded && isCarMoving)
            {
                if (!w._tireParticles.isPlaying)
                {
                    w._tireParticles.Play();
                }
            }
            else
            {
                if (w._tireParticles.isPlaying)
                {
                    w._tireParticles.Stop();
                }
            }
        }
    }
    
    private void CalculateTorque()
    {
        _acceleration = (_currentGear == 1) ? Mathf.MoveTowards(0, 1 * _throttleInputAxis, _throttlePower) : 1;
        _throttleAmount = AdjustThrottleForTraction(_throttleInputAxis);
        
        ShiftGear();
        CalculateEngineRPM();

        _totalTorque = _torqueCurve.Evaluate(_engineRPM) * (_gearRatios.Evaluate(_currentGear)) * _finalDrive * _throttleAmount *
                      _acceleration;

        if (_engineRPM >= _upShiftRPM)
        {
            _totalTorque = 0;
            _engineRPM = _upShiftRPM;
        }
    }
    
    private void CalculateEngineRPM()
    {
        // Calclulate Idle RPM
        _idleRPM = (_currentGear > 1) ? 0 : _idleRPM;
       
        // Calculate Wheel RPM
        float sum = 0;
        int c = 0;
        foreach (Wheel w in _wheels)
        {
            sum += w.collider.rpm;
            c++;
        }

        _wheelRPM = (c != 0) ? sum / c : 0;
        
        float velocity = 0.0f;
        _engineRPM = Mathf.SmoothDamp(_engineRPM,
            _idleRPM + (Mathf.Abs(_wheelRPM) * _finalDrive * _gearRatios.Evaluate(_currentGear)), ref velocity, _engineSmoothTime);
        _currentSpeed = _rigidBody.velocity.magnitude * 3.6f;
    }
    
    private float AdjustThrottleForTraction(float throttleInput)
    {
        foreach (Wheel w in _wheels)
        {
            w.collider.GetGroundHit(out WheelHit hit);
            if (hit.forwardSlip is > 0.12f or < -0.12f)
            {
                return throttleInput -= hit.forwardSlip;
            }

            if (hit.sidewaysSlip is > .3f or < -0.3f && !_isBrakeInputHeld)
            {
                return throttleInput -= Mathf.Abs(hit.sidewaysSlip);
            }
        }

        return _throttleInputAxis;
    }

    private void ApplyDriveTorque()
    {
        float torquePerWheel = _driveMode == EDriveMode.AWD ? _totalTorque / 4 : _totalTorque / 2;

        // Apply torque to front wheels for FWD and AWD
        if (_driveMode == EDriveMode.AWD || _driveMode == EDriveMode.FWD)
        {
            foreach (Wheel w in _wheels)
            {
                if (w.axel == EAxel.Front)
                {
                    w.collider.motorTorque = torquePerWheel;
                }
            }
        }

        // Apply torque to rear wheels for RWD and AWD
        if (_driveMode == EDriveMode.AWD || _driveMode == EDriveMode.RWD)
        {
            foreach (Wheel w in _wheels)
            {
                if (w.axel == EAxel.Rear)
                {
                    w.collider.motorTorque = torquePerWheel;
                }
            }
        }
    }
    
    private void AdjustTraction()
    {
        _forwardFriction = _wheels[0].collider.forwardFriction;
        _sidewaysFriction = _wheels[0].collider.sidewaysFriction;
        
        if (!_isBrakeInputHeld)
        {
            _forwardFriction.extremumValue = _forwardFriction.asymptoteValue = ((_currentSpeed * _frictionMultiplier) / 300) + 1;
            _sidewaysFriction.extremumValue = _sidewaysFriction.asymptoteValue = ((_currentSpeed * _frictionMultiplier) / 300) + 1;

            foreach (Wheel w in _wheels)
            {
                w.collider.forwardFriction = _forwardFriction;
                w.collider.sidewaysFriction = _sidewaysFriction;
            }
        }

        else if (Keyboard.current.spaceKey.isPressed)
        {
            float velocity = 0;
            _sidewaysFriction.extremumValue = _sidewaysFriction.asymptoteValue =
                Mathf.SmoothDamp(_sidewaysFriction.asymptoteValue, _handBrakeFriction, ref velocity,
                    0.05f * Time.deltaTime);
            _forwardFriction.extremumValue = _forwardFriction.asymptoteValue =
                Mathf.SmoothDamp(_forwardFriction.asymptoteValue, _handBrakeFriction, ref velocity,
                    0.05f * Time.deltaTime);

            foreach (Wheel w in _wheels)
            {
                w.collider.forwardFriction = _forwardFriction;
                w.collider.sidewaysFriction = _sidewaysFriction;
            }

            _sidewaysFriction.extremumValue = _sidewaysFriction.asymptoteValue = 1.5f;
            _forwardFriction.extremumValue = _forwardFriction.asymptoteValue = 1.5f;

            foreach (Wheel w in _wheels)
            {
                w.collider.forwardFriction = _forwardFriction;
                w.collider.sidewaysFriction = _sidewaysFriction;
            }
        }
    }
    
    private void AdjustFinalDrive()
    {
        _finalDrive = _currentGear is 1 or 4 or 5 ? _finalDriveRatio1 : _finalDriveRatio2;
    }
    
    private void AdjustDrag()
    {
        if (_currentSpeed >= _forwardSpeedLimit)
            _rigidBody.drag = _topSpeedDrag;
        else if (_outputTorque == 0)
            _rigidBody.drag = _idleDrag;
        else if (_currentSpeed >= _reverseSpeedLimit && _currentGear == -1 && _wheelRPM <= 0)
            _rigidBody.drag = _reverseDrag;
        else
        {
            _rigidBody.drag = _runningDrag;
        }
    }

    private void AddDownForce()
    {
        _downForce = _currentSpeed / 2;
        _rigidBody.AddForce(-transform.up * (_downForce * _rigidBody.velocity.magnitude));
    }
    
    private void Brake()
    {
        foreach (Wheel w in _wheels)
        {
            w.collider.brakeTorque = (_reverseInputAxis < 0) ? _brakePower : 0;
            if (_reverseInputAxis < 0 && _throttleInputAxis == 0) _rigidBody.AddForce(-transform.forward * 1000);
            if (_currentSpeed < 0 && _reverseInputAxis == 0) w.collider.brakeTorque = _brakePower * 18000;
            else w.collider.brakeTorque = 0;
        }
    }
    
    private void AckermanSteering()
    {
        _steerInputAxis = Keyboard.current.dKey.isPressed ? 1.0f : 0.0f;
        _steerInputAxis -= Keyboard.current.aKey.isPressed ? 1.0f : 0.0f;
    
        // Adjust the maximum steer angle based on the current speed
        float speedFactor = Mathf.Clamp01(_currentSpeed / _forwardSpeedLimit);
        float currentMaxSteerAngle = Mathf.Lerp(_steerAngle, _minSteerAngleAtMaxSpeed, speedFactor);
        _targetSteerAngle = currentMaxSteerAngle * _steerInputAxis;

        foreach (Wheel w in _wheels)
        {
            if (w.canSteer)
            {
                float steerAngle = w.axel == EAxel.Rear ? -_targetSteerAngle : _targetSteerAngle;
                w.collider.steerAngle = Mathf.Lerp(w.collider.steerAngle, steerAngle, Time.deltaTime * _steerSmoothSpeed);
            }
        }
    }

    
    private void CheckWheelSpin()
    {
        float blind = 0.28f;

        if (_isBrakeInputHeld)
        {
            foreach (Wheel w in _wheels)
            {
                w.collider.GetGroundHit(out WheelHit hit);
                if (hit.sidewaysSlip > blind || hit.sidewaysSlip < -blind)
                {
                    ApplyBooster(hit.sidewaysSlip);
                }
            }
        }
        
        foreach (Wheel w in _wheels)
        {
            w.collider.GetGroundHit(out WheelHit hit);
            
            float hInp = Keyboard.current.dKey.isPressed ? 1.0f : 0.0f;
            hInp -= Keyboard.current.aKey.isPressed ? 1.0f : 0.0f;
            
            if (hit.sidewaysSlip < 0)
                _tempo = (1 + -hInp) * Mathf.Abs(hit.sidewaysSlip * _handBrakeFrictionMultiplier);
            if (_tempo < 0.5) _tempo = 0.5f;
            if (hit.sidewaysSlip > 0)
                _tempo = (1 + hInp) * Mathf.Abs(hit.sidewaysSlip * _handBrakeFrictionMultiplier);
            if (_tempo < 0.5) _tempo = 0.5f;
            if (hit.sidewaysSlip > .99f || hit.sidewaysSlip < -.99f)
            {
                float velocity = 0;
                _handBrakeFriction = Mathf.SmoothDamp(_handBrakeFriction, _tempo * 3, ref velocity, 0.1f * Time.deltaTime);
            }
            else
                _handBrakeFriction = _tempo;
        }
    }
    
    private void ShiftGear()
    {
        if ((_currentGear < _gearRatios.length - 1 && _engineRPM >= _upShiftRPM ||
             (_currentGear == 0 && (_throttleInputAxis > 0 || _reverseInputAxis < 0))) && !IsFlying() && CheckGearSpeed())
        {
            _currentGear++;
        }

        if (_currentGear > 1 && _engineRPM <= _downShiftRPM)
            _currentGear--;
        if (CheckStandStill() && _reverseInputAxis < 0)
            _currentGear = -1;
        if (_currentGear == -1 && CheckStandStill() && _throttleInputAxis > 0)
            _currentGear = 1;
    }
    
    private bool CheckStandStill()
    {
        return _currentSpeed == 0;
    }
    
    private bool CheckGearSpeed()
    {
        if (_currentGear != -1)
        {
            // Ensure gearNum is within the bounds of the gearSpeeds array
            if (_currentGear > 0 && _currentGear <= _gearSpeeds.Length)
            {
                if (_checkSpeeds)
                {
                    return _currentSpeed >= _gearSpeeds[_currentGear - 1];
                }
                else
                    return true;
            }
        }
        return false;
    }
    
    private bool IsFlying()
    {
        return _wheels.All(w => !w.collider.isGrounded);
    }
    
    public void ApplyBooster(float amount)
    {
        float R = Mathf.Abs((_currentSpeed / (_forwardSpeedLimit * 2)) * 15000);
        if (_throttleInputAxis == 0) return;
        _rigidBody.AddForce(transform.forward * (1 + (_currentSpeed / _throttleInputAxis) * 5000));
        _rigidBody.AddForce(-transform.right * (amount * R * 2));
    }
    
    private void UpdateWheelAnimation()
    {
        foreach (Wheel w in _wheels)
        {
            w.collider.GetWorldPose(out Vector3 position, out Quaternion rotation);
            w.mesh.transform.position = position;
            w.mesh.transform.rotation = rotation;
        }
    }
    
    private void SetDebugStyle()
    {
        _debugFont = Resources.Load<Font>("UI/Fonts/SourceCodePro-Semibold");
        if (!_debugFont) Debug.LogError("Font was not found!");

        // Initialize the GUIStyle for text
        _debugStyle = new GUIStyle();
        _debugStyle.fontSize = 15;
        _debugStyle.normal.textColor = Color.white;
        _debugStyle.alignment = TextAnchor.UpperRight;
        if (_debugFont != null)
        {
            _debugStyle.font = _debugFont;
        }
        else
        {
            Debug.LogWarning("Debug font not found. Using default font.");
        }

        // Initialize the GUIStyle for background
        _debugBackgroundStyle = new GUIStyle();
        _debugBackgroundStyle.normal.background = MakeTex(2, 2, new Color(0, 0, 0, 0.5f)); // Semi-transparent black
    }
    
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
    
    private void OnGUI()
    {
        if (!_debug) return;

        Rect debugArea = new Rect(Screen.width - 510, 20, 500, 350); // Adjusted height for the title
        GUI.Box(debugArea, GUIContent.none, _debugBackgroundStyle);

        // Title
        GUIStyle titleStyle = new GUIStyle();
        titleStyle.fontSize = 16;
        titleStyle.normal.textColor = Color.yellow; // Set title color
        titleStyle.alignment = TextAnchor.UpperCenter;
        titleStyle.font = _debugFont;
        GUI.Label(new Rect(debugArea.x, debugArea.y, debugArea.width, 30), "Car Debug Info", titleStyle);

        string debugInfo = $"Current Speed: {_currentSpeed:F2} km/h\nGear: {_currentGear}\n\nEngine RPM: {_engineRPM:F2}\nEngine Torque: {_totalTorque:F2}\nSteering Angle: {_currentSteerAngle:F2}\n" +
                           $"Throttle Input: {_throttleInputAxis:F2}\nSteer Input: {_steerInputAxis:F2}\nBrake Input: {_isBrakeInputHeld}\n\\n" +
                           "Wheel Data:\n";

        foreach (Wheel w in _wheels)
        {
            w.collider.GetGroundHit(out WheelHit hit);
            debugInfo += $"{w.name} - Slip: Fwd {hit.forwardSlip:F2}, Side {hit.sidewaysSlip:F2}, Brake Torque: {w.collider.brakeTorque:F2}\n";
        }

        // Adjust the position for the content to start below the title
        GUI.Label(new Rect(debugArea.x - 10, debugArea.y + 30, debugArea.width, debugArea.height - 30), debugInfo, _debugStyle);
    }
}