namespace Core.Entities;

public class Region
{
    public int Id { get; set; }
    public string RegionName { get; set; } = null!;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}
