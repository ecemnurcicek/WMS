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
    public class TransferService : ITransferService
    {
        private readonly ApplicationContext _context;

        public TransferService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<TransferDto>> GetAllAsync(int? status = null)
        {
            var query = _context.Transfers
                .Include(t => t.FromShop).ThenInclude(s => s!.Brand)
                .Include(t => t.ToShop).ThenInclude(s => s!.Brand)
                .Include(t => t.Details)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            var list = await query
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TransferDto
                {
                    Id = t.Id,
                    FromShopId = t.FromShopId,
                    ToShopId = t.ToShopId,
                    Name = t.Name ?? "",
                    CreateAt = t.CreatedAt,
                    CreateBy = t.CreatedBy ?? 0,
                    UpdateAt = t.UpdatedAt,
                    UpdateBy = t.UpdatedBy,
                    Status = t.Status,
                    FromShopName = t.FromShop != null ? t.FromShop.Name : "",
                    ToShopName = t.ToShop != null ? t.ToShop.Name : "",
                    FromBrandName = t.FromShop != null && t.FromShop.Brand != null ? t.FromShop.Brand.Name : "",
                    ToBrandName = t.ToShop != null && t.ToShop.Brand != null ? t.ToShop.Brand.Name : "",
                    DetailCount = t.Details.Count,
                    TotalQuantity = t.Details.Sum(d => d.QuantityRequired ?? 0)
                })
                .ToListAsync();

            return list;
        }

        public async Task<TransferDto?> GetByIdAsync(int pId)
        {
            var transfer = await _context.Transfers
                .Include(x => x.FromShop).ThenInclude(s => s!.Brand)
                .Include(x => x.ToShop).ThenInclude(s => s!.Brand)
                .Include(x => x.Details).ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(x => x.Id == pId);

            if (transfer == null)
                return null;

            var dto = new TransferDto
            {
                Id = transfer.Id,
                FromShopId = transfer.FromShopId,
                ToShopId = transfer.ToShopId,
                Name = transfer.Name ?? "",
                CreateAt = transfer.CreatedAt,
                CreateBy = transfer.CreatedBy ?? 0,
                UpdateAt = transfer.UpdatedAt,
                UpdateBy = transfer.UpdatedBy,
                Status = transfer.Status,
                FromShopName = transfer.FromShop?.Name ?? "",
                ToShopName = transfer.ToShop?.Name ?? "",
                FromBrandName = transfer.FromShop?.Brand?.Name ?? "",
                ToBrandName = transfer.ToShop?.Brand?.Name ?? "",
                DetailCount = transfer.Details.Count,
                TotalQuantity = transfer.Details.Sum(d => d.QuantityRequired ?? 0)
            };

            dto.Details = transfer.Details.Select(d => new TransferDetailDto
            {
                Id = d.Id,
                TransferId = d.TransferId,
                ProductId = d.ProductId,
                QuantityReq = d.QuantityRequired ?? 0,
                QuantitySend = d.QuantitySent,
                CreateAt = d.CreatedAt,
                CreateBy = d.CreatedBy ?? 0,
                UpdateAt = d.UpdateAt,
                UpdateBy = d.UpdateBy,
                ProductModel = d.Product?.Model ?? "",
                ProductColor = d.Product?.Color ?? "",
                ProductSize = d.Product?.Size ?? "",
                ProductEan = d.Product?.Ean ?? ""
            }).ToList();

            return dto;
        }

        public async Task<List<TransferDto>> GetByShopIdAsync(int shopId)
        {
            var list = await _context.Transfers
                .Include(t => t.FromShop).ThenInclude(s => s!.Brand)
                .Include(t => t.ToShop).ThenInclude(s => s!.Brand)
                .Include(t => t.Details)
                .Where(t => t.FromShopId == shopId || t.ToShopId == shopId)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TransferDto
                {
                    Id = t.Id,
                    FromShopId = t.FromShopId,
                    ToShopId = t.ToShopId,
                    Name = t.Name ?? "",
                    CreateAt = t.CreatedAt,
                    CreateBy = t.CreatedBy ?? 0,
                    UpdateAt = t.UpdatedAt,
                    UpdateBy = t.UpdatedBy,
                    Status = t.Status,
                    FromShopName = t.FromShop != null ? t.FromShop.Name : "",
                    ToShopName = t.ToShop != null ? t.ToShop.Name : "",
                    FromBrandName = t.FromShop != null && t.FromShop.Brand != null ? t.FromShop.Brand.Name : "",
                    ToBrandName = t.ToShop != null && t.ToShop.Brand != null ? t.ToShop.Brand.Name : "",
                    DetailCount = t.Details.Count,
                    TotalQuantity = t.Details.Sum(d => d.QuantityRequired ?? 0)
                })
                .ToListAsync();

            return list;
        }

        public async Task<List<TransferDto>> GetOutgoingByShopIdAsync(int shopId)
        {
            // Göndereceğim: ToShopId = shopId (Ben gönderenim), İptal edilenler hariç
            var list = await _context.Transfers
                .Include(t => t.FromShop).ThenInclude(s => s!.Brand)
                .Include(t => t.ToShop).ThenInclude(s => s!.Brand)
                .Include(t => t.Details)
                .Where(t => t.ToShopId == shopId && t.Status != 3)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TransferDto
                {
                    Id = t.Id,
                    FromShopId = t.FromShopId,
                    ToShopId = t.ToShopId,
                    Name = t.Name ?? "",
                    CreateAt = t.CreatedAt,
                    CreateBy = t.CreatedBy ?? 0,
                    UpdateAt = t.UpdatedAt,
                    UpdateBy = t.UpdatedBy,
                    Status = t.Status,
                    FromShopName = t.FromShop != null ? t.FromShop.Name : "",
                    ToShopName = t.ToShop != null ? t.ToShop.Name : "",
                    FromBrandName = t.FromShop != null && t.FromShop.Brand != null ? t.FromShop.Brand.Name : "",
                    ToBrandName = t.ToShop != null && t.ToShop.Brand != null ? t.ToShop.Brand.Name : "",
                    DetailCount = t.Details.Count,
                    TotalQuantity = t.Details.Sum(d => d.QuantityRequired ?? 0)
                })
                .ToListAsync();

            return list;
        }

        public async Task<List<TransferDto>> GetIncomingByShopIdAsync(int shopId)
        {
            // Beklediğim: FromShopId = shopId (Ben talep edenim)
            var list = await _context.Transfers
                .Include(t => t.FromShop).ThenInclude(s => s!.Brand)
                .Include(t => t.ToShop).ThenInclude(s => s!.Brand)
                .Include(t => t.Details)
                .Where(t => t.FromShopId == shopId)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TransferDto
                {
                    Id = t.Id,
                    FromShopId = t.FromShopId,
                    ToShopId = t.ToShopId,
                    Name = t.Name ?? "",
                    CreateAt = t.CreatedAt,
                    CreateBy = t.CreatedBy ?? 0,
                    UpdateAt = t.UpdatedAt,
                    UpdateBy = t.UpdatedBy,
                    Status = t.Status,
                    FromShopName = t.FromShop != null ? t.FromShop.Name : "",
                    ToShopName = t.ToShop != null ? t.ToShop.Name : "",
                    FromBrandName = t.FromShop != null && t.FromShop.Brand != null ? t.FromShop.Brand.Name : "",
                    ToBrandName = t.ToShop != null && t.ToShop.Brand != null ? t.ToShop.Brand.Name : "",
                    DetailCount = t.Details.Count,
                    TotalQuantity = t.Details.Sum(d => d.QuantityRequired ?? 0)
                })
                .ToListAsync();

            return list;
        }

        public async Task<List<TransferDto>> GetByBrandIdAsync(int brandId)
        {
            var list = await _context.Transfers
                .Include(t => t.FromShop).ThenInclude(s => s!.Brand)
                .Include(t => t.ToShop).ThenInclude(s => s!.Brand)
                .Include(t => t.Details)
                .Where(t => (t.FromShop != null && t.FromShop.BrandId == brandId) || 
                           (t.ToShop != null && t.ToShop.BrandId == brandId))
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TransferDto
                {
                    Id = t.Id,
                    FromShopId = t.FromShopId,
                    ToShopId = t.ToShopId,
                    Name = t.Name ?? "",
                    CreateAt = t.CreatedAt,
                    CreateBy = t.CreatedBy ?? 0,
                    UpdateAt = t.UpdatedAt,
                    UpdateBy = t.UpdatedBy,
                    Status = t.Status,
                    FromShopName = t.FromShop != null ? t.FromShop.Name : "",
                    ToShopName = t.ToShop != null ? t.ToShop.Name : "",
                    FromBrandName = t.FromShop != null && t.FromShop.Brand != null ? t.FromShop.Brand.Name : "",
                    ToBrandName = t.ToShop != null && t.ToShop.Brand != null ? t.ToShop.Brand.Name : "",
                    DetailCount = t.Details.Count,
                    TotalQuantity = t.Details.Sum(d => d.QuantityRequired ?? 0)
                })
                .ToListAsync();

            return list;
        }

        public async Task<TransferDto> AddAsync(TransferDto pModel, List<TransferDetailDto>? details = null)
        {
            if (pModel.FromShopId == pModel.ToShopId)
                throw new Exception("Talep eden ve gönderen mağaza aynı olamaz");

            var entity = new Transfer
            {
                FromShopId = pModel.FromShopId,
                ToShopId = pModel.ToShopId,
                Name = Guid.NewGuid().ToString().ToUpper(), // Otomatik GUID
                CreatedAt = DateTime.Now,
                CreatedBy = pModel.CreateBy,
                UpdatedAt = null,
                UpdatedBy = null,
                Status = 0 // Bekliyor
            };

            if (details != null && details.Any())
            {
                foreach (var d in details)
                {
                    entity.Details.Add(new TransferDetail
                    {
                        ProductId = d.ProductId,
                        QuantityRequired = d.QuantityReq,
                        QuantitySent = 0, // Başlangıçta 0
                        CreatedAt = DateTime.Now,
                        CreatedBy = d.CreateBy,
                        UpdateAt = null,
                        UpdateBy = null
                    });
                }
            }

            _context.Transfers.Add(entity);
            await _context.SaveChangesAsync();

            pModel.Id = entity.Id;
            pModel.Name = entity.Name;
            pModel.Status = 0;

            return pModel;
        }

        public async Task<bool> UpdateAsync(TransferDto pModel)
        {
            var transfer = await _context.Transfers.FindAsync(pModel.Id);
            if (transfer == null)
                throw new Exception("Transfer bulunamadı");

            transfer.FromShopId = pModel.FromShopId;
            transfer.ToShopId = pModel.ToShopId;
            transfer.UpdatedAt = DateTime.Now;
            transfer.UpdatedBy = pModel.UpdateBy;

            _context.Transfers.Update(transfer);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateStatusAsync(int transferId, int status, int updatedBy)
        {
            var transfer = await _context.Transfers
                .Include(t => t.Details)
                .FirstOrDefaultAsync(t => t.Id == transferId);
            
            if (transfer == null)
                throw new Exception("Transfer bulunamadı");

            // Eğer status=1 (Gönderildi) ise stok kontrolü ve işlemleri yap
            if (status == 1)
            {
                await ProcessTransferSendingAsync(transfer, updatedBy);
            }

            transfer.Status = status;
            transfer.UpdatedAt = DateTime.Now;
            transfer.UpdatedBy = updatedBy;

            _context.Transfers.Update(transfer);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Transfer gönderilirken stok kontrolü ve işlemleri yapar
        /// </summary>
        private async Task ProcessTransferSendingAsync(Transfer transfer, int userId)
        {
            var warnings = new List<string>();

            foreach (var detail in transfer.Details)
            {
                // Gönderici mağazanın (ToShop) raflarındaki bu ürün için stok kontrolü
                var productShelves = await _context.ProductShelves
                    .Include(ps => ps.Shelf)
                        .ThenInclude(s => s!.Warehouse)
                    .Where(ps => ps.ProductId == detail.ProductId && 
                                 ps.Shelf != null && 
                                 ps.Shelf.Warehouse != null &&
                                 ps.Shelf.Warehouse.ShopId == transfer.ToShopId)
                    .ToListAsync();

                var totalAvailableStock = productShelves.Sum(ps => ps.Quantity);
                // Kullanıcının belirlediği adet (QuantitySent) öncelikli, yoksa istenen adet (QuantityRequired)
                var requestedQuantity = detail.QuantitySent == 0 ? detail.QuantityRequired : detail.QuantitySent;
                var quantityToSend = 0;

                if (totalAvailableStock == 0)
                {
                    // Hiç stok yok
                    quantityToSend = 0;
                    warnings.Add($"Ürün {detail.ProductId} için stok bulunmamaktadır.");
                }
                else if (totalAvailableStock < requestedQuantity)
                {
                    // Yeterli stok yok, sadece mevcut stok kadar gönderilecek
                    quantityToSend = totalAvailableStock;
                    warnings.Add($"Ürün {detail.ProductId} için yeterli stok yok. İstenilen: {requestedQuantity}, Gönderilen: {quantityToSend}");
                }
                else
                {
                    // Yeterli stok var
                    quantityToSend = requestedQuantity ?? 0;
                }

                // QuantitySent güncelle
                detail.QuantitySent = quantityToSend;
                detail.UpdateAt = DateTime.Now;
                detail.UpdateBy = userId;

                // Stoğu raflardan düş ve ProductTransactions kayıtları oluştur
                var remainingQuantity = quantityToSend;
                
                foreach (var productShelf in productShelves.OrderBy(ps => ps.Id))
                {
                    if (remainingQuantity <= 0) break;

                    var quantityFromThisShelf = Math.Min(productShelf.Quantity, remainingQuantity);

                    if (quantityFromThisShelf > 0)
                    {
                        // ProductTransaction kaydı ekle
                        var transaction = new ProductTransaction
                        {
                            ProductId = detail.ProductId,
                            FromShelfId = productShelf.ShelfId,
                            ToShelfId = null, // Henüz alıcı mağazada rafa atanmadı
                            TransactionType = "TRANSFER_OUT",
                            TransferId = transfer.Id,
                            Quantity = quantityFromThisShelf,
                            CreatedAt = DateTime.Now,
                            CreatedBy = userId
                        };
                        _context.ProductTransactions.Add(transaction);

                        // ProductShelves'teki miktarı azalt
                        productShelf.Quantity -= quantityFromThisShelf;
                        if (productShelf.Quantity == 0)
                        {
                            // Stok bittiyse kaydı sil
                            _context.ProductShelves.Remove(productShelf);
                        }
                        else
                        {
                            _context.ProductShelves.Update(productShelf);
                        }

                        remainingQuantity -= quantityFromThisShelf;
                    }
                }
            }

            // Uyarılar varsa loglayabiliriz veya başka bir yerde kullanabiliriz
            if (warnings.Any())
            {
                // Log sistemine warnings yazılabilir
                foreach (var warning in warnings)
                {
                    // TODO: Warning sistemine ekle
                    Console.WriteLine($"UYARI: {warning}");
                }
            }
        }

        public async Task<bool> UpdateDetailAsync(TransferDetailDto detail)
        {
            var entity = await _context.TransferDetails.FindAsync(detail.Id);
            if (entity == null)
                throw new Exception("Transfer detayı bulunamadı");

            entity.QuantitySent = detail.QuantitySend;
            entity.UpdateAt = DateTime.Now;
            entity.UpdateBy = detail.UpdateBy;

            _context.TransferDetails.Update(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int pId, int updatedBy)
        {
            var transfer = await _context.Transfers.FindAsync(pId);
            if (transfer == null)
                return false;

            // İptal durumuna çek
            transfer.Status = 3; // İptal Edildi
            transfer.UpdatedAt = DateTime.Now;
            transfer.UpdatedBy = updatedBy;

            _context.Transfers.Update(transfer);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<TransferDetailDto>> GetDetailsByTransferIdAsync(int transferId)
        {
            var details = await _context.TransferDetails
                .Include(d => d.Product)
                .Where(d => d.TransferId == transferId)
                .Select(d => new TransferDetailDto
                {
                    Id = d.Id,
                    TransferId = d.TransferId,
                    ProductId = d.ProductId,
                    QuantityReq = d.QuantityRequired ?? 0,
                    QuantitySend = d.QuantitySent,
                    CreateAt = d.CreatedAt,
                    CreateBy = d.CreatedBy ?? 0,
                    UpdateAt = d.UpdateAt,
                    UpdateBy = d.UpdateBy,
                    ProductModel = d.Product != null ? d.Product.Model : "",
                    ProductColor = d.Product != null ? d.Product.Color : "",
                    ProductSize = d.Product != null ? d.Product.Size : "",
                    ProductEan = d.Product != null ? d.Product.Ean : ""
                })
                .ToListAsync();

            return details;
        }

        public async Task<TransferDetailDto> AddDetailAsync(TransferDetailDto detail)
        {
            var entity = new TransferDetail
            {
                TransferId = detail.TransferId,
                ProductId = detail.ProductId,
                QuantityRequired = detail.QuantityReq,
                QuantitySent = 0,
                CreatedAt = DateTime.Now,
                CreatedBy = detail.CreateBy,
                UpdateAt = null,
                UpdateBy = null
            };

            _context.TransferDetails.Add(entity);
            await _context.SaveChangesAsync();

            detail.Id = entity.Id;
            return detail;
        }

        public async Task<bool> DeleteDetailAsync(int detailId)
        {
            var detail = await _context.TransferDetails.FindAsync(detailId);
            if (detail == null)
                return false;

            _context.TransferDetails.Remove(detail);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> CreateQuickTransferAsync(int fromShopId, int toShopId, int productId, int quantity, int userId)
        {
            if (fromShopId == toShopId)
                throw new Exception("Talep eden ve gönderen mağaza aynı olamaz");

            if (quantity <= 0)
                throw new Exception("Miktar 0'dan büyük olmalıdır");

            // Transfer oluştur
            var transfer = new Transfer
            {
                FromShopId = fromShopId,
                ToShopId = toShopId,
                Name = Guid.NewGuid().ToString().ToUpper(),
                CreatedAt = DateTime.Now,
                CreatedBy = userId,
                Status = 0 // Bekliyor
            };

            var transferDetail = new TransferDetail
            {
                ProductId = productId,
                QuantityRequired = quantity,
                QuantitySent = 0,
                CreatedAt = DateTime.Now,
                CreatedBy = userId
            };

            transfer.Details.Add(transferDetail);

            _context.Transfers.Add(transfer);
            await _context.SaveChangesAsync();

            return transfer.Id;
        }
    }
}
