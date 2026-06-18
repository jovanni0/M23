namespace M23.Simulator.Fsm;



public enum FlapPosition
{
    S6,
    S7,
    S8,
    Fault
}



public class FlapFSM
{
    public FlapPosition Position { get; private set; } = FlapPosition.S7;

    public event Action<FlapPosition, FlapPosition>? StateChanged;

    
    public void MoveTo(FlapPosition target)
    {
        if (Position == FlapPosition.Fault)
            throw new InvalidOperationException("Cannot move flap while in fault state.");

        if (target == FlapPosition.Fault)
            throw new ArgumentException("Use TriggerFault() to enter fault state.");

        if (target == Position) 
            return;

        var previous = Position;
        Position = target;
        StateChanged?.Invoke(previous, Position);
    }

    
    public void TriggerFault()
    {
        if (Position == FlapPosition.Fault) 
            return;

        var previous = Position;
        Position = FlapPosition.Fault;
        StateChanged?.Invoke(previous, Position);
    }

    
    public void Reset(FlapPosition position = FlapPosition.S7)
    {
        if (position == FlapPosition.Fault)
            throw new ArgumentException("Cannot reset to fault state.");

        var previous = Position;
        Position = position;

        if (previous != Position)
            StateChanged?.Invoke(previous, Position);
    }
}