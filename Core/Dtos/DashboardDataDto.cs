namespace Core.Dtos;

public class DashboardDataDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
    public int TotalProducts { get; set; }
    public int TotalWarehouses { get; set; }
    public int TotalTransfers { get; set; }
    public int ActiveTransfers { get; set; }
    public DateTime LastLoginTime { get; set; }
}
