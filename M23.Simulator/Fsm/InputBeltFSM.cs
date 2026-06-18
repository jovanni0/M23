namespace M23.Simulator.Fsm;



public enum BeltState
{
    Stopped,
    Ready,
    Running
}



public class InputBeltFSM
{
    public string Name { get; }
    public BeltState State { get; private set; } = BeltState.Stopped;

    public event Action<string, BeltState, BeltState>? StateChanged;

    
    public InputBeltFSM(string name)
    {
        Name = name;
    }

    
    public void Start()
    {
        if (State != BeltState.Stopped) 
            return;

        Transition(BeltState.Ready);
    }

    
    public void Stop()
    {
        if (State == BeltState.Stopped) 
            return;

        Transition(BeltState.Stopped);
    }

    
    /*
     * Used when the condition the belt was waiting for has been met and the belt
     * can transition to the Running state.
     */
    public void ConditionMet()
    {
        if (State != BeltState.Ready) 
            return;

        Transition(BeltState.Running);
    }

    
    /*
     * Used when the belt loses it's run condition. It will transition to the Ready state.
     */
    public void ConditionLost()
    {
        if (State != BeltState.Running) 
            return;

        Transition(BeltState.Ready);
    }

    
    /*
     * Transitions the belt to a new state.
     */
    private void Transition(BeltState next)
    {
        var previous = State;
        State = next;
        StateChanged?.Invoke(Name, previous, State);
    }
}