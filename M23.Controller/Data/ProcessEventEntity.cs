namespace M23.Controller.Data;

using System.ComponentModel.DataAnnotations;



public class ProcessEventEntity
{
    public int Id { get; set; }

    [Required]
    public string Source { get; set; } = string.Empty;

    [Required]
    public string From { get; set; } = string.Empty;

    [Required]
    public string To { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }
}