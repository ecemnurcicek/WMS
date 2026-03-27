using Core.Dtos;

namespace Business.Interfaces;

public interface IEntryAreaService
{
    /// <summary>
    /// Mağazaya gelen ve kabul edilen ürünleri getirir (Raf ataması için)
    /// Status=2 (Tamamlandı) olan transferlerdeki ürünler
    /// </summary>
    Task<List<EntryAreaItemDto>> GetPendingItemsByShopIdAsync(int shopId);

    /// <summary>
    /// Ürüne raf ataması yapar
    /// </summary>
    Task<bool> AssignShelfAsync(int transferDetailId, int productId, int toShelfId, int quantity, int userId);
}
