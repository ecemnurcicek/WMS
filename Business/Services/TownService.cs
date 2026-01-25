using Business.Interfaces;
using Core.Dtos;
using Core.Entities;
using Data.Context; 
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Service
{
    public class TownService : ITownService
    {
        private readonly ApplicationContext _context;

        public TownService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<TownDto>> GetAllAsync()
        {
            return await _context.Towns
                                 .Where(t => t.IsActive)
                                 .Select(t => new TownDto
                                 {
                                     Id = t.Id,
                                     Name = t.Name,
                                     CityId = t.CityId
                                 })
                                 .ToListAsync();
        }

        public async Task<TownDto?> GetByIdAsync(int id)
        {
            var town = await _context.Towns.FindAsync(id);
            if (town == null || !town.IsActive) return null;

            return new TownDto
            {
                Id = town.Id,
                Name = town.Name,
                CityId = town.CityId
            };
        }

        public async Task<TownDto> AddAsync(TownDto dto)
        {
            var town = new Town
            {
                Name = dto.Name,
                CityId = dto.CityId,
                IsActive = true
            };

            _context.Towns.Add(town);
            await _context.SaveChangesAsync();

            dto.Id = town.Id;
            return dto;
        }

        public async Task<TownDto> UpdateAsync(TownDto dto)
        {
            var town = await _context.Towns.FindAsync(dto.Id);
            if (town == null) throw new KeyNotFoundException("Town not found");

            town.Name = dto.Name;
            town.CityId = dto.CityId;

            _context.Towns.Update(town);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var town = await _context.Towns.FindAsync(id);
            if (town == null) return false;

            
            town.IsActive = false;
            _context.Towns.Update(town);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
