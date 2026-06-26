using M23.Simulator.Fsm;

namespace M23.Simulator;



public class PerturbationTask
{
    private readonly ProcessOrchestrator _orchestrator;
    private readonly Random _random = new();
    private readonly int _minDelaySeconds;
    private readonly int _maxDelaySeconds;

    public bool Enabled { get; set; } = true;

    
    public PerturbationTask(
        ProcessOrchestrator orchestrator,
        int minDelaySeconds = 15,
        int maxDelaySeconds = 45)
    {
        _orchestrator = orchestrator;
        _minDelaySeconds = minDelaySeconds;
        _maxDelaySeconds = maxDelaySeconds;
    }

    
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var delay = _random.Next(_minDelaySeconds, _maxDelaySeconds);
            await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);

            if (cancellationToken.IsCancellationRequested) break;

            if (!Enabled)
                continue;

            if (_orchestrator.System.State == SystemState.Normal)
            {
                Console.WriteLine("[Perturbation] Triggering sensor fault.");
                _orchestrator.TriggerFault();
            }
        }
    }
}