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
        Task<List<ShopDto>> GetAllAsync(bool pActive = false);
        Task<ShopDto?> GetByIdAsync(int pId);
        Task<List<ShopDto>> GetByTownIdAsync(int townId);

        Task<ShopDto> AddAsync(ShopDto pModel);
        Task<ShopDto> UpdateAsync(ShopDto pModel);
        Task<bool> DeleteAsync(int pId);
    }
}


