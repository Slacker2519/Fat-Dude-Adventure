using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerBaseState
{
    public PlayerWallJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
    }

    public override void EnterState()
    {
        WallJumpHandle();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.Rb.gravityScale = Ctx.FallMultiplier;
        Ctx.Rb.drag = Ctx.AirLinearDrag;
    }

    public override void CheckSwitchStates()
    {
        if (!Ctx.Grounded || Ctx.Rb.velocity.y < -0.05f)
        {
            SwitchState(Factory.Fall());
        }
    }

    public override void InitializeSubState()
    {
        
    }

    void WallJumpHandle()
    {
        float xAngle = Mathf.Cos(Ctx.WallJumpAngle * Mathf.Deg2Rad);
        float yAngle = Mathf.Sin(Ctx.WallJumpAngle * Mathf.Deg2Rad);

        if (Ctx.WallOnLeft)
        {
            Ctx.Rb.drag = Ctx.WallSlideDrag;
            Ctx.Rb.velocity = Vector2.zero;
            Ctx.Rb.AddForce(new Vector2(xAngle, yAngle).normalized * Ctx.WallJumpForce * 100f);
        }
        else if (Ctx.WallOnRight)
        {
            Ctx.Rb.drag = Ctx.WallSlideDrag;
            Ctx.Rb.velocity = Vector2.zero;
            Ctx.Rb.AddForce(new Vector2(xAngle * -1f, yAngle).normalized * Ctx.WallJumpForce * 100f);
        }
    }
}
