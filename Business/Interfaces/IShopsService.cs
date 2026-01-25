using Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IShopsService
    {
        Task<List<ShopDto>> GetAllAsync();
        Task<ShopDto?> GetByIdAsync(int id);
        Task<List<ShopDto>> GetByTownIdAsync(int townId);

        Task<ShopDto> AddAsync(ShopDto dto);
        Task<ShopDto> UpdateAsync(ShopDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
