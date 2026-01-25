using Business.Interfaces;
using Core.Dtos;
using Core.Entities;
using Data.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class ShopsService : IShopsService
    {
        private readonly ApplicationContext _context;
        public ShopsService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<ShopDto> AddAsync(ShopDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Mağaza adı boş olamaz");

            var entity = new Shop
            {
                Name = dto.Name,
                TownId = dto.TownId,
                BrandId = dto.BrandId,
                IsActive = true
            };

            _context.Shops.Add(entity);
            await _context.SaveChangesAsync();

            dto.Id = entity.Id;
            dto.IsActive = entity.IsActive;
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var shop = await _context.Shops.FindAsync(id);
            if (shop == null)
                return false;
            shop.IsActive = false;
            _context.Shops.Update(shop);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<List<ShopDto>> GetAllAsync()
        {
            return await _context.Shops
                .Where(s => s.IsActive)
                .Select(s => new ShopDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    TownId = s.TownId,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }

        public async Task<ShopDto?> GetByIdAsync(int id)
        {
            var shop = await _context.Shops.FindAsync(id);
            if (shop == null || !shop.IsActive)
                return null;

            return new ShopDto
            {
                Id = shop.Id,
                Name = shop.Name,
                TownId = shop.TownId,
                IsActive = shop.IsActive
            };
        }

        public async Task<List<ShopDto>> GetByTownIdAsync(int townId)
        {
            return await _context.Shops
                 .Where(s => s.TownId == townId && s.IsActive)
                 .Select(s => new ShopDto
                 {
                     Id = s.Id,
                     Name = s.Name,
                     TownId = s.TownId,
                     IsActive = s.IsActive
                 })
                 .ToListAsync();
        }

        public async Task<ShopDto> UpdateAsync(ShopDto dto)
        {
            var shop = await _context.Shops.FindAsync(dto.Id);
            if(shop == null || !shop.IsActive)
                throw new Exception("Mağaza bulunamadı"); 
            
            shop.Name = dto.Name;
            shop.TownId = dto.TownId;
            shop.BrandId = dto.BrandId;

            _context.Shops.Update(shop);
            await _context.SaveChangesAsync();
            return dto;
        }
    }
}


