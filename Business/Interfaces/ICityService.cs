using Core.Dtos;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface ICityService
    {
        Task<List<CityDto>> GetAllAsync();
        Task<CityDto?> GetByIdAsync(int id);
        Task<List<CityDto>> GetByRegionIdAsync(int regionId);

        Task<CityDto> AddAsync(CityDto dto);   
        Task<CityDto> UpdateAsync(CityDto dto);
        Task<bool> DeleteAsync(int id);
    }
}

