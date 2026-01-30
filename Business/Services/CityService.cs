using Business.Interfaces;
using Core.Dtos;
using Core.Entities;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
    public class CityService : ICityService
    {
        private readonly ApplicationContext _context;

        public CityService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<CityDto>> GetAllAsync(bool pActive = false)
        {
            var list = await _context.Cities
                .Include(c => c.Region)
                .Select(c => new CityDto
                {
                    Id = c.Id,
                    Name = c.CityName,
                    RegionId = c.RegionId,
                    RegionName = c.Region != null ? c.Region.RegionName : null,
                    IsActive = c.IsActive
                })
                .ToListAsync();

            // Eğer kullanıcı rolünde ise sadece aktifler
            if (pActive)
                list = list.Where(c => c.IsActive).ToList();

            return list;
        }

        public async Task<CityDto?> GetByIdAsync(int pId)
        {
            var city = await _context.Cities
                .Include(c => c.Region)
                .FirstOrDefaultAsync(c => c.Id == pId);
            if (city == null)
                return null;

            var rModel = new CityDto
            {
                Id = city.Id,
                Name = city.CityName,
                RegionId = city.RegionId,
                RegionName = city.Region?.RegionName,
                IsActive = city.IsActive
            };

            return rModel;
        }

        public async Task<List<CityDto>> GetByRegionIdAsync(int regionId)
        {
            var rModel = await _context.Cities
                .Include(c => c.Region)
                .Where(c => c.RegionId == regionId && c.IsActive)
                .Select(c => new CityDto
                {
                    Id = c.Id,
                    Name = c.CityName,
                    RegionId = c.RegionId,
                    RegionName = c.Region != null ? c.Region.RegionName : null,
                    IsActive = c.IsActive
                })
                .ToListAsync();
            return rModel;
        }

        public async Task<CityDto> AddAsync(CityDto pModel)
        {
            if (string.IsNullOrWhiteSpace(pModel.Name))
                throw new Exception("Şehir adı boş olamaz");

            var entity = new City
            {
                CityName = pModel.Name,
                RegionId = pModel.RegionId,
                IsActive = true
            };

            _context.Cities.Add(entity);
            await _context.SaveChangesAsync();

            pModel.Id = entity.Id;
            pModel.IsActive = true;
            return pModel;
        }

        public async Task<bool> UpdateAsync(CityDto pModel)
        {
            var city = await _context.Cities.FindAsync(pModel.Id);
            if (city == null)
                throw new Exception("Şehir bulunamadı");

            city.CityName = pModel.Name;
            city.RegionId = pModel.RegionId;
            city.IsActive = pModel.IsActive;

            _context.Cities.Update(city);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int pId)
        {
            var city = await _context.Cities.FindAsync(pId);
            if (city == null)
                return false;

            city.IsActive = false;
            _context.Cities.Update(city);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

