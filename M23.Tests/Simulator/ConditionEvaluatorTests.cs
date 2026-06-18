using M23.Simulator.Fsm;

namespace M23.Tests.Simulator;



public class ConditionEvaluatorTests
{
    [Theory]
    [InlineData("M1", FlapPosition.S6, OutputBeltState.Stopped, OutputBeltState.Running, BeltState.Stopped, true)]
    [InlineData("M1", FlapPosition.S6, OutputBeltState.Running, OutputBeltState.Running, BeltState.Stopped, true)]
    [InlineData("M1", FlapPosition.S6, OutputBeltState.Running, OutputBeltState.Running, BeltState.Running, false)]
    [InlineData("M2", FlapPosition.S6, OutputBeltState.Running, OutputBeltState.Running, BeltState.Stopped, true)]
    [InlineData("M2", FlapPosition.S6, OutputBeltState.Running, OutputBeltState.Running, BeltState.Running, false)]
    [InlineData("M1", FlapPosition.S7, OutputBeltState.Running, OutputBeltState.Stopped, BeltState.Stopped, true)]
    [InlineData("M1", FlapPosition.S7, OutputBeltState.Stopped, OutputBeltState.Running, BeltState.Stopped, false)]
    [InlineData("M2", FlapPosition.S7, OutputBeltState.Stopped, OutputBeltState.Running, BeltState.Stopped, true)]
    [InlineData("M2", FlapPosition.S7, OutputBeltState.Running, OutputBeltState.Stopped, BeltState.Stopped, false)]
    [InlineData("M1", FlapPosition.S7, OutputBeltState.Running, OutputBeltState.Running, BeltState.Running, true)]
    [InlineData("M2", FlapPosition.S7, OutputBeltState.Running, OutputBeltState.Running, BeltState.Running, true)]
    [InlineData("M1", FlapPosition.S8, OutputBeltState.Running, OutputBeltState.Stopped, BeltState.Stopped, true)]
    [InlineData("M1", FlapPosition.S8, OutputBeltState.Running, OutputBeltState.Stopped, BeltState.Running, false)]
    [InlineData("M2", FlapPosition.S8, OutputBeltState.Running, OutputBeltState.Stopped, BeltState.Stopped, true)]
    [InlineData("M2", FlapPosition.S8, OutputBeltState.Running, OutputBeltState.Stopped, BeltState.Running, false)]
    [InlineData("M1", FlapPosition.Fault, OutputBeltState.Running, OutputBeltState.Running, BeltState.Stopped, false)]
    public void CanRun_ReturnsExpected(
        string belt,
        FlapPosition flap,
        OutputBeltState m3,
        OutputBeltState m4,
        BeltState other,
        bool expected)
    {
        Assert.Equal(expected, ConditionEvaluator.CanRun(belt, flap, m3, m4, other));
    }
}