namespace M23.Shared.Protocol;

using System.Text.Json.Serialization;



public class CommandPayload
{
    [JsonPropertyName("target")]
    public string Target { get; set; } = string.Empty;

    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;

    [JsonPropertyName("position")]
    public string? Position { get; set; }
}