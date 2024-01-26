using UnityEngine;

// Base abstract class that holds common methods and variables for different states
public abstract class State : IState
{
    public abstract EStateID GetStateID();

    public abstract void BeginState(AIPawn pawn);
    public abstract void UpdateState(AIPawn pawn);
    public abstract void EndState(AIPawn pawn);
    
    protected bool IsInRange(Vector3 posA, Vector3 posB, float radius)
    {
        return Vector3.Distance(posA, posB) < radius;
    }
}
