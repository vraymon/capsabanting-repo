using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct State
{
    public Action OnEnter;
    public Action OnUpdate;
    public Action OnLateUpdate;
    public Action OnExit;

    public State(Action Enter, Action Update, Action LateUpdate, Action Exit)
    {
        OnEnter = Enter;
        OnUpdate = Update;
        OnLateUpdate = LateUpdate;
        OnExit = Exit;
    }
}

public class FSM<T>
{
    Dictionary<T, State> States;

    public T currentstate;
    public float StateElapseTime = 0;
    public bool StateInitialized = false;
    public void Initialize(Dictionary<T,State> stateList)
    {
        States = stateList;
        currentstate = default(T);
        StateElapseTime = 0;
    }
    public void Update()
    {
        StateElapseTime += Time.deltaTime;
        ValidateState(States[currentstate].OnUpdate);
    }

    public void LateUpdate()
    {
        ValidateState(States[currentstate].OnLateUpdate);
    }

    public void ChangeState(T state)
    {
        T newState = state;

        if(!newState.Equals(currentstate) || !StateInitialized)
            if(StateInitialized)
                ValidateState(States[currentstate].OnExit);

        currentstate = newState;
        ValidateState(States[currentstate].OnEnter);
        StateElapseTime = 0;
        StateInitialized = true;
    }

    void ValidateState(Action action)
    {
        if (action != null)
            action();
    }
}
