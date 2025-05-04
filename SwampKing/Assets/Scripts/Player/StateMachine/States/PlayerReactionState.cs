using Unity.VisualScripting;
using UnityEngine;

public class PlayerReactionState : PlayerBaseState
{
    bool hasReacted = false;

    public PlayerReactionState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }
    
    public override void CheckSwitchStates()
    {
        if (_ctx.PlayerManager.IsDead) SwitchState(_factory.Dead());
        
        if (!hasReacted) return;
        
        if (InputController.instance.CheckActions(InputController.InputActionType.Attack)) SwitchState(_factory.Attack());
        else if (InputController.instance.CheckActions(InputController.InputActionType.Interact)) SwitchState(_factory.Interact());
        else if (InputController.instance.IsAimingPressed) SwitchState(_factory.Aimning());

        else if (InputController.instance.MoveAmount != 0) SwitchState(_factory.Walk());
        else SwitchState(_factory.Idle());

    }

    public override void EnterState()
    {
        hasReacted = false;
        _ctx.PlayerMovement.StopMovement();
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.IsReactingHash,true);
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.ReactionFinishedHash, false);
        
    }


    public override void ExitState()
    {
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.IsReactingHash,false);
        _ctx.PlayerManager.IsReacting = false;
    }

    public override void InitializeSubState()
    {

    }

    public override void UpdateState()
    {
        if (_ctx.PlayerAnimator.Animator.GetBool(_ctx.PlayerAnimator.ReactionFinishedHash))
        {
            hasReacted = true;
        }

        
        CheckSwitchStates();
    }


}