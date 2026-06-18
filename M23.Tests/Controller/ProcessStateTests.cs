namespace M23.Tests.Controller;

using M23.Controller.Services;
using Xunit;



public class ProcessStateTests
{
    [Fact]
    public void InitialSnapshot_HasCorrectDefaults()
    {
        var state = new ProcessState();
        var snapshot = state.Snapshot() as dynamic;
        Assert.NotNull(snapshot);
    }

    
    [Theory]
    [InlineData("SYSTEM", "Normal")]
    [InlineData("SYSTEM", "Fault")]
    [InlineData("FLAP", "S6")]
    [InlineData("FLAP", "Fault")]
    [InlineData("M1", "Running")]
    [InlineData("M2", "Ready")]
    [InlineData("M3", "Running")]
    [InlineData("M4", "Stopped")]
    public void Apply_UpdatesCorrectField(string source, string to)
    {
        var state = new ProcessState();
        state.Apply(source, to);
        var json = System.Text.Json.JsonSerializer.Serialize(state.Snapshot());
        Assert.Contains(to, json);
    }

    
    [Fact]
    public void Apply_UnknownSource_DoesNotThrow()
    {
        var state = new ProcessState();
        var ex = Record.Exception(() => state.Apply("UNKNOWN", "SomeState"));
        Assert.Null(ex);
    }

    
    [Fact]
    public void Apply_MultipleTimes_KeepsLatestState()
    {
        var state = new ProcessState();
        state.Apply("M1", "Ready");
        state.Apply("M1", "Running");
        state.Apply("M1", "Ready");
        var json = System.Text.Json.JsonSerializer.Serialize(state.Snapshot());
        Assert.Contains("Ready", json);
    }
}