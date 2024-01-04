using UnityEngine;

public class ChaseState : IState
{
    public EStateID GetStateID()
    {
        return EStateID.Chase;
    }

    public void BeginState(Pawn pawn)
    {
    }

    public void UpdateState(Pawn pawn)
    {
    }

    public void EndState(Pawn pawn)
    {
    }
}
