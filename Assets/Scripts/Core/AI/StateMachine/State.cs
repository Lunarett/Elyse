using UnityEngine;

// Base abstract class that holds common methods and variables for different states
public abstract class State : IState
{
    public abstract EStateID GetStateID();

    public abstract void BeginState(Pawn pawn);
    public abstract void UpdateState(Pawn pawn);
    public abstract void EndState(Pawn pawn);

    // This is used for 2D games
    protected Vector2 GetMouseWorldLocation(Camera camera)
    {
        Vector3 mousePosWorld = camera.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(mousePosWorld.x, mousePosWorld.y);
    }

    protected bool IsInRange(Vector3 posA, Vector3 posB, float radius)
    {
        return Vector3.Distance(posA, posB) < radius;
    }
}
