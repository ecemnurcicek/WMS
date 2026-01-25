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

        public async Task<List<CityDto>> GetAllAsync()
        {
            return await _context.Cities
                                 .Where(c => c.IsActive)
                                 .Select(c => new CityDto
                                 {
                                     Id = c.Id,
                                     Name = c.CityName,
                                     RegionId = c.RegionId,
                                     IsActive = c.IsActive
                                 }).ToListAsync();
        }

        public async Task<CityDto?> GetByIdAsync(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null || !city.IsActive) return null;

            return new CityDto
            {
                Id = city.Id,
                Name = city.CityName,
                RegionId = city.RegionId,
                IsActive = city.IsActive
            };
        }

        public async Task<List<CityDto>> GetByRegionIdAsync(int regionId)
        {
            return await _context.Cities
                                 .Where(c => c.RegionId == regionId && c.IsActive)
                                 .Select(c => new CityDto
                                 {
                                     Id = c.Id,
                                     Name = c.CityName,
                                     RegionId = c.RegionId,
                                     IsActive = c.IsActive
                                 }).ToListAsync();
        }

        public async Task<CityDto> AddAsync(CityDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Şehir adı boş bırakılamaz");

            var entity = new City
            {
                CityName = dto.Name,
                RegionId = dto.RegionId,
                IsActive = true
            };

            _context.Cities.Add(entity);
            await _context.SaveChangesAsync();

            dto.Id = entity.Id;
            dto.IsActive = true;
            return dto;
        }

        public async Task<CityDto> UpdateAsync(CityDto dto)
        {
            var city = await _context.Cities.FindAsync(dto.Id);
            if (city == null)
                throw new Exception("Şehir bulunamadı");

            city.CityName = dto.Name;
            city.RegionId = dto.RegionId;
            city.IsActive = dto.IsActive;

            _context.Cities.Update(city);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
                return false;

            city.IsActive = false; 
            _context.Cities.Update(city);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
