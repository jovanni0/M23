namespace M23.Controller.Services;



public class ProcessState
{
    private readonly object _lock = new();

    public string System { get; private set; } = "Idle";
    public string Flap { get; private set; } = "S7";
    public Dictionary<string, string> Belts { get; private set; } = new()
    {
        { "M1", "Stopped" },
        { "M2", "Stopped" },
        { "M3", "Stopped" },
        { "M4", "Stopped" }
    };

    
    public void Apply(string source, string to)
    {
        lock (_lock)
        {
            switch (source)
            {
                case "SYSTEM": System = to; break;
                case "FLAP":   Flap = to;   break;
                case "M1" or "M2" or "M3" or "M4":
                    Belts[source] = to;
                    break;
            }
        }
    }

    
    public object Snapshot()
    {
        lock (_lock)
        {
            return new
            {
                system = System,
                flap = Flap,
                belts = new Dictionary<string, string>(Belts)
            };
        }
    }
}