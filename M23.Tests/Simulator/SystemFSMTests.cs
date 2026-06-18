using M23.Simulator.Fsm;

namespace M23.Tests.Simulator;



public class SystemFSMTests
{
    [Fact]
    public void InitialState_IsIdle()
    {
        var fsm = new SystemFSM();
        Assert.Equal(SystemState.Idle, fsm.State);
    }

    
    [Fact]
    public void BeltStarted_FromIdle_TransitionsToNormal()
    {
        var fsm = new SystemFSM();
        fsm.BeltStarted();
        Assert.Equal(SystemState.Normal, fsm.State);
    }

    
    [Fact]
    public void BeltStarted_WhenNotIdle_DoesNothing()
    {
        var fsm = new SystemFSM();
        fsm.BeltStarted();
        fsm.BeltStarted();
        Assert.Equal(SystemState.Normal, fsm.State);
    }

    
    [Fact]
    public void AllBeltsStopped_FromNormal_TransitionsToIdle()
    {
        var fsm = new SystemFSM();
        fsm.BeltStarted();
        fsm.AllBeltsStopped();
        Assert.Equal(SystemState.Idle, fsm.State);
    }
    

    [Fact]
    public void TriggerFault_FromNormal_TransitionsToFault()
    {
        var fsm = new SystemFSM();
        fsm.BeltStarted();
        fsm.TriggerFault();
        Assert.Equal(SystemState.Fault, fsm.State);
    }

    
    [Fact]
    public void TriggerFault_FromIdle_DoesNothing()
    {
        var fsm = new SystemFSM();
        fsm.TriggerFault();
        Assert.Equal(SystemState.Idle, fsm.State);
    }

    
    [Fact]
    public void Restart_FromFault_TransitionsToIdle()
    {
        var fsm = new SystemFSM();
        fsm.BeltStarted();
        fsm.TriggerFault();
        fsm.Restart();
        Assert.Equal(SystemState.Idle, fsm.State);
    }

    
    [Fact]
    public void Restart_WhenNotFault_DoesNothing()
    {
        var fsm = new SystemFSM();
        fsm.BeltStarted();
        fsm.Restart();
        Assert.Equal(SystemState.Normal, fsm.State);
    }
}