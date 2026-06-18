namespace M23.Tests.Simulator;

using M23.Simulator.Fsm;
using Xunit;



public class FlapFSMTests
{
    [Fact]
    public void InitialState_IsS7()
    {
        var fsm = new FlapFSM();
        Assert.Equal(FlapPosition.S7, fsm.Position);
    }

    
    [Theory]
    [InlineData(FlapPosition.S7, FlapPosition.S6)]
    [InlineData(FlapPosition.S7, FlapPosition.S8)]
    [InlineData(FlapPosition.S6, FlapPosition.S7)]
    [InlineData(FlapPosition.S6, FlapPosition.S8)]
    [InlineData(FlapPosition.S8, FlapPosition.S7)]
    [InlineData(FlapPosition.S8, FlapPosition.S6)]
    public void MoveTo_ValidTransition_ChangesState(FlapPosition from, FlapPosition to)
    {
        var fsm = new FlapFSM();
        fsm.Reset(from);
        fsm.MoveTo(to);
        Assert.Equal(to, fsm.Position);
    }

    
    [Fact]
    public void MoveTo_SamePosition_DoesNotFireEvent()
    {
        var fsm = new FlapFSM();
        int eventCount = 0;
        fsm.StateChanged += (_, _) => eventCount++;
        fsm.MoveTo(FlapPosition.S7);
        Assert.Equal(0, eventCount);
    }

    
    [Fact]
    public void MoveTo_WhileInFault_Throws()
    {
        var fsm = new FlapFSM();
        fsm.TriggerFault();
        Assert.Throws<InvalidOperationException>(() => fsm.MoveTo(FlapPosition.S6));
    }

    
    [Fact]
    public void MoveTo_FaultDirectly_Throws()
    {
        var fsm = new FlapFSM();
        Assert.Throws<ArgumentException>(() => fsm.MoveTo(FlapPosition.Fault));
    }
    

    [Fact]
    public void TriggerFault_TransitionsToFault()
    {
        var fsm = new FlapFSM();
        fsm.TriggerFault();
        Assert.Equal(FlapPosition.Fault, fsm.Position);
    }

    
    [Fact]
    public void TriggerFault_WhenAlreadyFault_DoesNotFireEvent()
    {
        var fsm = new FlapFSM();
        fsm.TriggerFault();
        int eventCount = 0;
        fsm.StateChanged += (_, _) => eventCount++;
        fsm.TriggerFault();
        Assert.Equal(0, eventCount);
    }

    
    [Fact]
    public void TriggerFault_FiresEventWithCorrectStates()
    {
        var fsm = new FlapFSM();
        FlapPosition? from = null, to = null;
        fsm.StateChanged += (f, t) => { from = f; to = t; };
        fsm.TriggerFault();
        Assert.Equal(FlapPosition.S7, from);
        Assert.Equal(FlapPosition.Fault, to);
    }

    
    [Fact]
    public void Reset_FromFault_RestoresPosition()
    {
        var fsm = new FlapFSM();
        fsm.TriggerFault();
        fsm.Reset(FlapPosition.S6);
        Assert.Equal(FlapPosition.S6, fsm.Position);
    }

    
    [Fact]
    public void Reset_ToFault_Throws()
    {
        var fsm = new FlapFSM();
        Assert.Throws<ArgumentException>(() => fsm.Reset(FlapPosition.Fault));
    }
}