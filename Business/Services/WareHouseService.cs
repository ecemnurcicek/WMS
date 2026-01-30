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
            var query = _context.Warehouses.AsQueryable();

            if (pActive)
                query = query.Where(w => w.IsActive);

            return await query
                .Select(w => new WareHouseDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    ShopId = w.ShopId,
                    IsActive = w.IsActive
                })
                .ToListAsync();
        }

        public async Task<WareHouseDto?> GetByIdAsync(int pId)
        {
            return await _context.Warehouses
                .Where(w => w.Id == pId)
                .Select(w => new WareHouseDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    ShopId = w.ShopId,
                    IsActive = w.IsActive
                })
                .FirstOrDefaultAsync();
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
