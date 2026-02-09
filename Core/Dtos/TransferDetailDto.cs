using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class TransferDetailDto
    {
        public int Id { get; set; }
        public int TransferId { get; set; }
        public int ProductId { get; set; }
        public int QuantityReq { get; set; }
        public int QuantitySend { get; set; }
        public DateTime CreateAt { get; set; }
        public int CreateBy { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UpdateBy { get; set; }

        // Display properties
        public string? ProductModel { get; set; }
        public string? ProductColor { get; set; }
        public string? ProductSize { get; set; }
        public string? ProductEan { get; set; }
    }
}

