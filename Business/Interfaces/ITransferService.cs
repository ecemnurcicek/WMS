using Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface ITransferService
    {
        /// <summary>
        /// Tüm transferleri getirir
        /// </summary>
        Task<List<TransferDto>> GetAllAsync(int? status = null);

        /// <summary>
        /// Seçili transferi getirir (detaylarıyla birlikte)
        /// </summary>
        Task<TransferDto?> GetByIdAsync(int pId);

        /// <summary>
        /// Mağazaya göre transferleri getirir (FromShopId veya ToShopId)
        /// </summary>
        Task<List<TransferDto>> GetByShopIdAsync(int shopId);

        /// <summary>
        /// Mağazanın göndereceği transferleri getirir (ToShopId = shopId, İptal edilenler hariç)
        /// </summary>
        Task<List<TransferDto>> GetOutgoingByShopIdAsync(int shopId);

        /// <summary>
        /// Mağazanın beklediği transferleri getirir (FromShopId = shopId)
        /// </summary>
        Task<List<TransferDto>> GetIncomingByShopIdAsync(int shopId);

        /// <summary>
        /// Markaya göre transferleri getirir
        /// </summary>
        Task<List<TransferDto>> GetByBrandIdAsync(int brandId);

        /// <summary>
        /// Transfer + detayları ile ekler (Name otomatik GUID olur)
        /// </summary>
        Task<TransferDto> AddAsync(TransferDto pModel, List<TransferDetailDto>? details = null);

        /// <summary>
        /// Transfer güncelleme
        /// </summary>
        Task<bool> UpdateAsync(TransferDto pModel);

        /// <summary>
        /// Transfer durumunu günceller (0: Bekliyor, 1: Gönderildi, 2: Tamamlandı, 3: İptal)
        /// </summary>
        Task<bool> UpdateStatusAsync(int transferId, int status, int updatedBy);

        /// <summary>
        /// Transfer detayı günceller (gönderilen adet güncellemesi için)
        /// </summary>
        Task<bool> UpdateDetailAsync(TransferDetailDto detail);

        /// <summary>
        /// Transfer silme (iptal)
        /// </summary>
        Task<bool> DeleteAsync(int pId, int updatedBy);

        /// <summary>
        /// Transfer detaylarını getirir
        /// </summary>
        Task<List<TransferDetailDto>> GetDetailsByTransferIdAsync(int transferId);

        /// <summary>
        /// Transfer detayı ekler
        /// </summary>
        Task<TransferDetailDto> AddDetailAsync(TransferDetailDto detail);

        /// <summary>
        /// Transfer detayı siler
        /// </summary>
        Task<bool> DeleteDetailAsync(int detailId);

        /// <summary>
        /// Hızlı transfer talebi oluşturur (tek ürün için)
        /// </summary>
        Task<int> CreateQuickTransferAsync(int fromShopId, int toShopId, int productId, int quantity, int userId);
    }
}

