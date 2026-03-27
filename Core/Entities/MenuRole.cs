namespace Core.Entities;

public class MenuRole
{
    public int Id { get; set; }
    public int MenuId { get; set; }
    public int RoleId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }

    // Navigation properties
    public virtual Menu Menu { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
}
