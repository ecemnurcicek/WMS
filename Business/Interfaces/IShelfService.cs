using Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IShelfService
    {
        Task<List<ShelfDto>> GetAllAsync();
        Task<ShelfDto?> GetByIdAsync(int id);
        Task<List<ShelfDto>> GetByWarehouseIdAsync(int warehouseId);

        Task<ShelfDto> AddAsync(ShelfDto dto);
        Task<ShelfDto> UpdateAsync(ShelfDto dto);
        Task<bool> DeleteAsync(int id);
    }
}

