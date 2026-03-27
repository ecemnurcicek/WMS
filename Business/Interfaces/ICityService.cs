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
        Task<List<CityDto>> GetAllAsync(bool pActive = false);
        Task<CityDto?> GetByIdAsync(int pId);
        Task<List<CityDto>> GetByRegionIdAsync(int regionId);
        Task<CityDto> AddAsync(CityDto pModel);   
        Task<bool> UpdateAsync(CityDto pModel);
        Task<bool> DeleteAsync(int pId);
    }
}

