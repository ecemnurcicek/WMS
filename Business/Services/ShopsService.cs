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
    public class ShopsService : IShopsService
    {
        private readonly ApplicationContext _context;

        public ShopsService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<ShopDto>> GetAllAsync(bool pActive = false)
        {
            var list = await _context.Shops
                .Select(s => new ShopDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    BrandId = s.BrandId,
                    TownId = s.TownId,
                    IsActive = s.IsActive
                })
                .ToListAsync();

            // Kullanıcı rolü ise sadece aktifler
            if (pActive)
                list = list.Where(s => s.IsActive).ToList();

            return list;
        }

        public async Task<ShopDto?> GetByIdAsync(int pId)
        {
            var shop = await _context.Shops.FindAsync(pId);
            if (shop == null)
                return null;

            return new ShopDto
            {
                Id = shop.Id,
                Name = shop.Name,
                BrandId = shop.BrandId,
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
                    BrandId = s.BrandId,
                    TownId = s.TownId,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }

        public async Task<ShopDto> AddAsync(ShopDto pModel)
        {
            if (string.IsNullOrWhiteSpace(pModel.Name))
                throw new Exception("Mağaza adı boş olamaz");

            var entity = new Shop
            {
                Name = pModel.Name,
                BrandId = pModel.BrandId,
                TownId = pModel.TownId,
                IsActive = true
            };

            _context.Shops.Add(entity);
            await _context.SaveChangesAsync();

            pModel.Id = entity.Id;
            pModel.IsActive = true;
            return pModel;
        }

        public async Task<ShopDto> UpdateAsync(ShopDto pModel)
        {
            var shop = await _context.Shops.FindAsync(pModel.Id);
            if (shop == null)
                throw new Exception("Mağaza bulunamadı");

            shop.Name = pModel.Name;
            shop.BrandId = pModel.BrandId;
            shop.TownId = pModel.TownId;
            shop.IsActive = pModel.IsActive;

            _context.Shops.Update(shop);
            await _context.SaveChangesAsync();

            return pModel;
        }

        public async Task<bool> DeleteAsync(int pId)
        {
            var shop = await _context.Shops.FindAsync(pId);
            if (shop == null)
                return false;

            shop.IsActive = false;
            _context.Shops.Update(shop);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
