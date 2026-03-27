using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransferController : ControllerBase
    {
        private readonly ITransferService _transferService;

        public TransferController(ITransferService transferService)
        {
            _transferService = transferService;
        }

        /// <summary>
        /// Tüm transferleri getirir (opsiyonel durum filtresi)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? status = null)
        {
            var transfers = await _transferService.GetAllAsync(status);
            return Ok(transfers);
        }

        /// <summary>
        /// ID'ye göre transfer getirir (detaylarıyla birlikte)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var transfer = await _transferService.GetByIdAsync(id);
            if (transfer == null)
                return NotFound();
            return Ok(transfer);
        }

        /// <summary>
        /// Mağazaya göre transferleri getirir
        /// </summary>
        [HttpGet("shop/{shopId}")]
        public async Task<IActionResult> GetByShopId(int shopId)
        {
            var transfers = await _transferService.GetByShopIdAsync(shopId);
            return Ok(transfers);
        }

        /// <summary>
        /// Mağazanın göndereceği transferleri getirir (ToShopId = shopId, iptal hariç)
        /// </summary>
        [HttpGet("shop/{shopId}/outgoing")]
        public async Task<IActionResult> GetOutgoingByShopId(int shopId)
        {
            var transfers = await _transferService.GetOutgoingByShopIdAsync(shopId);
            return Ok(transfers);
        }

        /// <summary>
        /// Mağazanın beklediği transferleri getirir (FromShopId = shopId)
        /// </summary>
        [HttpGet("shop/{shopId}/incoming")]
        public async Task<IActionResult> GetIncomingByShopId(int shopId)
        {
            var transfers = await _transferService.GetIncomingByShopIdAsync(shopId);
            return Ok(transfers);
        }

        /// <summary>
        /// Markaya göre transferleri getirir
        /// </summary>
        [HttpGet("brand/{brandId}")]
        public async Task<IActionResult> GetByBrandId(int brandId)
        {
            var transfers = await _transferService.GetByBrandIdAsync(brandId);
            return Ok(transfers);
        }

        /// <summary>
        /// Transfer detaylarını getirir
        /// </summary>
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var details = await _transferService.GetDetailsByTransferIdAsync(id);
            return Ok(details);
        }

        /// <summary>
        /// Yeni transfer oluşturur
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TransferCreateRequest request)
        {
            var transfer = await _transferService.AddAsync(request.Transfer, request.Details);
            return Created(nameof(GetById), transfer);
        }

        /// <summary>
        /// Transfer günceller
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TransferDto dto)
        {
            dto.Id = id;
            await _transferService.UpdateAsync(dto);
            return Ok();
        }

        /// <summary>
        /// Transfer durumunu günceller
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusUpdateRequest request)
        {
            var result = await _transferService.UpdateStatusAsync(id, request.Status, request.UpdatedBy);
            return Ok(result);
        }

        /// <summary>
        /// Transfer detayı günceller (gönderilen adet)
        /// </summary>
        [HttpPut("detail/{detailId}")]
        public async Task<IActionResult> UpdateDetail(int detailId, [FromBody] TransferDetailDto dto)
        {
            dto.Id = detailId;
            await _transferService.UpdateDetailAsync(dto);
            return Ok();
        }

        /// <summary>
        /// Transfer detayı ekler
        /// </summary>
        [HttpPost("{id}/detail")]
        public async Task<IActionResult> AddDetail(int id, [FromBody] TransferDetailDto dto)
        {
            dto.TransferId = id;
            var detail = await _transferService.AddDetailAsync(dto);
            return Created("", detail);
        }

        /// <summary>
        /// Transfer detayı siler
        /// </summary>
        [HttpDelete("detail/{detailId}")]
        public async Task<IActionResult> DeleteDetail(int detailId)
        {
            var result = await _transferService.DeleteDetailAsync(detailId);
            if (!result)
                return NotFound();
            return Ok();
        }

        /// <summary>
        /// Transfer iptal eder
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] int updatedBy)
        {
            var result = await _transferService.DeleteAsync(id, updatedBy);
            if (!result)
                return NotFound();
            return Ok();
        }

        /// <summary>
        /// Hızlı transfer talebi oluşturur (tek ürün için)
        /// </summary>
        [HttpPost("quick")]
        public async Task<IActionResult> CreateQuickTransfer([FromBody] QuickTransferRequest request)
        {
            try
            {
                var transferId = await _transferService.CreateQuickTransferAsync(
                    request.FromShopId,
                    request.ToShopId,
                    request.ProductId,
                    request.Quantity,
                    request.UserId
                );
                return Ok(new { transferId, message = "Transfer talebi başarıyla oluşturuldu." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    // Request models
    public class TransferCreateRequest
    {
        public TransferDto Transfer { get; set; } = null!;
        public List<TransferDetailDto>? Details { get; set; }
    }

    public class StatusUpdateRequest
    {
        public int Status { get; set; }
        public int UpdatedBy { get; set; }
    }

    public class QuickTransferRequest
    {
        public int FromShopId { get; set; }
        public int ToShopId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int UserId { get; set; }
    }
}
