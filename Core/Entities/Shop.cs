namespace Core.Entities;

public class Shop
{
    public int Id { get; set; }
    public int BrandId { get; set; }
    public int TownId { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Brand? Brand { get; set; }
    public virtual Town? Town { get; set; }
    public virtual ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
    public virtual ICollection<Transfer> FromTransfers { get; set; } = new List<Transfer>();
    public virtual ICollection<Transfer> ToTransfers { get; set; } = new List<Transfer>();
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
