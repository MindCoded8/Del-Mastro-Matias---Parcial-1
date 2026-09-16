public class FSM
{
    private State currentState;

    public State CurrentState => currentState;

    public void ChangeState(State newState)
    {
        if (newState == null || newState == currentState) return;

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }
}