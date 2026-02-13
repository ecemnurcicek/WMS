namespace Core.Dtos;

/// <summary>
/// Giriş alanında raf ataması bekleyen ürünler için DTO
/// </summary>
public class EntryAreaItemDto
{
    public int TransferId { get; set; }
    public int TransferDetailId { get; set; }
    public int ProductId { get; set; }
    public string ProductModel { get; set; } = string.Empty;
    public string ProductColor { get; set; } = string.Empty;
    public string ProductSize { get; set; } = string.Empty;
    public string ProductEan { get; set; } = string.Empty;
    public int QuantitySent { get; set; }
    public int? FromShelfId { get; set; }
    public string FromShelfName { get; set; } = string.Empty;
    public string FromShopName { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
}
