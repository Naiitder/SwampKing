using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState {
    void OnEnter();
    void Update();
    void FixedUpdate();
    void OnExit();
}

public abstract class BaseState : IState
{
    //protected readonly PlayerController player;
    protected readonly Animator anim;
    public virtual void FixedUpdate()
    {
        //noop
    }
    public virtual void OnEnter()
    {
        //noop
    }
    public virtual void OnExit()
    {
        //noop
    }
    public virtual void Update()
    {
        //noop
    }
}