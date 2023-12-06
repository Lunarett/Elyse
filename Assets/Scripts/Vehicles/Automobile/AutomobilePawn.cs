using System.Collections;
using System.Collections.Generic;
using QFSW.QC;
using UnityEngine;
using UnityEngine.ProBuilder;

public class AutomobilePawn : Pawn, IInteractable
{
    [SerializeField] private float upsideDownSteerForce = 5f;
    [SerializeField] private Camera _mainCamera;

    private Rigidbody _rigidBody;
    private VehicleInputHandler _input;
    
    protected override void Awake()
    {
        base.Awake();
        _input = GetComponent<VehicleInputHandler>();
        _rigidBody = GetComponent<Rigidbody>();
    }
    
    protected override void Start()
    {
        base.Start();
        ShowMouseCursor(false);
    }

    private void Update()
    {
        if (IsVehicleUpsideDown())
        {
            ApplyUpsideDownSteering();
        }
    }

    private bool IsVehicleUpsideDown()
    {
        return transform.up.y <= 0;
    }

    private void ApplyUpsideDownSteering()
    {
        float steerForce = _input.GetSteerInput() * upsideDownSteerForce * Time.deltaTime;
        Vector3 direction = transform.TransformDirection(new Vector3(0, 0, steerForce));
        _rigidBody.angularVelocity += direction;
    }

    [Command("car.flip")]
    private void Flip()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);
        transform.eulerAngles = new Vector3(0, 0, 180);
    }

    public void Interact(Controller controller)
    {
        if (Owner != null) return;
        controller.Possess(this);
        
    }
}