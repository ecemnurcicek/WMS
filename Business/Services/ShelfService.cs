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
                .Select(s => new ShelfDto
                {
                    Id = s.Id,
                    WarehouseId = s.WarehouseId,
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
            var shelf = await _context.Shelves.FindAsync(pId);
            if (shelf == null)
                return null;

            return new ShelfDto
            {
                Id = shelf.Id,
                WarehouseId = shelf.WarehouseId,
                Code = shelf.Code,
                IsActive = shelf.IsActive
            };
        }

        public async Task<List<ShelfDto>> GetByWarehouseIdAsync(int warehouseId)
        {
            return await _context.Shelves
                .Where(s => s.WarehouseId == warehouseId && s.IsActive)
                .Select(s => new ShelfDto
                {
                    Id = s.Id,
                    WarehouseId = s.WarehouseId,
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


