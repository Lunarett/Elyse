public class StateMachine
{
    private IState[] states;
    private EStateID CurrentStateID;
    private Pawn _pawn;

    public StateMachine(Pawn pawn)
    {
        this._pawn = pawn;
        int len = System.Enum.GetNames(typeof(EStateID)).Length;
        states = new IState[len];
    }

    public void InitializeState(IState state)
    {
        int i = (int)state.GetStateID();
		states[i] = state;
    }

    public IState GetState(EStateID stateID)
    {
        int i = (int)stateID;
		return states[i];
    }

    public void UpdateStateMachine()
    {
        GetState(CurrentStateID)?.UpdateState(_pawn);
    }

    public void ChangeState(EStateID newStateID)
    {
        GetState(CurrentStateID)?.EndState(_pawn);
		CurrentStateID = newStateID;
		GetState(CurrentStateID)?.BeginState(_pawn);
    }
}