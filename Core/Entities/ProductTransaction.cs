namespace Core.Entities;

public class ProductTransaction
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int? FromShelfId { get; set; }
    public int? ToShelfId { get; set; }
    public string TransactionType { get; set; } = null!;
    public int? TransferId { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int CreatedBy { get; set; }

    // Navigation properties
    public virtual Product? Product { get; set; }
    public virtual Shelf? FromShelf { get; set; }
    public virtual Shelf? ToShelf { get; set; }
    public virtual Transfer? Transfer { get; set; }
}
