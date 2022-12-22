using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {

    }

    public override void EnterState()
    {
        
    }

    public override void UpdateState()
    {
        MovePlayer();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.HorizontalDirection == 0f)
        {
            SwitchState(Factory.Idle());
        }
    }

    public override void InitializeSubState()
    {

    }

    void MovePlayer()
    {
        Ctx.Rb.AddForce(new Vector2(Ctx.HorizontalDirection, 0f) * Ctx.MovementAcceleration);
        if (Mathf.Abs(Ctx.Rb.velocity.x) > Ctx.MaxMoveSpeed)
        {
            Ctx.Rb.velocity = new Vector2(Mathf.Sign(Ctx.Rb.velocity.x) * Ctx.MaxMoveSpeed, Ctx.Rb.velocity.y);
        }
    }
}
