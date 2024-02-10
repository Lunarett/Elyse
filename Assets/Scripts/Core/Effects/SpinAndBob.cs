using UnityEngine;

public class SpinAndBob : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float _rotationSpeed = 50f;

    [Header("Bobbing Settings")]
    [SerializeField] private float _bobSpeed = 2f;
    [SerializeField] private float _bobHeight = 0.5f;

    private float _intialY;

    private void Start()
    {
        _intialY = transform.position.y;
    }

    private void Update()
    {
        UpdateRotation();
        UpdateBobbing();
    }

    private void UpdateBobbing()
    {
        float newY = _intialY + Mathf.Sin(Time.time * _bobSpeed) * _bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        
    }

    private void UpdateRotation()
    {
        transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime, Space.World);
    }
}