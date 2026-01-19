namespace Core.Entities;

public class TransferDetail
{
    public int Id { get; set; }
    public int TransferId { get; set; }
    public int ProductId { get; set; }
    public int? QuantityRequired { get; set; }
    public int QuantitySent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int? CreatedBy { get; set; }

    // Navigation properties
    public virtual Transfer? Transfer { get; set; }
    public virtual Product? Product { get; set; }
}
