using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class ProductShelfDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductModel { get; set; }
        public string? ProductColor { get; set; }
        public string? ProductSize { get; set; }
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
        public int ShelfId { get; set; }
        public string? ShelfCode { get; set; }
        public int Quantity { get; set; }
    }
    public class ProductShelfDetailDto
    {
        public int Id { get; set; }
        public int ShelfId { get; set; }
        public string ShelfCode { get; set; } = null!;

        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = null!;

        public int ShopId { get; set; }
        public string ShopName { get; set; } = null!;

        public int TownId { get; set; }
        public string? TownName { get; set; }

        public int CityId { get; set; }
        public string? CityName { get; set; }

        public int RegionId { get; set; }
        public string? RegionName { get; set; }

        public int Quantity { get; set; }
    }

}



