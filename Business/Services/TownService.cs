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
    public class TownService : ITownService
    {
        private readonly ApplicationContext _context;

        public TownService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<TownDto>> GetAllAsync(bool pActive = false)
        {
            var list = await _context.Towns
                .Include(t => t.City)
                    .ThenInclude(c => c!.Region)
                .Select(t => new TownDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    CityId = t.CityId,
                    CityName = t.City != null ? t.City.CityName : null,
                    RegionId = t.City != null ? t.City.RegionId : 0,
                    RegionName = t.City != null && t.City.Region != null ? t.City.Region.RegionName : null,
                    IsActive = t.IsActive
                })
                .ToListAsync();

            if (pActive)
                list = list.Where(t => t.IsActive).ToList();

            return list;
        }

        public async Task<TownDto?> GetByIdAsync(int pId)
        {
            var town = await _context.Towns
                .Include(t => t.City)
                    .ThenInclude(c => c!.Region)
                .FirstOrDefaultAsync(t => t.Id == pId);

            if (town == null)
                return null;

            return new TownDto
            {
                Id = town.Id,
                Name = town.Name,
                CityId = town.CityId,
                CityName = town.City?.CityName,
                RegionId = town.City?.RegionId ?? 0,
                RegionName = town.City?.Region?.RegionName,
                IsActive = town.IsActive
            };
        }

        public async Task<List<TownDto>> GetByCityIdAsync(int cityId)
        {
            return await _context.Towns
                .Include(t => t.City)
                    .ThenInclude(c => c!.Region)
                .Where(t => t.CityId == cityId && t.IsActive)
                .Select(t => new TownDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    CityId = t.CityId,
                    CityName = t.City != null ? t.City.CityName : null,
                    RegionId = t.City != null ? t.City.RegionId : 0,
                    RegionName = t.City != null && t.City.Region != null ? t.City.Region.RegionName : null,
                    IsActive = t.IsActive
                })
                .ToListAsync();
        }

        public async Task<TownDto> AddAsync(TownDto pModel)
        {
            if (string.IsNullOrWhiteSpace(pModel.Name))
                throw new Exception("İlçe adı boş olamaz");

            var entity = new Town
            {
                Name = pModel.Name,
                CityId = pModel.CityId,
                IsActive = true
            };

            _context.Towns.Add(entity);
            await _context.SaveChangesAsync();

            pModel.Id = entity.Id;
            pModel.IsActive = true;
            return pModel;
        }

        public async Task<bool> UpdateAsync(TownDto pModel)
        {
            var town = await _context.Towns.FindAsync(pModel.Id);
            if (town == null)
                throw new Exception("İlçe bulunamadı");

            town.Name = pModel.Name;
            town.CityId = pModel.CityId;
            town.IsActive = pModel.IsActive;

            _context.Towns.Update(town);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int pId)
        {
            var town = await _context.Towns.FindAsync(pId);
            if (town == null)
                return false;

            town.IsActive = false;
            _context.Towns.Update(town);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
