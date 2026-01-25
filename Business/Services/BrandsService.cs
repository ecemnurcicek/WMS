using Business.Interfaces;
using Core.Dtos;
using Core.Entities;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<BrandDto> AddAsync(BrandDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Marka adı boş olamaz");

            
            var entity = new Brand
            {
                Name = dto.Name,
                IsActive = true
            };

            _context.Brands.Add(entity);      
            await _context.SaveChangesAsync(); 

            
            dto.Id = entity.Id;
            dto.IsActive = entity.IsActive;
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
                return false;
            brand.IsActive = false;
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<BrandDto>> GetAllAsync()
        {
            return await _context.Brands
                                 .Where(b => b.IsActive)
                                 .Select(b => new BrandDto
                                 {
                                     Id = b.Id,
                                     Name = b.Name,
                                     IsActive = b.IsActive
                                 })
                                 .ToListAsync();
        }

        public async Task<BrandDto?> GetByIdAsync(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if(brand == null || !brand.IsActive)
                return null;
            return new BrandDto
            {
                Id = brand.Id,
                Name = brand.Name,
                IsActive = brand.IsActive
            };
        }

        public async Task<BrandDto> UpdateAsync(BrandDto dto)
        {
           var brand = await _context.Brands.FindAsync(dto.Id);
           if(brand == null )
                throw new Exception("Marka bulunamadı");

            brand.Name = dto.Name;
            brand.IsActive = dto.IsActive;

            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
            return dto;
        }
    }
}

