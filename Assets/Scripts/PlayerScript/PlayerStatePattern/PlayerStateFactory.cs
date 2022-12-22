using System.Collections.Generic;
using Unity.VisualScripting;

enum PlayerStates
{
    Idle,
    Run,
    Jump,
    Grounded,
    Fall,
    AirJump,
    WallSlide,
    WallJump
}

public class PlayerStateFactory
{
    PlayerStateMachine _context;
    Dictionary<PlayerStates, PlayerBaseState> _state = new Dictionary<PlayerStates, PlayerBaseState>();

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext;
        _state[PlayerStates.Grounded] = new PlayerGroundedState(_context, this);
        _state[PlayerStates.Jump] = new PlayerJumpState(_context, this);
        _state[PlayerStates.Idle] = new PlayerIdleState(_context, this);
        _state[PlayerStates.Run] = new PlayerRunState(_context, this);
        _state[PlayerStates.Fall] = new PlayerFallState(_context, this);
        _state[PlayerStates.AirJump] = new PlayerAirJumpState(_context, this);
        //_state["LedgeHang"] = new PlayerLedgeHangState(_context, this);
        _state[PlayerStates.WallSlide] = new PlayerWallSlideState(_context, this);
        _state[PlayerStates.WallJump] = new PlayerWallJumpState(_context, this);
    }

    public PlayerBaseState Grounded()
    {
        return _state[PlayerStates.Grounded];
    }

    public PlayerBaseState Jump()
    {
        return _state[PlayerStates.Jump];
    }

    public PlayerBaseState Idle()
    {
        return _state[PlayerStates.Idle];
    }

    public PlayerBaseState Run()
    {
        return _state[PlayerStates.Run];
    }

    public PlayerBaseState Fall()
    {
        return _state[PlayerStates.Fall];
    }

    public PlayerBaseState AirJump()
    {
        return _state[PlayerStates.AirJump];
    }

    public PlayerBaseState WallSlide()
    {
        return _state[PlayerStates.WallSlide];
    }

    public PlayerBaseState WallJump()
    {
        return _state[PlayerStates.WallJump];
    }
}
