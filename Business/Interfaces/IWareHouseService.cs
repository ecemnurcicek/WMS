using Core.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IWareHouseService
    {
        Task<List<WareHouseDto>> GetAllAsync(bool pActive = false);
        Task<WareHouseDto?> GetByIdAsync(int pId);

        Task<WareHouseDto> AddAsync(WareHouseDto pModel);
        Task<bool> UpdateAsync(WareHouseDto pModel);
        Task<bool> DeleteAsync(int pId);
    }
}
