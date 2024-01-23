using UnityEngine;
using Cinemachine;

public class VehicleView : MonoBehaviour
{
    public CinemachineVirtualCamera cinemachineCamera;
    private AutomobileInputManager _inputManager;
    public float yawSpeed = 10f;
    public float pitchSpeed = 10f;

    private void Awake()
    {
        _inputManager = GetComponent<AutomobileInputManager>();
    }

    private void Update()
    {
        Vector2 lookInput = _inputManager.GetMouseLook();
        Vector3 rotation = cinemachineCamera.transform.rotation.eulerAngles;

        float yaw = rotation.y + lookInput.x * yawSpeed * Time.deltaTime;
        float pitch = rotation.x - lookInput.y * pitchSpeed * Time.deltaTime;

        cinemachineCamera.transform.rotation = Quaternion.Euler(pitch, yaw, 0f); // Setting roll to 0 prevents the camera from rolling
    }
}