using M23.Simulator.Fsm;

namespace M23.Simulator;



public record ProcessEvent(
    string Source,
    string From,
    string To,
    DateTime Timestamp);



public class ProcessOrchestrator
{
    public FlapFSM Flap { get; } = new();
    public InputBeltFSM M1 { get; } = new("M1");
    public InputBeltFSM M2 { get; } = new("M2");
    public OutputBeltFSM M3 { get; } = new("M3");
    public OutputBeltFSM M4 { get; } = new("M4");
    public SystemFSM System { get; } = new();

    public event Action<ProcessEvent>? EventOccurred;

    
    public ProcessOrchestrator()
    {
        Flap.StateChanged += OnFlapChanged;
        M1.StateChanged += OnInputBeltChanged;
        M2.StateChanged += OnInputBeltChanged;
        M3.StateChanged += OnOutputBeltChanged;
        M4.StateChanged += OnOutputBeltChanged;
        System.StateChanged += OnSystemChanged;
    }

    
    public void StartBelt(string belt)
    {
        switch (belt)
        {
            case "M1": M1.Start(); break;
            case "M2": M2.Start(); break;
            case "M3": M3.Start(); break;
            case "M4": M4.Start(); break;
        }
        ReevaluateInputBelts();
        UpdateSystemState();
    }

    
    public void StopBelt(string belt)
    {
        switch (belt)
        {
            case "M1": M1.Stop(); break;
            case "M2": M2.Stop(); break;
            case "M3": M3.Stop(); break;
            case "M4": M4.Stop(); break;
        }
        ReevaluateInputBelts();
        UpdateSystemState();
    }

    
    public void StopAllBelts()
    {
        M1.Stop();
        M2.Stop();
        M3.Stop();
        M4.Stop();
        UpdateSystemState();
    }

    
    public void StopInputBelts()
    {
        M1.Stop();
        M2.Stop();
        UpdateSystemState();
    }

    
    public void MoveFlap(FlapPosition position)
    {
        Flap.MoveTo(position);
        ReevaluateInputBelts();
    }

    
    public void TriggerFault()
    {
        Flap.TriggerFault();
        M1.Stop();
        M2.Stop();
        M3.Stop();
        M4.Stop();
        System.TriggerFault();
    }

    
    public void Restart(FlapPosition flapPosition = FlapPosition.S7)
    {
        Flap.Reset(flapPosition);
        System.Restart();
    }

    
    private void ReevaluateInputBelts()
    {
        if (Flap.Position == FlapPosition.Fault) 
            return;

        EvaluateBelt(M1, M2);
        EvaluateBelt(M2, M1);
    }

    
    /*
     * Chech if the belt can keep running in the current system setup.
     */
    private void EvaluateBelt(InputBeltFSM belt, InputBeltFSM other)
    {
        if (belt.State == BeltState.Stopped) 
            return;

        bool canRun = ConditionEvaluator.CanRun(
            belt.Name,
            Flap.Position,
            M3.State,
            M4.State,
            other.State);

        if (canRun && belt.State == BeltState.Ready)
            belt.ConditionMet();
        else if (!canRun && belt.State == BeltState.Running)
            belt.ConditionLost();
    }

    
    /*
     * Update the system state based on the component's states.
     */
    private void UpdateSystemState()
    {
        bool anyRunning =
            M1.State != BeltState.Stopped ||
            M2.State != BeltState.Stopped ||
            M3.State != OutputBeltState.Stopped ||
            M4.State != OutputBeltState.Stopped;

        if (anyRunning && System.State == SystemState.Idle)
            System.BeltStarted();
        else if (!anyRunning && System.State == SystemState.Normal)
            System.AllBeltsStopped();
    }

    
    private void OnFlapChanged(FlapPosition from, FlapPosition to) =>
        Emit("FLAP", from.ToString(), to.ToString());

    
    private void OnInputBeltChanged(string name, BeltState from, BeltState to) =>
        Emit(name, from.ToString(), to.ToString());

    
    private void OnOutputBeltChanged(string name, OutputBeltState from, OutputBeltState to) =>
        Emit(name, from.ToString(), to.ToString());

    
    private void OnSystemChanged(SystemState from, SystemState to) =>
        Emit("SYSTEM", from.ToString(), to.ToString());

    
    private void Emit(string source, string from, string to) =>
        EventOccurred?.Invoke(new ProcessEvent(source, from, to, DateTime.UtcNow));
}