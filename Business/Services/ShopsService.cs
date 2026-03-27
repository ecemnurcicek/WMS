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
                .Include(s => s.Brand)
                .Include(s => s.Town)
                    .ThenInclude(t => t!.City)
                        .ThenInclude(c => c!.Region)
                .Select(s => new ShopDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    BrandId = s.BrandId,
                    BrandName = s.Brand != null ? s.Brand.Name : null,
                    TownId = s.TownId,
                    TownName = s.Town != null ? s.Town.Name : null,
                    CityId = s.Town != null ? s.Town.CityId : 0,
                    CityName = s.Town != null && s.Town.City != null ? s.Town.City.CityName : null,
                    RegionId = s.Town != null && s.Town.City != null ? s.Town.City.RegionId : 0,
                    RegionName = s.Town != null && s.Town.City != null && s.Town.City.Region != null ? s.Town.City.Region.RegionName : null,
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
            var shop = await _context.Shops
                .Include(s => s.Brand)
                .Include(s => s.Town)
                    .ThenInclude(t => t!.City)
                        .ThenInclude(c => c!.Region)
                .FirstOrDefaultAsync(s => s.Id == pId);
            if (shop == null)
                return null;

            return new ShopDto
            {
                Id = shop.Id,
                Name = shop.Name,
                BrandId = shop.BrandId,
                BrandName = shop.Brand?.Name,
                TownId = shop.TownId,
                TownName = shop.Town?.Name,
                CityId = shop.Town?.CityId ?? 0,
                CityName = shop.Town?.City?.CityName,
                RegionId = shop.Town?.City?.RegionId ?? 0,
                RegionName = shop.Town?.City?.Region?.RegionName,
                IsActive = shop.IsActive
            };
        }

        public async Task<List<ShopDto>> GetByTownIdAsync(int townId)
        {
            return await _context.Shops
                .Include(s => s.Brand)
                .Include(s => s.Town)
                    .ThenInclude(t => t!.City)
                        .ThenInclude(c => c!.Region)
                .Where(s => s.TownId == townId && s.IsActive)
                .Select(s => new ShopDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    BrandId = s.BrandId,
                    BrandName = s.Brand != null ? s.Brand.Name : null,
                    TownId = s.TownId,
                    TownName = s.Town != null ? s.Town.Name : null,
                    CityId = s.Town != null ? s.Town.CityId : 0,
                    CityName = s.Town != null && s.Town.City != null ? s.Town.City.CityName : null,
                    RegionId = s.Town != null && s.Town.City != null ? s.Town.City.RegionId : 0,
                    RegionName = s.Town != null && s.Town.City != null && s.Town.City.Region != null ? s.Town.City.Region.RegionName : null,
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
