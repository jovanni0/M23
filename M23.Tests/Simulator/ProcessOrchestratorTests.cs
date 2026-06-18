using M23.Simulator;
using M23.Simulator.Fsm;

namespace M23.Tests.Simulator;



public class ProcessOrchestratorTests
{
    [Fact]
    public void StartOutputBelt_ThenInputBelt_InputTransitionsToRunning()
    {
        var o = new ProcessOrchestrator();
        o.StartBelt("M3");
        o.StartBelt("M1");
        Assert.Equal(BeltState.Running, o.M1.State);
    }

    
    [Fact]
    public void StartInputBelt_WithoutOutputBelt_InputStaysReady()
    {
        var o = new ProcessOrchestrator();
        o.StartBelt("M1");
        Assert.Equal(BeltState.Ready, o.M1.State);
    }

    
    [Fact]
    public void StartInputBelt_ThenOutputBelt_InputTransitionsToRunning()
    {
        var o = new ProcessOrchestrator();
        o.StartBelt("M1");
        o.StartBelt("M3");
        Assert.Equal(BeltState.Running, o.M1.State);
    }

    
    [Fact]
    public void StopOutputBelt_WhileInputRunning_InputDropsToReady()
    {
        var o = new ProcessOrchestrator();
        o.StartBelt("M3");
        o.StartBelt("M1");
        o.StopBelt("M3");
        Assert.Equal(BeltState.Ready, o.M1.State);
    }

    
    [Fact]
    public void S0_StopsAllBelts()
    {
        var o = new ProcessOrchestrator();
        o.StartBelt("M3");
        o.StartBelt("M4");
        o.StartBelt("M1");
        o.StartBelt("M2");
        o.StopAllBelts();
        Assert.Equal(BeltState.Stopped, o.M1.State);
        Assert.Equal(BeltState.Stopped, o.M2.State);
        Assert.Equal(OutputBeltState.Stopped, o.M3.State);
        Assert.Equal(OutputBeltState.Stopped, o.M4.State);
    }

    
    [Fact]
    public void S5_StopsOnlyInputBelts()
    {
        var o = new ProcessOrchestrator();
        o.StartBelt("M3");
        o.StartBelt("M4");
        o.StartBelt("M1");
        o.StartBelt("M2");
        o.StopInputBelts();
        Assert.Equal(BeltState.Stopped, o.M1.State);
        Assert.Equal(BeltState.Stopped, o.M2.State);
        Assert.Equal(OutputBeltState.Running, o.M3.State);
        Assert.Equal(OutputBeltState.Running, o.M4.State);
    }

    
    [Fact]
    public void TriggerFault_StopsAllBeltsAndSystemGoesToFault()
    {
        var o = new ProcessOrchestrator();
        o.StartBelt("M3");
        o.StartBelt("M1");
        o.TriggerFault();
        Assert.Equal(SystemState.Fault, o.System.State);
        Assert.Equal(BeltState.Stopped, o.M1.State);
        Assert.Equal(OutputBeltState.Stopped, o.M3.State);
    }

    
    [Fact]
    public void Restart_FromFault_SystemReturnsToIdle()
    {
        var o = new ProcessOrchestrator();
        o.StartBelt("M3");
        o.StartBelt("M1");
        o.TriggerFault();
        o.Restart();
        Assert.Equal(SystemState.Idle, o.System.State);
    }

    
    [Fact]
    public void MoveFlap_WhileInputRunning_ReevaluatesConditions()
    {
        var o = new ProcessOrchestrator();
        o.StartBelt("M3");
        o.StartBelt("M1");
        Assert.Equal(BeltState.Running, o.M1.State);
        o.MoveFlap(FlapPosition.S6);
        Assert.Equal(BeltState.Ready, o.M1.State);
    }

    
    [Fact]
    public void S7_BothInputBelts_CanRunSimultaneously()
    {
        var o = new ProcessOrchestrator();
        o.StartBelt("M3");
        o.StartBelt("M4");
        o.StartBelt("M1");
        o.StartBelt("M2");
        Assert.Equal(BeltState.Running, o.M1.State);
        Assert.Equal(BeltState.Running, o.M2.State);
    }

    
    [Fact]
    public void EventOccurred_FiresOnEveryTransition()
    {
        var o = new ProcessOrchestrator();
        var events = new List<ProcessEvent>();
        o.EventOccurred += events.Add;
        o.StartBelt("M3");
        o.StartBelt("M1");
        Assert.NotEmpty(events);
    }

    
    [Fact]
    public void SystemTransitionsToNormal_WhenFirstBeltStarts()
    {
        var o = new ProcessOrchestrator();
        o.StartBelt("M3");
        Assert.Equal(SystemState.Normal, o.System.State);
    }

    
    [Fact]
    public void SystemTransitionsToIdle_WhenLastBeltStops()
    {
        var o = new ProcessOrchestrator();
        o.StartBelt("M3");
        o.StopBelt("M3");
        Assert.Equal(SystemState.Idle, o.System.State);
    }
}