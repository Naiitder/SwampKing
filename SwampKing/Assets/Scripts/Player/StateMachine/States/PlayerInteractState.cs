using UnityEngine;

public class PlayerInteractState : PlayerBaseState
{
    public PlayerInteractState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory): base(currentContext, playerStateFactory) { }

    public override void EnterState() {
        _ctx.PlayerMovement.StopMovement();
        
        InputController.instance.InputBuffer.Dequeue();
    }
    public override void UpdateState(){
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
        
        if (_ctx.npc != null)
        {
            if (DialogueManager.instance.dialogueBox.activeSelf) DialogueManager.instance.SkipOrNext();
            else _ctx.npc.TriggerDialogue();
        }
        else SwitchState(_factory.UsingItem());
        
        if (_ctx.PlayerManager.IsChargingJumping) SwitchState(_factory.ChargeJump());
        else if (InputController.instance.CheckActions(InputController.InputActionType.Attack)) SwitchState(_factory.Attack());
        else if (InputController.instance.IsAimingPressed) SwitchState(_factory.Aimning());
        else if (InputController.instance.MoveAmount != 0) SwitchState(_factory.Walk());
        else if (InputController.instance.MoveAmount == 0) SwitchState(_factory.Idle());


    }
}
