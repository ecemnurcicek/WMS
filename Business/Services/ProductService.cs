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
    public class ProductService : IProductService
    {
        private readonly ApplicationContext _context;

        public ProductService(ApplicationContext context)
        {
            _context = context;
        }
        public async Task<List<ProductDto>> GetAllAsync(bool pActive = false)
        {
            var list = await _context.Products
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Model = p.Model,
                    Color = p.Color,
                    Size = p.Size,
                    Price = p.Price ?? 0m,
                    Ean = p.Ean,
                    CoverUrl = p.CoverUrl,
                    Description = null,
                    CreateAt = p.CreatedAt,
                    UpdateAt = p.UpdatedAt,
                    IsActive = p.IsActive
                })
                .ToListAsync();

            if (pActive)
                list = list.Where(p => p.IsActive).ToList();

            return list;
        }
        public async Task<ProductDto?> GetByIdAsync(int pId)
        {
            var product = await _context.Products.FindAsync(pId);
            if (product == null)
                return null;

            return new ProductDto
            {
                Id = product.Id,
                Model = product.Model,
                Color = product.Color,
                Size = product.Size,
                Price = product.Price ?? 0m,   
                Ean = product.Ean,
                CoverUrl = product.CoverUrl,
                Description = null,
                CreateAt = product.CreatedAt,
                UpdateAt = product.UpdatedAt,
                IsActive = product.IsActive
            };
        }
        public async Task<ProductDetailDto?> GetDetailByIdAsync(int pId)
        {
            var product = await _context.Products
                .Include(p => p.ProductShelves)
                    .ThenInclude(ps => ps.Shelf)
                        .ThenInclude(s => s.Warehouse)
                            .ThenInclude(w => w.Shop)
                .FirstOrDefaultAsync(p => p.Id == pId && p.IsActive);

            if (product == null)
                return null;

            return new ProductDetailDto
            {
                Id = product.Id,
                Model = product.Model,
                Color = product.Color,
                Size = product.Size,
                Price = product.Price ?? 0m,   
                Ean = product.Ean,
                CoverUrl = product.CoverUrl,
                Description = null,
                IsActive = product.IsActive,

                Shelves = product.ProductShelves.Select(ps => new ProductShelfDetailDto
                {
                    ShelfId = ps.ShelfId,
                    ShelfCode = ps.Shelf.Code,
                    WarehouseId = ps.Shelf.WarehouseId,
                    WarehouseName = ps.Shelf.Warehouse.Name,
                    ShopId = ps.Shelf.Warehouse.ShopId,
                    ShopName = ps.Shelf.Warehouse.Shop.Name,
                    Quantity = ps.Quantity
                }).ToList()
            };
        }
        public async Task<ProductDto> AddAsync(ProductDto pModel)
        {
            if (string.IsNullOrWhiteSpace(pModel.Model))
                throw new Exception("Ürün modeli boş olamaz");

            var entity = new Product
            {
                Model = pModel.Model,
                Color = pModel.Color,
                Size = pModel.Size,
                Price = pModel.Price, 
                Ean = pModel.Ean,
                CoverUrl = pModel.CoverUrl,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Products.Add(entity);
            await _context.SaveChangesAsync();

            pModel.Id = entity.Id;
            pModel.IsActive = true;
            pModel.CreateAt = entity.CreatedAt;

            return pModel;
        }
        public async Task<bool> UpdateAsync(ProductDto pModel)
        {
            var product = await _context.Products.FindAsync(pModel.Id);
            if (product == null)
                throw new Exception("Ürün bulunamadı");

            product.Model = pModel.Model;
            product.Color = pModel.Color;
            product.Size = pModel.Size;
            product.Price = pModel.Price; 
            product.Ean = pModel.Ean;
            product.CoverUrl = pModel.CoverUrl;
            product.IsActive = pModel.IsActive;
            product.UpdatedAt = DateTime.Now;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> DeleteAsync(int pId)
        {
            var product = await _context.Products.FindAsync(pId);
            if (product == null)
                return false;

            product.IsActive = false;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}




