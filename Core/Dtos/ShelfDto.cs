using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class ShelfDto
    {
        public int Id { get; set; }
        public int RegionId { get; set; }
        public string? RegionName { get; set; }
        public int CityId { get; set; }
        public string? CityName { get; set; }
        public int TownId { get; set; }
        public string? TownName { get; set; }
        public int ShopId { get; set; }
        public string? ShopName { get; set; }
        public int WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public string Code { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
