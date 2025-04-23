using UnityEngine;

public class PlayerUseItemState : PlayerBaseState
{
    public PlayerUseItemState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory): base(currentContext, playerStateFactory) { }

    public override void EnterState() {
        _ctx.PlayerMovement.StopMovement();
        
    }
    public override void UpdateState(){
        CheckSwitchStates();
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() {
        if(_ctx.PlayerManager.IsDead) SwitchState(_factory.Dead());
        else if(_ctx.PlayerManager.IsReacting) SwitchState(_factory.Reaction());
        
        if (_ctx.PlayerManager.IsChargingJumping) SwitchState(_factory.ChargeJump());
        else if (InputController.instance.MoveAmount != 0) SwitchState(_factory.Walk());
        else if (InputController.instance.CheckActions(InputController.InputActionType.Attack)) SwitchState(_factory.Attack());
    }
}
