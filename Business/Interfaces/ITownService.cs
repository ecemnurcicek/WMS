using Core.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface ITownService
    {
        Task<List<TownDto>> GetAllAsync(bool pActive = false);
        Task<TownDto?> GetByIdAsync(int pId);

        Task<TownDto> AddAsync(TownDto pModel);
        Task<bool> UpdateAsync(TownDto pModel);
        Task<bool> DeleteAsync(int pId);
    }
}
