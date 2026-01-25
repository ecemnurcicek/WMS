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
        Task<List<BrandDto>> GetAllAsync();
        Task<BrandDto?> GetByIdAsync(int id);
        Task<BrandDto> AddAsync(BrandDto dto);
        Task<BrandDto> UpdateAsync(BrandDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
