namespace Core.Dtos;

public class UserLoginResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
    public List<int> RoleIds { get; set; } = new List<int>();
}
