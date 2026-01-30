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
        public int ShelfId { get; set; }
        public int Quantity { get; set; }
    }
    public class ProductShelfDetailDto
    {
        public int ShelfId { get; set; }
        public string ShelfCode { get; set; } = null!;

        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = null!;

        public int ShopId { get; set; }
        public string ShopName { get; set; } = null!;

        public int Quantity { get; set; }
    }

}



