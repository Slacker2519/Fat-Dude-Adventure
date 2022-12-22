using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
    }

    public override void EnterState()
    {
        Ctx.Rb.gravityScale = Ctx.JumpGravity;
        Ctx.AirHangTimeCounter = Ctx.AirHangTime;
        Ctx.AirJumpValue = Ctx.AirJump;
        ApllyGroundLinearDrag();
    }

    public override void UpdateState()
    {
        InitializeSubState();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.CanJump)
        {
            SwitchState(Factory.Jump());
        }
        else if (!Ctx.Grounded || Ctx.Rb.velocity.y < -0.05f)
        {
            SwitchState(Factory.Fall());
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

    void ApllyGroundLinearDrag()
    {
        if (Mathf.Abs(Ctx.HorizontalDirection) < 0.4f || !Ctx.ChangingDirection)
        {
            Ctx.Rb.drag = Ctx.GroundLinearDrag;
        }
        else
        {
            Ctx.Rb.drag = Ctx.AirLinearDrag;
        }
    }
}
