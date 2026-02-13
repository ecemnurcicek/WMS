using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    /// <summary>
    /// Transfer durumları:
    /// 0 = Bekliyor
    /// 1 = Gönderildi
    /// 2 = Tamamlandı
    /// 3 = İptal Edildi
    /// </summary>
    public class TransferDto
    {
        public int Id { get; set; }
        public int FromShopId { get; set; }
        public int ToShopId { get; set; }
        public string? Name { get; set; }
        public DateTime CreateAt { get; set; }
        public int CreateBy { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UpdateBy { get; set; }
        public int Status { get; set; } // 0: Bekliyor, 1: Gönderildi, 2: Tamamlandı, 3: İptal Edildi

        // Display properties
        public string? FromShopName { get; set; }
        public string? ToShopName { get; set; }
        public string? FromBrandName { get; set; }
        public string? ToBrandName { get; set; }
        public string? CreatedByName { get; set; }
        public int DetailCount { get; set; }
        public int TotalQuantity { get; set; }

        public string StatusText => Status switch
        {
            0 => "Bekliyor",
            1 => "Gönderildi",
            2 => "Tamamlandı",
            3 => "İptal Edildi",
            _ => "Bilinmiyor"
        };

        public List<TransferDetailDto> Details { get; set; } = new List<TransferDetailDto>();
    }
}
