namespace Core.Entities;

public class City
{
    public int Id { get; set; }
    public int RegionId { get; set; }
    public string CityName { get; set; } = null!;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Region? Region { get; set; }
    public virtual ICollection<Town> Towns { get; set; } = new List<Town>();
}
