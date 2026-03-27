namespace Core.Entities;

public class Town
{
    public int Id { get; set; }
    public int CityId { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual City? City { get; set; }
    public virtual ICollection<Shop> Shops { get; set; } = new List<Shop>();
}
