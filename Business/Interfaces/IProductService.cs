using Core.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllAsync(bool pActive = false);
        Task<ProductDto?> GetByIdAsync(int pId);
        Task<ProductDetailDto?> GetDetailByIdAsync(int pId);
        Task<ProductDto> AddAsync(ProductDto pModel);
        Task<bool> UpdateAsync(ProductDto pModel);
        Task<bool> DeleteAsync(int pId);
    }
}

