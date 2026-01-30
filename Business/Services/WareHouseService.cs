using Business.Interfaces;
using Core.Dtos;
using Core.Entities;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
    public class WareHouseService : IWareHouseService
    {
        private readonly ApplicationContext _context;

        public WareHouseService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<WareHouseDto>> GetAllAsync(bool pActive = false)
        {
            var query = _context.Warehouses
                .Include(w => w.Shop)
                    .ThenInclude(s => s!.Town)
                        .ThenInclude(t => t!.City)
                            .ThenInclude(c => c!.Region)
                .AsQueryable();

            if (pActive)
                query = query.Where(w => w.IsActive);

            return await query
                .Select(w => new WareHouseDto
                {
                    Id = w.Id,
                    Name = w.Name ?? string.Empty,
                    ShopId = w.ShopId,
                    ShopName = w.Shop != null ? w.Shop.Name : null,
                    TownId = w.Shop != null ? w.Shop.TownId : 0,
                    TownName = w.Shop != null && w.Shop.Town != null ? w.Shop.Town.Name : null,
                    CityId = w.Shop != null && w.Shop.Town != null ? w.Shop.Town.CityId : 0,
                    CityName = w.Shop != null && w.Shop.Town != null && w.Shop.Town.City != null ? w.Shop.Town.City.CityName : null,
                    RegionId = w.Shop != null && w.Shop.Town != null && w.Shop.Town.City != null ? w.Shop.Town.City.RegionId : 0,
                    RegionName = w.Shop != null && w.Shop.Town != null && w.Shop.Town.City != null && w.Shop.Town.City.Region != null ? w.Shop.Town.City.Region.RegionName : null,
                    IsActive = w.IsActive
                })
                .ToListAsync();
        }

        public async Task<WareHouseDto?> GetByIdAsync(int pId)
        {
            var warehouse = await _context.Warehouses
                .Include(w => w.Shop)
                    .ThenInclude(s => s!.Town)
                        .ThenInclude(t => t!.City)
                            .ThenInclude(c => c!.Region)
                .FirstOrDefaultAsync(w => w.Id == pId);

            if (warehouse == null)
                return null;

            return new WareHouseDto
            {
                Id = warehouse.Id,
                Name = warehouse.Name ?? string.Empty,
                ShopId = warehouse.ShopId,
                ShopName = warehouse.Shop?.Name,
                TownId = warehouse.Shop?.TownId ?? 0,
                TownName = warehouse.Shop?.Town?.Name,
                CityId = warehouse.Shop?.Town?.CityId ?? 0,
                CityName = warehouse.Shop?.Town?.City?.CityName,
                RegionId = warehouse.Shop?.Town?.City?.RegionId ?? 0,
                RegionName = warehouse.Shop?.Town?.City?.Region?.RegionName,
                IsActive = warehouse.IsActive
            };
        }

        public async Task<List<WareHouseDto>> GetByShopIdAsync(int shopId)
        {
            return await _context.Warehouses
                .Include(w => w.Shop)
                    .ThenInclude(s => s!.Town)
                        .ThenInclude(t => t!.City)
                            .ThenInclude(c => c!.Region)
                .Where(w => w.ShopId == shopId && w.IsActive)
                .Select(w => new WareHouseDto
                {
                    Id = w.Id,
                    Name = w.Name ?? string.Empty,
                    ShopId = w.ShopId,
                    ShopName = w.Shop != null ? w.Shop.Name : null,
                    TownId = w.Shop != null ? w.Shop.TownId : 0,
                    TownName = w.Shop != null && w.Shop.Town != null ? w.Shop.Town.Name : null,
                    CityId = w.Shop != null && w.Shop.Town != null ? w.Shop.Town.CityId : 0,
                    CityName = w.Shop != null && w.Shop.Town != null && w.Shop.Town.City != null ? w.Shop.Town.City.CityName : null,
                    RegionId = w.Shop != null && w.Shop.Town != null && w.Shop.Town.City != null ? w.Shop.Town.City.RegionId : 0,
                    RegionName = w.Shop != null && w.Shop.Town != null && w.Shop.Town.City != null && w.Shop.Town.City.Region != null ? w.Shop.Town.City.Region.RegionName : null,
                    IsActive = w.IsActive
                })
                .ToListAsync();
        }

        public async Task<WareHouseDto> AddAsync(WareHouseDto pModel)
        {
            var warehouse = new Warehouse
            {
                Name = pModel.Name,
                ShopId = pModel.ShopId,
                IsActive = true
            };

            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();

            pModel.Id = warehouse.Id;
            pModel.IsActive = true;

            return pModel;
        }

        public async Task<bool> UpdateAsync(WareHouseDto pModel)
        {
            var warehouse = await _context.Warehouses.FindAsync(pModel.Id);
            if (warehouse == null)
                return false;

            warehouse.Name = pModel.Name;
            warehouse.ShopId = pModel.ShopId;

            _context.Warehouses.Update(warehouse);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int pId)
        {
            var warehouse = await _context.Warehouses.FindAsync(pId);
            if (warehouse == null)
                return false;

            warehouse.IsActive = false;
            _context.Warehouses.Update(warehouse);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
