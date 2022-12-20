public abstract class PlayerBaseState
{
    protected bool _isRootState = false;
    protected PlayerStateMachine Ctx;
    protected PlayerStateFactory Factory;
    protected PlayerBaseState CurrentSuperState;
    protected PlayerBaseState CurrentSubState;
    public PlayerBaseState(PlayerStateMachine context, PlayerStateFactory playerStateFactory)
    {
        Ctx = context;
        Factory = playerStateFactory;
    }

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

    public abstract void CheckSwitchStates();

    public abstract void InitializeSubState();

    public void UpdateStates()
    {
        UpdateState();
        if (CurrentSubState != null)
        {
            CurrentSubState.UpdateStates();
        }
    }

    protected void SwitchState(PlayerBaseState newState)
    {
        ExitState();
        newState.EnterState();
        if (_isRootState)
        {
            Ctx.CurrentState = newState;
        }
        else if (CurrentSuperState != null)
        {
            CurrentSuperState.SetSubState(newState);
        }
    }

    protected void SetSuperState(PlayerBaseState newSuperState)
    {
        CurrentSuperState = newSuperState;
    }

    protected void SetSubState(PlayerBaseState newSubState)
    {
        CurrentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
