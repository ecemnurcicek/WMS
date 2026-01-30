namespace Core.Entities;

public class Product
{
    public int Id { get; set; }
    public string Model { get; set; } = null!;
    public string? Color { get; set; }
    public string? Size { get; set; }
    public string? CoverUrl { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? Ean { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<ProductShelf> ProductShelves { get; set; } = new List<ProductShelf>();
    public virtual ICollection<ProductTransaction> Transactions { get; set; } = new List<ProductTransaction>();
    public virtual ICollection<TransferDetail> TransferDetails { get; set; } = new List<TransferDetail>();
}
