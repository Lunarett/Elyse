using UnityEngine;

public class SpringArm : MonoBehaviour
{
    [Header("Spring Arm Settings")]
    [SerializeField] private Vector3 _cameraOffset;
    [SerializeField] private float _armLength = 3.0f;
    [SerializeField] private float _collisionRadius = 0.2f;
    [SerializeField] private LayerMask _collisionLayers;

    private Camera _attachedCamera;
    private Vector3 _defaultLocalCameraPosition;

    public Camera AttachedCamera => _attachedCamera;
    public Vector3 Offset => _cameraOffset;

    private void Awake()
    {
        _attachedCamera = GetComponentInChildren<Camera>();
        if (_attachedCamera == null)
        {
            Debug.LogWarning("SpringArm: No Camera found as a child. Disabling script.");
            enabled = false;
            return;
        }

        _defaultLocalCameraPosition = _attachedCamera.transform.localPosition;
    }

    private void LateUpdate()
    {
        if (_attachedCamera != null)
        {
            UpdateCameraPosition();
        }
    }

    private void UpdateCameraPosition()
    {
        Transform newTransform = _attachedCamera.transform;
        newTransform.localPosition = _cameraOffset;
        Vector3 desiredCameraPos = newTransform.position + newTransform.forward * -_armLength;
        RaycastHit hit;

        if (Physics.SphereCast(newTransform.position, _collisionRadius, -newTransform.forward, out hit, _armLength, _collisionLayers))
        {
            _attachedCamera.transform.position = hit.point + hit.normal * _collisionRadius + _cameraOffset;
        }
        else
        {
            _attachedCamera.transform.position = desiredCameraPos;
        }
    }

    public void SetArmLength(float length)
    {
        _armLength = Mathf.Clamp(length, 0, 1000);
    }

    public void SetCameraOffset(Vector3 offset)
    {
        _cameraOffset = offset;
    }
}
