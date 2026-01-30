using Core.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IShelfService
    {
        Task<List<ShelfDto>> GetAllAsync(bool pActive = false);
        Task<ShelfDto?> GetByIdAsync(int pId);
        Task<List<ShelfDto>> GetByWarehouseIdAsync(int warehouseId);

        Task<ShelfDto> AddAsync(ShelfDto pModel);
        Task<bool> UpdateAsync(ShelfDto pModel);
        Task<bool> DeleteAsync(int pId);
    }
}

