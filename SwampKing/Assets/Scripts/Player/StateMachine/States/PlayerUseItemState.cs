using UnityEngine;

public class PlayerUseItemState : PlayerBaseState
{
    public PlayerUseItemState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory): base(currentContext, playerStateFactory) { }

    public override void EnterState() {
        //_ctx.PlayerMovement.StopMovement();
        //Moviemento lento
        Debug.Log("Player Use Item");
        QuickSlotManager.instance.HandleUseInput();
        
    }
    public override void UpdateState(){
        CheckSwitchStates();
    }
    public override void ExitState() { }
    
    public override void FixedUpdateState()
    {
        
    }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() {
        if(_ctx.PlayerManager.IsDead) SwitchState(_factory.Dead());
        else if(_ctx.PlayerManager.IsReacting) SwitchState(_factory.Reaction());
        else if(_ctx.PlayerManager.IsDrowned) SwitchState(_factory.Drown());
        
        if (_ctx.PlayerManager.IsChargingJumping) SwitchState(_factory.ChargeJump());
        else if (InputController.instance.CheckActions(InputController.InputActionType.Attack)) SwitchState(_factory.Attack());
        else if (InputController.instance.IsAimingPressed) SwitchState(_factory.Aimning());
        else if (InputController.instance.MoveAmount != 0) SwitchState(_factory.Walk());
        else SwitchState(_factory.Idle());

    }
}
