using UnityEngine;

    // Enum containg all states. This will be used to determine which state to switch to..
    public enum EStateID
    {
        Roam,
        Chase
    }

    public interface IState
    {
        EStateID GetStateID();
        void BeginState(AIPawn pawn);
        void UpdateState(AIPawn pawn);
        void EndState(AIPawn pawn);
    }