namespace Core.Entities;

public class Log
{
    public int Id { get; set; }
    public string? ControllerName { get; set; }
    public int? LineNumber { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int? CreatedBy { get; set; }
}
