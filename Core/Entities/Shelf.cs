namespace Core.Entities;

public class Shelf
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public string Code { get; set; } = null!;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Warehouse? Warehouse { get; set; }
    public virtual ICollection<ProductShelf> ProductShelves { get; set; } = new List<ProductShelf>();
    public virtual ICollection<ProductTransaction> FromTransactions { get; set; } = new List<ProductTransaction>();
    public virtual ICollection<ProductTransaction> ToTransactions { get; set; } = new List<ProductTransaction>();
}
