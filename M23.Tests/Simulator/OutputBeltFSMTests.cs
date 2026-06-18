namespace M23.Tests.Simulator;

using M23.Simulator.Fsm;
using Xunit;



public class OutputBeltFSMTests
{
    [Fact]
    public void InitialState_IsStopped()
    {
        var fsm = new OutputBeltFSM("M3");
        Assert.Equal(OutputBeltState.Stopped, fsm.State);
    }

    
    [Fact]
    public void Start_FromStopped_TransitionsToRunning()
    {
        var fsm = new OutputBeltFSM("M3");
        fsm.Start();
        Assert.Equal(OutputBeltState.Running, fsm.State);
    }

    
    [Fact]
    public void Start_WhenAlreadyRunning_DoesNotFireEvent()
    {
        var fsm = new OutputBeltFSM("M3");
        fsm.Start();
        int eventCount = 0;
        fsm.StateChanged += (_, _, _) => eventCount++;
        fsm.Start();
        Assert.Equal(0, eventCount);
    }

    
    [Fact]
    public void Stop_FromRunning_TransitionsToStopped()
    {
        var fsm = new OutputBeltFSM("M3");
        fsm.Start();
        fsm.Stop();
        Assert.Equal(OutputBeltState.Stopped, fsm.State);
    }

    
    [Fact]
    public void Stop_WhenAlreadyStopped_DoesNotFireEvent()
    {
        var fsm = new OutputBeltFSM("M3");
        int eventCount = 0;
        fsm.StateChanged += (_, _, _) => eventCount++;
        fsm.Stop();
        Assert.Equal(0, eventCount);
    }

    
    [Fact]
    public void StateChanged_FiresWithCorrectArguments()
    {
        var fsm = new OutputBeltFSM("M3");
        string? name = null;
        OutputBeltState? from = null, to = null;
        fsm.StateChanged += (n, f, t) => { name = n; from = f; to = t; };
        fsm.Start();
        Assert.Equal("M3", name);
        Assert.Equal(OutputBeltState.Stopped, from);
        Assert.Equal(OutputBeltState.Running, to);
    }
}