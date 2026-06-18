namespace M23.Simulator.Fsm;



public enum OutputBeltState
{
    Stopped,
    Running
}



public class OutputBeltFSM
{
    public string Name { get; }
    public OutputBeltState State { get; private set; } = OutputBeltState.Stopped;

    public event Action<string, OutputBeltState, OutputBeltState>? StateChanged;

    
    public OutputBeltFSM(string name)
    {
        Name = name;
    }

    
    public void Start()
    {
        if (State != OutputBeltState.Stopped) 
            return;

        var previous = State;
        State = OutputBeltState.Running;
        StateChanged?.Invoke(Name, previous, State);
    }

    
    public void Stop()
    {
        if (State != OutputBeltState.Running) 
            return;

        var previous = State;
        State = OutputBeltState.Stopped;
        StateChanged?.Invoke(Name, previous, State);
    }
}