
public class PlayerFallingState : PlayerBaseState
{
    public PlayerFallingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {
    }
    public override void EnterState()
    {
    }
    public override void UpdateState()
    {
        if (!_ctx.PlayerAnimator.Animator.GetBool(_ctx.PlayerAnimator.IsFallingHash) && _ctx.PlayerManager.InAirTimer > _ctx.PlayerManager.CoyoteTime) 
            _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.IsFallingHash, true);
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.IsFallingHash, false);
    }
    public override void InitializeSubState()
    {
    }
    
    public override void FixedUpdateState()
    {
        
    }
    
    public override void CheckSwitchStates()
    {

        if (InputController.instance.CheckActions(InputController.InputActionType.Attack)) SwitchState(_factory.JumpAttack());

        if (_ctx.PlayerMovement.isGrounded()) 
            SwitchState(_factory.Grounded());

        if (_ctx.PlayerManager.InAirTimer <= _ctx.PlayerManager.CoyoteTime
            && InputController.instance.CheckActions(InputController.InputActionType.Jump)) 
            SwitchState(_factory.Jump());
        else if (_ctx.PlayerManager.InAirTimer > _ctx.PlayerManager.CoyoteTime
            && InputController.instance.CheckActions(InputController.InputActionType.Jump)
            && _ctx.PlayerManager.CanDoubleJump) 
            SwitchState(_factory.DoubleJump());
    }


}
