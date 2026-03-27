namespace Core.Dtos;

public class MenuRoleDto
{
    public int Id { get; set; }
    public int MenuId { get; set; }
    public int RoleId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }

    // Display properties
    public string? MenuName { get; set; }
    public string? RoleName { get; set; }
}
