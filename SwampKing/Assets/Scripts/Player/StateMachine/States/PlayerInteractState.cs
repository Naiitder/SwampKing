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
    public override void CheckSwitchStates() {
        
        var npc = FindNPC(); 
        if (npc != null)
        {
            if (DialogueManager.instance.dialogueBox.activeSelf) DialogueManager.instance.SkipOrNext();
            else npc.TriggerDialogue();
        }
        else SwitchState(_factory.UsingItem());
        
        if (_ctx.PlayerManager.IsChargingJumping) SwitchState(_factory.ChargeJump());
        else if (InputController.instance.CheckActions(InputController.InputActionType.Attack)) SwitchState(_factory.Attack());
        else if (InputController.instance.MoveAmount != 0) SwitchState(_factory.Walk());
        else if (InputController.instance.MoveAmount == 0) SwitchState(_factory.Idle());
    }

    private NPCDialogueTrigger FindNPC()
    {
        float interactionRadius = 2f;
        Vector3 center = _ctx.transform.position + Vector3.up;

        Collider[] colliders = Physics.OverlapSphere(center, interactionRadius);
        foreach (Collider collider in colliders)
        {
            NPCDialogueTrigger npc = collider.GetComponent<NPCDialogueTrigger>();
            if (npc != null)
            {
                return npc;
            }
        }

        return null;
    }
}
