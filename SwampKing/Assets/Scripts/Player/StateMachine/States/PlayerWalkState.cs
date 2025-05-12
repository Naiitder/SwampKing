
public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory): base(currentContext, playerStateFactory) { }

    public override void EnterState() { }
    public override void UpdateState(){
        _ctx.PlayerMovement.HandleGroundedMovement();
        _ctx.PlayerMovement.HandleRotation();


        CheckSwitchStates();
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    
    public override void FixedUpdateState()
    {
        
    }
    
    public override void CheckSwitchStates() {
        if(_ctx.PlayerManager.IsDead) SwitchState(_factory.Dead());
        else if(_ctx.PlayerManager.IsReacting) SwitchState(_factory.Reaction());
        
        if (_ctx.PlayerManager.IsChargingJumping) SwitchState(_factory.ChargeJump());
        else if (InputController.instance.CheckActions(InputController.InputActionType.Attack)) SwitchState(_factory.Attack());
        else if (InputController.instance.CheckActions(InputController.InputActionType.Interact)) SwitchState(_factory.Interact());
        else if (InputController.instance.IsAimingPressed) SwitchState(_factory.Aimning());
        else if (InputController.instance.MoveAmount == 0) SwitchState(_factory.Idle()) ;
    }
}
