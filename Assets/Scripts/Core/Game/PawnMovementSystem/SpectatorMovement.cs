using UnityEngine;

public class SpectatorMovement : PawnMovementBase
{
    public void Move(Vector2 inputAxis)
    {
        Vector3 forward = transform.forward;
        forward.y = 0;
        Vector3 right = transform.right;
        right.y = 0;
        Vector3 direction = (forward * inputAxis.y + right * inputAxis.x).normalized;

        MovePawn(direction);
    }
}
