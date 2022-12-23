using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerBaseState
{
    public PlayerWallSlideState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
    }

    public override void EnterState()
    {
        Ctx.AirJumpValue = Ctx.AirJump;
        HandleWallSlide();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        InitializeSubState();
    }

    public override void ExitState()
    {
        Ctx.Rb.gravityScale = Ctx.FallMultiplier;
        Ctx.Rb.drag = Ctx.AirLinearDrag;
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.Grounded)
        {
            SwitchState(Factory.Grounded());
        }
        if ((!Ctx.NextToWall && !Ctx.Grounded) || (Ctx.NextToWall && !Ctx.Grounded && Ctx.GroundedTransition))
        {
            SwitchState(Factory.Fall());
        }
        if (Ctx.IsJumpPress)
        {
            SwitchState(Factory.WallJump());
        }
    }

    public override void InitializeSubState()
    {
        if (Ctx.HorizontalDirection != 0f)
        {
            SetSubState(Factory.Run());
        }
    }

    void HandleWallSlide()
    {
        Ctx.Rb.gravityScale = Ctx.WallSlideGravity;
        Ctx.Rb.drag = Ctx.WallSlideDrag;
    }
}
