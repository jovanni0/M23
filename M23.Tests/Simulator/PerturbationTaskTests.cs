namespace M23.Tests.Simulator;

using M23.Simulator;
using M23.Simulator.Fsm;
using Xunit;



public class PerturbationTaskTests
{
    [Fact]
    public async Task Perturbation_WhenSystemNormal_TriggersFault()
    {
        var orchestrator = new ProcessOrchestrator();
        orchestrator.StartBelt("M3");
        Assert.Equal(SystemState.Normal, orchestrator.System.State);

        var task = new PerturbationTask(orchestrator, minDelaySeconds: 0, maxDelaySeconds: 1);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        await task.RunAsync(cts.Token).ContinueWith(_ => { });

        Assert.Equal(SystemState.Fault, orchestrator.System.State);
    }

    
    [Fact]
    public async Task Perturbation_WhenSystemIdle_DoesNotTriggerFault()
    {
        var orchestrator = new ProcessOrchestrator();
        Assert.Equal(SystemState.Idle, orchestrator.System.State);

        var task = new PerturbationTask(orchestrator, minDelaySeconds: 0, maxDelaySeconds: 1);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        await task.RunAsync(cts.Token).ContinueWith(_ => { });

        Assert.Equal(SystemState.Idle, orchestrator.System.State);
    }
}