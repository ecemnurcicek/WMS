using Microsoft.AspNetCore.Identity;

namespace Core.Entities;

public class ApplicationUser : IdentityUser<string>
{
    public int? ShopId { get; set; }
    public string? FullName { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
}
