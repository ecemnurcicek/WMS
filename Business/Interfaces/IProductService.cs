using Core.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IProductService
    {
        // Option bazlı (Model+Color) metodlar
        Task<List<ProductOptionDto>> GetAllOptionsAsync(bool pActive = false, int? shopId = null);
        Task<ProductOptionDetailDto?> GetOptionDetailAsync(string model, string color, int? shopId = null);
        Task<List<ProductStockSummaryDto>> GetOptionStockSummaryAsync(string model, string color, int? shopId = null);

        // Size (Beden) bazlı metodlar
        Task<List<ProductSizeDto>> GetSizesByOptionAsync(string model, string color, int? shopId = null);
        Task<ProductDto?> GetByIdAsync(int pId);
        Task<ProductDto> AddAsync(ProductDto pModel);
        Task<bool> UpdateAsync(ProductDto pModel);
        Task<bool> DeleteAsync(int pId);

        // Eski metodlar (uyumluluk için)
        Task<List<ProductDto>> GetAllAsync(bool pActive = false);
        Task<ProductDetailDto?> GetDetailByIdAsync(int pId);

        // ProductShelf methods
        Task<List<ProductShelfDetailDto>> GetProductShelvesAsync(int productId, int? shopId = null);
        Task<ProductShelfDto?> GetProductShelfByIdAsync(int id);
        Task<ProductShelfDto> AddProductShelfAsync(ProductShelfDto pModel);
        Task<bool> UpdateProductShelfAsync(ProductShelfDto pModel);
        Task<bool> DeleteProductShelfAsync(int id);

        /// <summary>
        /// Belirtilen ürün için en fazla stok bulunan mağazayı döner
        /// </summary>
        Task<ShopStockDto?> GetShopWithMaxStockAsync(int productId, int? excludeShopId = null);

        /// <summary>
        /// Ürün arama - barkod, model, renk, beden, açıklama ile arama yapar
        /// Mağaza bazlı stok bilgilerini döner
        /// </summary>
        Task<List<ProductSearchResultDto>> SearchProductsAsync(string? query = null, int? brandId = null, int? shopId = null);
    }
}

