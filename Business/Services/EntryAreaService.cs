using Business.Interfaces;
using Core.Dtos;
using Core.Entities;
using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public class EntryAreaService : IEntryAreaService
{
    private readonly ApplicationContext _context;

    public EntryAreaService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<List<EntryAreaItemDto>> GetPendingItemsByShopIdAsync(int shopId)
    {
        // Status=2 (Tamamlandı) olan ve FromShopId = shopId olan transferleri getir (alıcı mağaza)
        var transfers = await _context.Transfers
            .Where(t => t.FromShopId == shopId && t.Status == 2)
            .Include(t => t.Details)
                .ThenInclude(d => d.Product)
            .Include(t => t.FromShop)
            .Include(t => t.Transactions)
            .ToListAsync();

        var items = new List<EntryAreaItemDto>();

        foreach (var transfer in transfers)
        {
            foreach (var detail in transfer.Details)
            {
                // Bu detail için henüz raf ataması yapılmamış transaction var mı kontrol et
                var hasPendingTransaction = transfer.Transactions.Any(tr =>
                    tr.ProductId == detail.ProductId &&
                    tr.ToShelfId == null &&
                    tr.TransactionType == "TRANSFER_OUT"
                );

                if (hasPendingTransaction)
                {
                    // FromShelfId bilgisi
                    var fromShelfId = transfer.Transactions
                        .Where(tr => tr.ProductId == detail.ProductId && tr.TransactionType == "TRANSFER_OUT")
                        .Select(tr => tr.FromShelfId)
                        .FirstOrDefault();

                    items.Add(new EntryAreaItemDto
                    {
                        TransferId = transfer.Id,
                        TransferDetailId = detail.Id,
                        ProductId = detail.ProductId,
                        ProductModel = detail.Product?.Model ?? "",
                        ProductColor = detail.Product?.Color ?? "",
                        ProductSize = detail.Product?.Size ?? "",
                        ProductEan = detail.Product?.Ean ?? "",
                        QuantitySent = detail.QuantitySent,
                        FromShopName = transfer.FromShop?.Name ?? "",
                        ReceivedAt = transfer.UpdatedAt ?? transfer.CreatedAt,
                        FromShelfId = fromShelfId,
                        FromShelfName = ""
                    });
                }
            }
        }

        // Shelf kodlarını yükle
        var shelfIds = items.Where(i => i.FromShelfId.HasValue).Select(i => i.FromShelfId!.Value).Distinct().ToList();
        if (shelfIds.Any())
        {
            var shelves = await _context.Shelves
                .Where(s => shelfIds.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, s => s.Code);

            foreach (var item in items.Where(i => i.FromShelfId.HasValue))
            {
                if (shelves.TryGetValue(item.FromShelfId!.Value, out var shelfCode))
                {
                    item.FromShelfName = shelfCode;
                }
            }
        }

        return items;
    }

    public async Task<bool> AssignShelfAsync(int transferDetailId, int productId, int toShelfId, int quantity, int userId)
    {
        // Transfer detayını bul
        var transferDetail = await _context.TransferDetails
            .Include(d => d.Transfer)
            .FirstOrDefaultAsync(d => d.Id == transferDetailId);

        if (transferDetail == null)
            throw new Exception("Transfer detayı bulunamadı");

        // Raf kontrolü
        var shelf = await _context.Shelves.FindAsync(toShelfId);
        if (shelf == null)
            throw new Exception("Raf bulunamadı");

        // ProductShelves tablosunda bu ürün için kayıt var mı kontrol et
        var productShelf = await _context.ProductShelves
            .FirstOrDefaultAsync(ps => ps.ProductId == productId && ps.ShelfId == toShelfId);

        if (productShelf != null)
        {
            // Var ise miktarı artır
            productShelf.Quantity += quantity;
            _context.ProductShelves.Update(productShelf);
        }
        else
        {
            // Yok ise yeni kayıt ekle
            productShelf = new ProductShelf
            {
                ProductId = productId,
                ShelfId = toShelfId,
                Quantity = quantity
            };
            _context.ProductShelves.Add(productShelf);
        }

        // ProductTransaction kayıtlarını güncelle (ToShelfId'yi set et)
        var transactions = await _context.ProductTransactions
            .Where(pt => pt.TransferId == transferDetail.TransferId && 
                        pt.ProductId == productId && 
                        pt.ToShelfId == null &&
                        pt.TransactionType == "TRANSFER_OUT")
            .ToListAsync();

        foreach (var transaction in transactions)
        {
            transaction.ToShelfId = toShelfId;
            _context.ProductTransactions.Update(transaction);
        }

        // Yeni bir TRANSFER_IN transaction da ekleyelim
        var transferInTransaction = new ProductTransaction
        {
            ProductId = productId,
            FromShelfId = null,
            ToShelfId = toShelfId,
            TransactionType = "TRANSFER_IN",
            TransferId = transferDetail.TransferId,
            Quantity = quantity,
            CreatedAt = DateTime.Now,
            CreatedBy = userId
        };
        _context.ProductTransactions.Add(transferInTransaction);

        await _context.SaveChangesAsync();
        return true;
    }
}
