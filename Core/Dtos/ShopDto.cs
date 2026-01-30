using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class ShopDto
    {
        public int Id { get; set; }
        public int RegionId { get; set; }
        public string? RegionName { get; set; }
        public int CityId { get; set; }
        public string? CityName { get; set; }
        public int TownId { get; set; }
        public string? TownName { get; set; }
        public int BrandId { get; set; }
        public string? BrandName { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}

