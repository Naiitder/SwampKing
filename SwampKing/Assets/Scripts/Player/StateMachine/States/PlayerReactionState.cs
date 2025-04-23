using UnityEngine;

public class PlayerReactionState : PlayerBaseState
{

    public PlayerReactionState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }
    
    public override void CheckSwitchStates()
    {
    }

    public override void EnterState()
    {
        _ctx.PlayerMovement.StopMovement();
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.IsReactingHash,true);
            
    }


    public override void ExitState()
    {
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.IsReactingHash,false);

    }

    public override void InitializeSubState()
    {

    }

    public override void UpdateState()
    {
    }


}