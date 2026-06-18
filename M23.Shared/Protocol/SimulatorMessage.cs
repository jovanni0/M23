namespace M23.Shared.Protocol;

using System.Text.Json;
using System.Text.Json.Serialization;



public class SimulatorMessage
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("payload")]
    public JsonElement Payload { get; set; }
    
    
    /*
     * Create a new Event message.
     */
    public static SimulatorMessage CreateEvent(string source, string from, string to) =>
        new()
        {
            Type = MessageType.Event,
            Payload = JsonSerializer.SerializeToElement(new
            {
                source,
                from,
                to
            })
        };

    
    /*
     * Create a new Sync message.
     */
    public static SimulatorMessage CreateSync(
        string system,
        string flap,
        string m1, string m2,
        string m3, string m4) =>
        new()
        {
            Type = MessageType.Sync,
            Payload = JsonSerializer.SerializeToElement(new
            {
                system,
                flap,
                belts = new { M1 = m1, M2 = m2, M3 = m3, M4 = m4 }
            })
        };

    
    public static SimulatorMessage? Deserialize(string json) =>
        JsonSerializer.Deserialize<SimulatorMessage>(json);

    
    public string Serialize() =>
        JsonSerializer.Serialize(this);
}