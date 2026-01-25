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
}


