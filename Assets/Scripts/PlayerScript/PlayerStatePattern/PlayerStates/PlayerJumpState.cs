using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
    }

    public override void EnterState()
    {
        HandleJump();
    }

    public override void UpdateState()
    {
        InitializeSubState();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.Rb.gravityScale = Ctx.FallMultiplier;
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.Rb.velocity.y <= 0f && !Ctx.Grounded)
        {
            SwitchState(Factory.Fall());
        }
        if (Ctx.CanAirJump)
        {
            SwitchState(Factory.AirJump());
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
    }

    void HandleJump()
    {
        Ctx.Rb.gravityScale = Ctx.JumpGravity;
        Ctx.ApplyAirLinearDrag();
        Ctx.Rb.velocity = new Vector2(Ctx.Rb.velocity.x, 0f);
        Ctx.Rb.AddForce(Vector2.up * Ctx.JumpForce, ForceMode2D.Impulse);
        Ctx.AirHangTimeCounter = 0f;
    }
}
