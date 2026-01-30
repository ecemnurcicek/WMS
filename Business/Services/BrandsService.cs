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
    public class BrandsService : IBrandsService
    {
        private readonly ApplicationContext _context;

        public BrandsService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<BrandDto>> GetAllAsync(bool pActive = false)
        {
            var list = await _context.Brands
                .Select(b => new BrandDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    IsActive = b.IsActive
                })
                .ToListAsync();

            // Kullanıcı rolü ise sadece aktifler
            if (pActive)
                list = list.Where(b => b.IsActive).ToList();

            return list;
        }

        public async Task<BrandDto?> GetByIdAsync(int pId)
        {
            var brand = await _context.Brands.FindAsync(pId);
            if (brand == null)
                return null;

            return new BrandDto
            {
                Id = brand.Id,
                Name = brand.Name,
                IsActive = brand.IsActive
            };
        }

        public async Task<BrandDto> AddAsync(BrandDto pModel)
        {
            if (string.IsNullOrWhiteSpace(pModel.Name))
                throw new Exception("Marka adı boş olamaz");

            var entity = new Brand
            {
                Name = pModel.Name,
                IsActive = true
            };

            _context.Brands.Add(entity);
            await _context.SaveChangesAsync();

            pModel.Id = entity.Id;
            pModel.IsActive = true;
            return pModel;
        }

        public async Task<bool> UpdateAsync(BrandDto pModel)
        {
            var brand = await _context.Brands.FindAsync(pModel.Id);
            if (brand == null)
                throw new Exception("Marka bulunamadı");

            brand.Name = pModel.Name;
            brand.IsActive = pModel.IsActive;

            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int pId)
        {
            var brand = await _context.Brands.FindAsync(pId);
            if (brand == null)
                return false;

            brand.IsActive = false;
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}


