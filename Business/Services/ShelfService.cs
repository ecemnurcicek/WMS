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
    public class ShelfService : IShelfService
    {
        private readonly ApplicationContext _context;

        public ShelfService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<ShelfDto>> GetAllAsync(bool pActive = false)
        {
            var list = await _context.Shelves
                .Include(s => s.Warehouse)
                    .ThenInclude(w => w!.Shop)
                        .ThenInclude(sh => sh!.Town)
                            .ThenInclude(t => t!.City)
                                .ThenInclude(c => c!.Region)
                .Select(s => new ShelfDto
                {
                    Id = s.Id,
                    WarehouseId = s.WarehouseId,
                    WarehouseName = s.Warehouse != null ? s.Warehouse.Name : null,
                    ShopId = s.Warehouse != null ? s.Warehouse.ShopId : 0,
                    ShopName = s.Warehouse != null && s.Warehouse.Shop != null ? s.Warehouse.Shop.Name : null,
                    TownId = s.Warehouse != null && s.Warehouse.Shop != null ? s.Warehouse.Shop.TownId : 0,
                    TownName = s.Warehouse != null && s.Warehouse.Shop != null && s.Warehouse.Shop.Town != null ? s.Warehouse.Shop.Town.Name : null,
                    CityId = s.Warehouse != null && s.Warehouse.Shop != null && s.Warehouse.Shop.Town != null ? s.Warehouse.Shop.Town.CityId : 0,
                    CityName = s.Warehouse != null && s.Warehouse.Shop != null && s.Warehouse.Shop.Town != null && s.Warehouse.Shop.Town.City != null ? s.Warehouse.Shop.Town.City.CityName : null,
                    RegionId = s.Warehouse != null && s.Warehouse.Shop != null && s.Warehouse.Shop.Town != null && s.Warehouse.Shop.Town.City != null ? s.Warehouse.Shop.Town.City.RegionId : 0,
                    RegionName = s.Warehouse != null && s.Warehouse.Shop != null && s.Warehouse.Shop.Town != null && s.Warehouse.Shop.Town.City != null && s.Warehouse.Shop.Town.City.Region != null ? s.Warehouse.Shop.Town.City.Region.RegionName : null,
                    Code = s.Code,
                    IsActive = s.IsActive
                })
                .ToListAsync();

            // Eğer kullanıcı rolünde ise sadece aktifler
            if (pActive)
                list = list.Where(s => s.IsActive).ToList();

            return list;
        }

        public async Task<ShelfDto?> GetByIdAsync(int pId)
        {
            var shelf = await _context.Shelves
                .Include(s => s.Warehouse)
                    .ThenInclude(w => w!.Shop)
                        .ThenInclude(sh => sh!.Town)
                            .ThenInclude(t => t!.City)
                                .ThenInclude(c => c!.Region)
                .FirstOrDefaultAsync(s => s.Id == pId);

            if (shelf == null)
                return null;

            return new ShelfDto
            {
                Id = shelf.Id,
                WarehouseId = shelf.WarehouseId,
                WarehouseName = shelf.Warehouse?.Name,
                ShopId = shelf.Warehouse?.ShopId ?? 0,
                ShopName = shelf.Warehouse?.Shop?.Name,
                TownId = shelf.Warehouse?.Shop?.TownId ?? 0,
                TownName = shelf.Warehouse?.Shop?.Town?.Name,
                CityId = shelf.Warehouse?.Shop?.Town?.CityId ?? 0,
                CityName = shelf.Warehouse?.Shop?.Town?.City?.CityName,
                RegionId = shelf.Warehouse?.Shop?.Town?.City?.RegionId ?? 0,
                RegionName = shelf.Warehouse?.Shop?.Town?.City?.Region?.RegionName,
                Code = shelf.Code,
                IsActive = shelf.IsActive
            };
        }

        public async Task<List<ShelfDto>> GetByWarehouseIdAsync(int warehouseId)
        {
            return await _context.Shelves
                .Include(s => s.Warehouse)
                    .ThenInclude(w => w!.Shop)
                        .ThenInclude(sh => sh!.Town)
                            .ThenInclude(t => t!.City)
                                .ThenInclude(c => c!.Region)
                .Where(s => s.WarehouseId == warehouseId && s.IsActive)
                .Select(s => new ShelfDto
                {
                    Id = s.Id,
                    WarehouseId = s.WarehouseId,
                    WarehouseName = s.Warehouse != null ? s.Warehouse.Name : null,
                    ShopId = s.Warehouse != null ? s.Warehouse.ShopId : 0,
                    ShopName = s.Warehouse != null && s.Warehouse.Shop != null ? s.Warehouse.Shop.Name : null,
                    TownId = s.Warehouse != null && s.Warehouse.Shop != null ? s.Warehouse.Shop.TownId : 0,
                    TownName = s.Warehouse != null && s.Warehouse.Shop != null && s.Warehouse.Shop.Town != null ? s.Warehouse.Shop.Town.Name : null,
                    CityId = s.Warehouse != null && s.Warehouse.Shop != null && s.Warehouse.Shop.Town != null ? s.Warehouse.Shop.Town.CityId : 0,
                    CityName = s.Warehouse != null && s.Warehouse.Shop != null && s.Warehouse.Shop.Town != null && s.Warehouse.Shop.Town.City != null ? s.Warehouse.Shop.Town.City.CityName : null,
                    RegionId = s.Warehouse != null && s.Warehouse.Shop != null && s.Warehouse.Shop.Town != null && s.Warehouse.Shop.Town.City != null ? s.Warehouse.Shop.Town.City.RegionId : 0,
                    RegionName = s.Warehouse != null && s.Warehouse.Shop != null && s.Warehouse.Shop.Town != null && s.Warehouse.Shop.Town.City != null && s.Warehouse.Shop.Town.City.Region != null ? s.Warehouse.Shop.Town.City.Region.RegionName : null,
                    Code = s.Code,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }

        public async Task<ShelfDto> AddAsync(ShelfDto pModel)
        {
            if (string.IsNullOrWhiteSpace(pModel.Code))
                throw new Exception("Raf kodu boş olamaz");

            var entity = new Shelf
            {
                Code = pModel.Code,
                WarehouseId = pModel.WarehouseId,
                IsActive = true
            };

            _context.Shelves.Add(entity);
            await _context.SaveChangesAsync();

            pModel.Id = entity.Id;
            pModel.IsActive = true;
            return pModel;
        }

        public async Task<bool> UpdateAsync(ShelfDto pModel)
        {
            var shelf = await _context.Shelves.FindAsync(pModel.Id);
            if (shelf == null)
                throw new Exception("Raf bulunamadı");

            shelf.Code = pModel.Code;
            shelf.WarehouseId = pModel.WarehouseId;
            shelf.IsActive = pModel.IsActive;

            _context.Shelves.Update(shelf);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int pId)
        {
            var shelf = await _context.Shelves.FindAsync(pId);
            if (shelf == null)
                return false;

            shelf.IsActive = false;
            _context.Shelves.Update(shelf);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}


