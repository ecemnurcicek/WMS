using Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IRegionService
    {
        /// <summary>
        /// Bu metod bütün bölgeleri getirir.
        /// </summary>
        /// <returns></returns>
        Task<List<RegionDto>> GetAllAsync(bool pActive = false);

        /// <summary>
        /// Bu metod sadece seçili bölgeyi getirir.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<RegionDto?>GetByIdAsync(int pId);
        Task<RegionDto> AddAsync(RegionDto pModel);
        Task<bool> UpdateAsync(RegionDto pModel);
        Task<bool> DeleteAsync(int pId);
    }
}



