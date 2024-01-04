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
        void BeginState(Pawn pawn);
        void UpdateState(Pawn pawn);
        void EndState(Pawn pawn);
    }