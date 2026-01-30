using Core.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IProductService
    {
        // Option bazlı (Model+Color) metodlar
        Task<List<ProductOptionDto>> GetAllOptionsAsync(bool pActive = false);
        Task<ProductOptionDetailDto?> GetOptionDetailAsync(string model, string color);
        Task<List<ProductStockSummaryDto>> GetOptionStockSummaryAsync(string model, string color);

        // Size (Beden) bazlı metodlar
        Task<List<ProductSizeDto>> GetSizesByOptionAsync(string model, string color);
        Task<ProductDto?> GetByIdAsync(int pId);
        Task<ProductDto> AddAsync(ProductDto pModel);
        Task<bool> UpdateAsync(ProductDto pModel);
        Task<bool> DeleteAsync(int pId);

        // Eski metodlar (uyumluluk için)
        Task<List<ProductDto>> GetAllAsync(bool pActive = false);
        Task<ProductDetailDto?> GetDetailByIdAsync(int pId);

        // ProductShelf methods
        Task<List<ProductShelfDetailDto>> GetProductShelvesAsync(int productId);
        Task<ProductShelfDto?> GetProductShelfByIdAsync(int id);
        Task<ProductShelfDto> AddProductShelfAsync(ProductShelfDto pModel);
        Task<bool> UpdateProductShelfAsync(ProductShelfDto pModel);
        Task<bool> DeleteProductShelfAsync(int id);
    }
}

