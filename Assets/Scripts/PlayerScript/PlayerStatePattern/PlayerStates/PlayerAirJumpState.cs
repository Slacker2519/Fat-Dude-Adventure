using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirJumpState : PlayerBaseState
{
    public PlayerAirJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
    }

    public override void EnterState()
    {
        HandleAirJump();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        InitializeSubState();
    }

    public override void ExitState()
    {
        
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.Rb.velocity.y < 0f && !Ctx.Grounded)
        {
            SwitchState(Factory.Fall());
        }

        if (Ctx.CanWallSlide)
        {
            SwitchState(Factory.WallSlide());
        }
    }

    public override void InitializeSubState()
    {
        if (Ctx.HorizontalDirection != 0f)
        {
            SetSubState(Factory.Run());
        }
        else
        {
            SetSubState(Factory.Idle());
        }
    }

    void HandleAirJump()
    {
        Ctx.AirJumpValue--;
        Ctx.Rb.gravityScale = Ctx.JumpGravity;
        Ctx.ApplyAirLinearDrag();
        Ctx.Rb.velocity = new Vector2(Ctx.Rb.velocity.x, 0f);
        Ctx.Rb.AddForce(Vector2.up * Ctx.JumpForce, ForceMode2D.Impulse);
    }
}
