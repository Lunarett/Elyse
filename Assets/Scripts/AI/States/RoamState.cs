using UnityEngine;
using UnityEngine.AI;

public class RoamState : IState
{
    public EStateID GetStateID()
    {
        return EStateID.Roam;
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