namespace Core.Entities;

public class ProductShelf
{
    public int Id { get; set; }
    public int ShelfId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    // Navigation properties
    public virtual Shelf? Shelf { get; set; }
    public virtual Product? Product { get; set; }
}
