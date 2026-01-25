using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class TransferDto
    {
        public int Id { get; set; }
        public int FromShopId { get; set; }
        public int ToShopId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreateAt { get; set; }
        public int CreateBy { get; set; }
        public bool IsSend { get; set; }
    }
}
