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

        // ==================== Option Bazlı Metodlar ====================

        // Option bazlı listeleme (Model + Color gruplu)
        public async Task<List<ProductOptionDto>> GetAllOptionsAsync(bool pActive = false, int? shopId = null)
        {
            var query = _context.Products
                .Include(p => p.ProductShelves)
                    .ThenInclude(ps => ps.Shelf)
                        .ThenInclude(s => s!.Warehouse)
                .AsQueryable();

            if (pActive)
                query = query.Where(p => p.IsActive);

            var products = await query.ToListAsync();

            var options = products
                .GroupBy(p => new { p.Model, p.Color })
                .Select(g => new ProductOptionDto
                {
                    Model = g.Key.Model,
                    Color = g.Key.Color,
                    CoverUrl = g.First().CoverUrl,
                    Description = g.First().Description,
                    Price = g.First().Price ?? 0m,
                    SizeCount = g.Count(),
                    TotalStock = shopId.HasValue
                        ? g.Sum(p => p.ProductShelves
                            .Where(ps => ps.Shelf != null && ps.Shelf.Warehouse != null && ps.Shelf.Warehouse.ShopId == shopId.Value)
                            .Sum(ps => ps.Quantity))
                        : g.Sum(p => p.ProductShelves.Sum(ps => ps.Quantity)),
                    IsActive = g.Any(p => p.IsActive)
                })
                .OrderBy(o => o.Model)
                .ThenBy(o => o.Color)
                .ToList();

            return options;
        }

        // Option detayı (Model + Color için tüm bedenler)
        public async Task<ProductOptionDetailDto?> GetOptionDetailAsync(string model, string color, int? shopId = null)
        {
            var products = await _context.Products
                .Include(p => p.ProductShelves)
                    .ThenInclude(ps => ps.Shelf)
                        .ThenInclude(s => s!.Warehouse)
                .Where(p => p.Model == model && p.Color == color)
                .ToListAsync();

            if (!products.Any())
                return null;

            var firstProduct = products.First();

            return new ProductOptionDetailDto
            {
                Model = model,
                Color = color,
                CoverUrl = firstProduct.CoverUrl,
                Description = firstProduct.Description,
                Price = firstProduct.Price ?? 0m,
                IsActive = products.Any(p => p.IsActive),
                Sizes = products.Select(p => new ProductSizeDto
                {
                    Id = p.Id,
                    Model = p.Model,
                    Color = p.Color,
                    Size = p.Size ?? string.Empty,
                    Ean = p.Ean ?? string.Empty,
                    Price = p.Price ?? 0m,
                    TotalStock = shopId.HasValue
                        ? p.ProductShelves
                            .Where(ps => ps.Shelf != null && ps.Shelf.Warehouse != null && ps.Shelf.Warehouse.ShopId == shopId.Value)
                            .Sum(ps => ps.Quantity)
                        : p.ProductShelves.Sum(ps => ps.Quantity),
                    IsActive = p.IsActive
                }).OrderBy(s => s.Size).ToList()
            };
        }

        // Option için mağaza bazlı stok özeti
        public async Task<List<ProductStockSummaryDto>> GetOptionStockSummaryAsync(string model, string color, int? shopId = null)
        {
            var productIds = await _context.Products
                .Where(p => p.Model == model && p.Color == color)
                .Select(p => p.Id)
                .ToListAsync();

            var query = _context.ProductShelves
                .Include(ps => ps.Shelf)
                    .ThenInclude(s => s!.Warehouse)
                        .ThenInclude(w => w!.Shop)
                .Where(ps => productIds.Contains(ps.ProductId));

            // Eğer shopId verilmişse sadece o mağazayı filtrele
            if (shopId.HasValue)
                query = query.Where(ps => ps.Shelf!.Warehouse!.ShopId == shopId.Value);

            var stockSummary = await query
                .GroupBy(ps => new { ps.Shelf!.Warehouse!.ShopId, ps.Shelf.Warehouse.Shop!.Name })
                .Select(g => new ProductStockSummaryDto
                {
                    ShopId = g.Key.ShopId,
                    ShopName = g.Key.Name ?? string.Empty,
                    TotalStock = g.Sum(ps => ps.Quantity)
                })
                .OrderBy(s => s.ShopName)
                .ToListAsync();

            return stockSummary;
        }

        // Option için beden listesi
        public async Task<List<ProductSizeDto>> GetSizesByOptionAsync(string model, string color, int? shopId = null)
        {
            var products = await _context.Products
                .Include(p => p.ProductShelves)
                    .ThenInclude(ps => ps.Shelf)
                        .ThenInclude(s => s!.Warehouse)
                .Where(p => p.Model == model && p.Color == color)
                .Select(p => new ProductSizeDto
                {
                    Id = p.Id,
                    Model = p.Model,
                    Color = p.Color,
                    Size = p.Size ?? string.Empty,
                    Ean = p.Ean ?? string.Empty,
                    Price = p.Price ?? 0m,
                    TotalStock = shopId.HasValue
                        ? p.ProductShelves
                            .Where(ps => ps.Shelf != null && ps.Shelf.Warehouse != null && ps.Shelf.Warehouse.ShopId == shopId.Value)
                            .Sum(ps => ps.Quantity)
                        : p.ProductShelves.Sum(ps => ps.Quantity),
                    IsActive = p.IsActive
                })
                .OrderBy(p => p.Size)
                .ToListAsync();

            return products;
        }

        // ==================== Mevcut Metodlar ====================

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

        // ProductShelf methods
        public async Task<List<ProductShelfDetailDto>> GetProductShelvesAsync(int productId, int? shopId = null)
        {
            var query = _context.ProductShelves
                .Include(ps => ps.Shelf)
                    .ThenInclude(s => s!.Warehouse)
                        .ThenInclude(w => w!.Shop)
                            .ThenInclude(sh => sh!.Town)
                                .ThenInclude(t => t!.City)
                                    .ThenInclude(c => c!.Region)
                .Where(ps => ps.ProductId == productId);

            // Eğer shopId belirtilmişse sadece o mağazanın raflarını getir
            if (shopId.HasValue)
            {
                query = query.Where(ps => ps.Shelf != null && ps.Shelf.Warehouse != null && ps.Shelf.Warehouse.ShopId == shopId.Value);
            }

            return await query
                .Select(ps => new ProductShelfDetailDto
                {
                    Id = ps.Id,
                    ShelfId = ps.ShelfId,
                    ShelfCode = ps.Shelf != null ? ps.Shelf.Code : string.Empty,
                    WarehouseId = ps.Shelf != null ? ps.Shelf.WarehouseId : 0,
                    WarehouseName = ps.Shelf != null && ps.Shelf.Warehouse != null ? ps.Shelf.Warehouse.Name ?? string.Empty : string.Empty,
                    ShopId = ps.Shelf != null && ps.Shelf.Warehouse != null ? ps.Shelf.Warehouse.ShopId : 0,
                    ShopName = ps.Shelf != null && ps.Shelf.Warehouse != null && ps.Shelf.Warehouse.Shop != null ? ps.Shelf.Warehouse.Shop.Name ?? string.Empty : string.Empty,
                    TownId = ps.Shelf != null && ps.Shelf.Warehouse != null && ps.Shelf.Warehouse.Shop != null ? ps.Shelf.Warehouse.Shop.TownId : 0,
                    TownName = ps.Shelf != null && ps.Shelf.Warehouse != null && ps.Shelf.Warehouse.Shop != null && ps.Shelf.Warehouse.Shop.Town != null ? ps.Shelf.Warehouse.Shop.Town.Name : null,
                    CityId = ps.Shelf != null && ps.Shelf.Warehouse != null && ps.Shelf.Warehouse.Shop != null && ps.Shelf.Warehouse.Shop.Town != null ? ps.Shelf.Warehouse.Shop.Town.CityId : 0,
                    CityName = ps.Shelf != null && ps.Shelf.Warehouse != null && ps.Shelf.Warehouse.Shop != null && ps.Shelf.Warehouse.Shop.Town != null && ps.Shelf.Warehouse.Shop.Town.City != null ? ps.Shelf.Warehouse.Shop.Town.City.CityName : null,
                    RegionId = ps.Shelf != null && ps.Shelf.Warehouse != null && ps.Shelf.Warehouse.Shop != null && ps.Shelf.Warehouse.Shop.Town != null && ps.Shelf.Warehouse.Shop.Town.City != null ? ps.Shelf.Warehouse.Shop.Town.City.RegionId : 0,
                    RegionName = ps.Shelf != null && ps.Shelf.Warehouse != null && ps.Shelf.Warehouse.Shop != null && ps.Shelf.Warehouse.Shop.Town != null && ps.Shelf.Warehouse.Shop.Town.City != null && ps.Shelf.Warehouse.Shop.Town.City.Region != null ? ps.Shelf.Warehouse.Shop.Town.City.Region.RegionName : null,
                    Quantity = ps.Quantity
                })
                .ToListAsync();
        }

        public async Task<ProductShelfDto?> GetProductShelfByIdAsync(int id)
        {
            var ps = await _context.ProductShelves
                .Include(ps => ps.Product)
                .Include(ps => ps.Shelf)
                    .ThenInclude(s => s!.Warehouse)
                        .ThenInclude(w => w!.Shop)
                            .ThenInclude(sh => sh!.Town)
                                .ThenInclude(t => t!.City)
                                    .ThenInclude(c => c!.Region)
                .FirstOrDefaultAsync(ps => ps.Id == id);

            if (ps == null)
                return null;

            return new ProductShelfDto
            {
                Id = ps.Id,
                ProductId = ps.ProductId,
                ProductModel = ps.Product?.Model,
                ProductColor = ps.Product?.Color,
                ProductSize = ps.Product?.Size,
                ShelfId = ps.ShelfId,
                ShelfCode = ps.Shelf?.Code,
                WarehouseId = ps.Shelf?.WarehouseId ?? 0,
                WarehouseName = ps.Shelf?.Warehouse?.Name,
                ShopId = ps.Shelf?.Warehouse?.ShopId ?? 0,
                ShopName = ps.Shelf?.Warehouse?.Shop?.Name,
                TownId = ps.Shelf?.Warehouse?.Shop?.TownId ?? 0,
                TownName = ps.Shelf?.Warehouse?.Shop?.Town?.Name,
                CityId = ps.Shelf?.Warehouse?.Shop?.Town?.CityId ?? 0,
                CityName = ps.Shelf?.Warehouse?.Shop?.Town?.City?.CityName,
                RegionId = ps.Shelf?.Warehouse?.Shop?.Town?.City?.RegionId ?? 0,
                RegionName = ps.Shelf?.Warehouse?.Shop?.Town?.City?.Region?.RegionName,
                Quantity = ps.Quantity
            };
        }

        public async Task<ProductShelfDto> AddProductShelfAsync(ProductShelfDto pModel)
        {
            // Check if already exists
            var existing = await _context.ProductShelves
                .FirstOrDefaultAsync(ps => ps.ProductId == pModel.ProductId && ps.ShelfId == pModel.ShelfId);

            if (existing != null)
            {
                // Update quantity
                existing.Quantity += pModel.Quantity;
                _context.ProductShelves.Update(existing);
                await _context.SaveChangesAsync();
                pModel.Id = existing.Id;
                return pModel;
            }

            var entity = new ProductShelf
            {
                ProductId = pModel.ProductId,
                ShelfId = pModel.ShelfId,
                Quantity = pModel.Quantity
            };

            _context.ProductShelves.Add(entity);
            await _context.SaveChangesAsync();

            pModel.Id = entity.Id;
            return pModel;
        }

        public async Task<bool> UpdateProductShelfAsync(ProductShelfDto pModel)
        {
            var ps = await _context.ProductShelves.FindAsync(pModel.Id);
            if (ps == null)
                return false;

            ps.ShelfId = pModel.ShelfId;
            ps.Quantity = pModel.Quantity;

            _context.ProductShelves.Update(ps);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteProductShelfAsync(int id)
        {
            var ps = await _context.ProductShelves.FindAsync(id);
            if (ps == null)
                return false;

            _context.ProductShelves.Remove(ps);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ShopStockDto?> GetShopWithMaxStockAsync(int productId, int? excludeShopId = null)
        {
            var query = _context.ProductShelves
                .Include(ps => ps.Shelf)
                    .ThenInclude(s => s!.Warehouse)
                        .ThenInclude(w => w!.Shop)
                .Where(ps => ps.ProductId == productId && 
                            ps.Shelf != null && 
                            ps.Shelf.Warehouse != null && 
                            ps.Shelf.Warehouse.Shop != null);

            if (excludeShopId.HasValue)
            {
                query = query.Where(ps => ps.Shelf!.Warehouse!.ShopId != excludeShopId.Value);
            }

            var productShelves = await query.ToListAsync();

            if (!productShelves.Any())
                return null;

            var shopStocks = productShelves
                .GroupBy(ps => new
                {
                    ShopId = ps.Shelf!.Warehouse!.ShopId,
                    ShopName = ps.Shelf!.Warehouse!.Shop!.Name
                })
                .Select(g => new ShopStockDto
                {
                    ShopId = g.Key.ShopId,
                    ShopName = g.Key.ShopName,
                    TotalStock = g.Sum(ps => ps.Quantity)
                })
                .OrderByDescending(s => s.TotalStock)
                .FirstOrDefault();

            return shopStocks;
        }

        /// <summary>
        /// Ürün arama - barkod, model, renk, beden, açıklama ile arama yapar
        /// Mağaza bazlı stok bilgilerini döner
        /// </summary>
        public async Task<List<ProductSearchResultDto>> SearchProductsAsync(string? query = null, int? brandId = null, int? shopId = null)
        {
            var productsQuery = _context.Products
                .Include(p => p.ProductShelves)
                    .ThenInclude(ps => ps.Shelf)
                        .ThenInclude(s => s!.Warehouse)
                            .ThenInclude(w => w!.Shop)
                                .ThenInclude(sh => sh!.Brand)
                .Where(p => p.IsActive)
                .AsQueryable();

            // Arama sorgusu
            if (!string.IsNullOrWhiteSpace(query))
            {
                var searchTerm = query.Trim().ToLower();
                productsQuery = productsQuery.Where(p =>
                    (p.Ean != null && p.Ean.ToLower().Contains(searchTerm)) ||
                    (p.Model != null && p.Model.ToLower().Contains(searchTerm)) ||
                    (p.Color != null && p.Color.ToLower().Contains(searchTerm)) ||
                    (p.Size != null && p.Size.ToLower().Contains(searchTerm)) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchTerm))
                );
            }

            var products = await productsQuery.ToListAsync();

            // Marka filtresi (ProductShelves üzerinden)
            if (brandId.HasValue)
            {
                products = products
                    .Where(p => p.ProductShelves.Any(ps =>
                        ps.Shelf != null &&
                        ps.Shelf.Warehouse != null &&
                        ps.Shelf.Warehouse.Shop != null &&
                        ps.Shelf.Warehouse.Shop.BrandId == brandId.Value))
                    .ToList();
            }

            // Mağaza filtresi (ProductShelves üzerinden)
            if (shopId.HasValue)
            {
                products = products
                    .Where(p => p.ProductShelves.Any(ps =>
                        ps.Shelf != null &&
                        ps.Shelf.Warehouse != null &&
                        ps.Shelf.Warehouse.ShopId == shopId.Value))
                    .ToList();
            }

            var results = new List<ProductSearchResultDto>();

            foreach (var product in products)
            {
                var shopStocks = product.ProductShelves
                    .Where(ps => ps.Shelf != null &&
                                 ps.Shelf.Warehouse != null &&
                                 ps.Shelf.Warehouse.Shop != null)
                    .GroupBy(ps => new
                    {
                        ShopId = ps.Shelf!.Warehouse!.ShopId,
                        ShopName = ps.Shelf!.Warehouse!.Shop!.Name,
                        BrandName = ps.Shelf!.Warehouse!.Shop!.Brand!.Name
                    })
                    .Select(g => new ShopStockInfoDto
                    {
                        ShopId = g.Key.ShopId,
                        ShopName = g.Key.ShopName,
                        BrandName = g.Key.BrandName,
                        TotalStock = g.Sum(ps => ps.Quantity)
                    })
                    .OrderByDescending(s => s.TotalStock)
                    .ToList();

                // Sadece stok varsa ekle
                if (shopStocks.Any(s => s.TotalStock > 0))
                {
                    results.Add(new ProductSearchResultDto
                    {
                        ProductId = product.Id,
                        Barcode = product.Ean ?? "",
                        Model = product.Model ?? "",
                        Color = product.Color ?? "",
                        Size = product.Size ?? "",
                        Description = product.Description ?? "",
                        CoverUrl = product.CoverUrl,
                        ShopStocks = shopStocks
                    });
                }
            }

            return results;
        }
    }
}




