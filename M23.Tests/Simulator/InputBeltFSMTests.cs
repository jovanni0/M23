namespace M23.Tests.Simulator;

using M23.Simulator.Fsm;
using Xunit;



public class InputBeltFSMTests
{
    [Fact]
    public void InitialState_IsStopped()
    {
        var fsm = new InputBeltFSM("M1");
        Assert.Equal(BeltState.Stopped, fsm.State);
    }

    
    [Fact]
    public void Start_FromStopped_TransitionsToReady()
    {
        var fsm = new InputBeltFSM("M1");
        fsm.Start();
        Assert.Equal(BeltState.Ready, fsm.State);
    }

    
    [Fact]
    public void Start_WhenNotStopped_DoesNothing()
    {
        var fsm = new InputBeltFSM("M1");
        fsm.Start();
        fsm.Start();
        Assert.Equal(BeltState.Ready, fsm.State);
    }

    
    [Fact]
    public void ConditionMet_FromReady_TransitionsToRunning()
    {
        var fsm = new InputBeltFSM("M1");
        fsm.Start();
        fsm.ConditionMet();
        Assert.Equal(BeltState.Running, fsm.State);
    }

    
    [Fact]
    public void ConditionMet_WhenNotReady_DoesNothing()
    {
        var fsm = new InputBeltFSM("M1");
        fsm.ConditionMet();
        Assert.Equal(BeltState.Stopped, fsm.State);
    }

    
    [Fact]
    public void ConditionLost_FromRunning_TransitionsToReady()
    {
        var fsm = new InputBeltFSM("M1");
        fsm.Start();
        fsm.ConditionMet();
        fsm.ConditionLost();
        Assert.Equal(BeltState.Ready, fsm.State);
    }

    
    [Fact]
    public void ConditionLost_WhenNotRunning_DoesNothing()
    {
        var fsm = new InputBeltFSM("M1");
        fsm.Start();
        fsm.ConditionLost();
        Assert.Equal(BeltState.Ready, fsm.State);
    }

    
    [Fact]
    public void Stop_FromReady_TransitionsToStopped()
    {
        var fsm = new InputBeltFSM("M1");
        fsm.Start();
        fsm.Stop();
        Assert.Equal(BeltState.Stopped, fsm.State);
    }

    
    [Fact]
    public void Stop_FromRunning_TransitionsToStopped()
    {
        var fsm = new InputBeltFSM("M1");
        fsm.Start();
        fsm.ConditionMet();
        fsm.Stop();
        Assert.Equal(BeltState.Stopped, fsm.State);
    }

    
    [Fact]
    public void Stop_WhenAlreadyStopped_DoesNotFireEvent()
    {
        var fsm = new InputBeltFSM("M1");
        int eventCount = 0;
        fsm.StateChanged += (_, _, _) => eventCount++;
        fsm.Stop();
        Assert.Equal(0, eventCount);
    }

    
    [Fact]
    public void StateChanged_FiresWithCorrectArguments()
    {
        var fsm = new InputBeltFSM("M1");
        string? name = null;
        BeltState? from = null, to = null;
        fsm.StateChanged += (n, f, t) => { name = n; from = f; to = t; };
        fsm.Start();
        Assert.Equal("M1", name);
        Assert.Equal(BeltState.Stopped, from);
        Assert.Equal(BeltState.Ready, to);
    }
}