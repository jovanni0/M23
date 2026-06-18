using M23.Simulator;



var orchestrator = new ProcessOrchestrator();
var server = new TcpServer(orchestrator);
var perturbation = new PerturbationTask(orchestrator);

orchestrator.EventOccurred += e =>
    Console.WriteLine($"[{e.Timestamp:HH:mm:ss}] {e.Source}: {e.From} → {e.To}");

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

await Task.WhenAll(
    server.StartAsync(5000, cts.Token),
    perturbation.RunAsync(cts.Token)
);

Console.WriteLine("Simulator stopped.");