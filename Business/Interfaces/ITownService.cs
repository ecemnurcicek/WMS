using Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface ITownService
    {
        Task<List<TownDto>> GetAllAsync();
        Task<TownDto?> GetByIdAsync(int id);
        Task<TownDto> AddAsync(TownDto dto);
        Task<TownDto> UpdateAsync(TownDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
