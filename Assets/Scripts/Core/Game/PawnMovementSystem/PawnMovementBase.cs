using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PawnMovementBase : MonoBehaviour
{
    [Header("Movement Properties")]
    [SerializeField] protected float _movementSpeed = 10.0f;
    [SerializeField] protected float _movementSharpness = 15.0f;

    protected CharacterController _characterController;
    protected Vector3 _groundNormal;
    
    public float MovementSpeed => _movementSpeed;
    public Vector3 CharacterVelocity { get; set; }

    protected virtual void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    protected void MovePawn(Vector3 direction, float multiplier = 1)
    {
        direction.Normalize();
        float speed = _movementSpeed * multiplier;
        Vector3 targetVelocity = GetReorientDirectionOnSlope(direction, _groundNormal) * speed;
        CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity, _movementSharpness * Time.deltaTime);
        _characterController.Move(CharacterVelocity * Time.deltaTime);
    }


    protected Vector3 GetReorientDirectionOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        Vector3 directionRight = Vector3.Cross(direction, transform.up);
        return Vector3.Cross(slopeNormal, directionRight).normalized;
    }
    
    protected Vector3 CalculateCapsuleBottomPosition()
    {
        return transform.position + (transform.up * _characterController.radius);
    }

    protected Vector3 CalculateCapsuleTopPosition(float height)
    {
        return transform.position + (transform.up * (height - _characterController.radius));
    }
}