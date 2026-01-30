using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    // Option bazlı DTO (Model + Color) - Listeleme için
    public class ProductOptionDto
    {
        public string Model { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string? CoverUrl { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int SizeCount { get; set; }  // Kaç farklı beden var
        public int TotalStock { get; set; } // Toplam stok adedi
        public bool IsActive { get; set; }
    }

    // Beden bazlı DTO - Detay modal için
    public class ProductSizeDto
    {
        public int Id { get; set; }
        public string Model { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string Size { get; set; } = null!;
        public string Ean { get; set; } = null!;
        public decimal Price { get; set; }
        public int TotalStock { get; set; } // Bu bedenin toplam stoku
        public bool IsActive { get; set; }
    }

    // Standart Product DTO
    public class ProductDto
    {
        public int Id { get; set; }
        public string Model { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string Size { get; set; } = null!;
        public decimal Price { get; set; }
        public string Ean { get; set; } = null!;
        public string? CoverUrl { get; set; }
        public string? Description { get; set; }
        public DateTime CreateAt { get; set; }
        public int CreateBy { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int UpdateBy { get; set; }
        public bool IsActive { get; set; }
    }

    // Option detay DTO - Model+Color için tüm bedenler
    public class ProductOptionDetailDto
    {
        public string Model { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string? CoverUrl { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public List<ProductSizeDto> Sizes { get; set; } = new();
    }

    // Mağaza bazlı stok özeti
    public class ProductStockSummaryDto
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; } = null!;
        public int TotalStock { get; set; }
    }

    public class ProductDetailDto
    {
       
        public int Id { get; set; }
        public string Model { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string Size { get; set; } = null!;
        public decimal Price { get; set; }
        public string Ean { get; set; } = null!;
        public string? CoverUrl { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        
        public int BrandId { get; set; }
        public string BrandName { get; set; } = null!;

        
        public List<ProductShelfDetailDto> Shelves { get; set; } = new();
    }
}


