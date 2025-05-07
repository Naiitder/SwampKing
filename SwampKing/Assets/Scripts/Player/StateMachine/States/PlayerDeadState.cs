using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeadState : PlayerBaseState
{

    private float timeOfDeath;
    public PlayerDeadState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }
    
    public override void CheckSwitchStates()
    {
    }

    public override void EnterState()
    {
        _ctx.PlayerMovement.StopMovement();
        _ctx.PlayerAnimator.Animator.SetBool(_ctx.PlayerAnimator.IsDeadHash,true);
        timeOfDeath = Time.time;
            
    }
    
    public override void FixedUpdateState()
    {
        
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {

    }

    public override void UpdateState()
    {
        if (Time.time - timeOfDeath >= 3f)
        {
            LevelManager.instance.LoadScene(SceneManager.GetActiveScene().name);
        }
    }


}
