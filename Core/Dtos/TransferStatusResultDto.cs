namespace Core.Dtos
{
    /// <summary>
    /// Transfer durumu güncelleme işleminin sonucunu döner
    /// Stok uyarıları ve otomatik iptal bilgisi içerir
    /// </summary>
    public class TransferStatusResultDto
    {
        /// <summary>
        /// İşlem başarılı mı?
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Sonuç durumu: 1=Gönderildi, 2=Tamamlandı, 3=İptal Edildi (otomatik)
        /// </summary>
        public int FinalStatus { get; set; }

        /// <summary>
        /// Transfer otomatik olarak iptal mi edildi?
        /// </summary>
        public bool AutoCancelled { get; set; }

        /// <summary>
        /// Kullanıcıya gösterilecek mesaj
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// Stok uyarıları listesi (ürün bazlı)
        /// </summary>
        public List<StockWarningDto> Warnings { get; set; } = new List<StockWarningDto>();
    }

    /// <summary>
    /// Ürün bazlı stok uyarısı
    /// </summary>
    public class StockWarningDto
    {
        public int ProductId { get; set; }
        public string ProductModel { get; set; } = "";
        public string ProductColor { get; set; } = "";
        public string ProductSize { get; set; } = "";
        public int QuantityRequested { get; set; }
        public int QuantityAvailable { get; set; }
        public int QuantitySent { get; set; }
    }
}
