namespace M23.Simulator.Fsm;



public enum SystemState
{
    Idle,
    Normal,
    Fault
}



public class SystemFSM
{
    public SystemState State { get; private set; } = SystemState.Idle;

    public event Action<SystemState, SystemState>? StateChanged;

    
    /*
     * Transitions the system to the Normal state when a belt has started.
     */
    public void BeltStarted()
    {
        if (State != SystemState.Idle) 
            return;

        Transition(SystemState.Normal);
    }

    
    /*
     * Transitions the system when all belts have been stopped.
     */
    public void AllBeltsStopped()
    {
        if (State != SystemState.Normal) 
            return;

        Transition(SystemState.Idle);
    }

    
    /*
     * Transitions the system when a fault is detected.
     */
    public void TriggerFault()
    {
        if (State != SystemState.Normal) 
            return;

        Transition(SystemState.Fault);
    }

    
    public void Restart()
    {
        if (State != SystemState.Fault) 
            return;

        Transition(SystemState.Idle);
    }

    
    /*
     * Transitions the system to another state. 
     */
    private void Transition(SystemState next)
    {
        var previous = State;
        State = next;
        StateChanged?.Invoke(previous, State);
    }
}