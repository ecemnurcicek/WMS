namespace Core.Entities;

public class Warehouse
{
    public int Id { get; set; }
    public int ShopId { get; set; }
    public string? Name { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Shop? Shop { get; set; }
    public virtual ICollection<Shelf> Shelves { get; set; } = new List<Shelf>();
}
