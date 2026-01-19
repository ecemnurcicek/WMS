namespace Core.Entities;

public class Transfer
{
    public int Id { get; set; }
    public int FromShopId { get; set; }
    public int ToShopId { get; set; }
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public bool IsSent { get; set; } = false;

    // Navigation properties
    public virtual Shop? FromShop { get; set; }
    public virtual Shop? ToShop { get; set; }
    public virtual ICollection<TransferDetail> Details { get; set; } = new List<TransferDetail>();
    public virtual ICollection<ProductTransaction> Transactions { get; set; } = new List<ProductTransaction>();
}
