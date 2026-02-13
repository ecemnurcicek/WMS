namespace Core.Entities;

public class Brand
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<Shop> Shops { get; set; } = new List<Shop>();
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
