using UnityEngine;

public abstract class State
{
    public abstract void Enter();
    public abstract void Tick();
    public abstract void Exit();

    public virtual void OnTriggerEnter(Collider collider) { }
}

public abstract class StateMachine : MonoBehaviour
{
    protected State currentState;
    public State GetCurrentState() { return currentState; }

    //On changing states, exit old state, set new state & enter it
    public void SwitchState(State state)
    {
        currentState?.Exit();
        currentState = state;
        currentState.Enter();
        //Debug.Log("Entered New State : " + currentState.GetType().Name);
    }

    //call Tick in current state to update independently of state
    private void Update()
    {
        currentState?.Tick();
    }
}