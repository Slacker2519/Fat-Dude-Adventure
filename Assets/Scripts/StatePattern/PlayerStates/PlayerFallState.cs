using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
    }

    public override void EnterState()
    {
        Ctx.Rb.gravityScale = Ctx.FallMultiplier;
    }

    public override void UpdateState()
    {
        if (!Ctx.IsJumpPress && !Ctx.Grounded)
        {
            Ctx.AirHangTimeCounter -= Time.deltaTime;
        }
        HandleFall();
        InitializeSubState();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.Rb.gravityScale = 0f;
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.Grounded)
        {
            SwitchState(Factory.Grounded());
        }
        else if (Ctx.CanJump)
        {
            SwitchState(Factory.Jump());
        }
        else if (Ctx.CanAirJump)
        {
            SwitchState(Factory.AirJump());
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

    void HandleFall()
    {
        Ctx.Rb.gravityScale += Ctx.FallMultiplier * Time.deltaTime;

        if (Ctx.Rb.gravityScale > Ctx.MaxGravity)
        {
            Ctx.Rb.gravityScale = Ctx.MaxGravity;
        }
    }
}
