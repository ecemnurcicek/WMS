using Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IBrandsService
    {
        Task<List<BrandDto>> GetAllAsync(bool pActive = false);
        Task<BrandDto?> GetByIdAsync(int pId);
        Task<BrandDto> AddAsync(BrandDto pModel);
        Task<bool> UpdateAsync(BrandDto pModel);
        Task<bool> DeleteAsync(int pId);
    }
}

