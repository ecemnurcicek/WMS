using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // ==================== Option Bazlı Endpoints ====================

        // Option listesi (Model + Color gruplu)
        [HttpGet("options")]
        public async Task<IActionResult> GetAllOptions([FromQuery] int? shopId = null)
        {
            var options = await _productService.GetAllOptionsAsync(shopId: shopId);
            return Ok(options);
        }

        // Ürün arama - barkod, model, renk, beden, açıklama
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string? query = null, [FromQuery] int? brandId = null, [FromQuery] int? shopId = null)
        {
            var results = await _productService.SearchProductsAsync(query, brandId, shopId);
            return Ok(new { success = true, data = results });
        }

        // Option detayı (Beden listesi ile)
        [HttpGet("option")]
        public async Task<IActionResult> GetOptionDetail([FromQuery] string model, [FromQuery] string color, [FromQuery] int? shopId = null)
        {
            var option = await _productService.GetOptionDetailAsync(model, color, shopId);
            if (option == null)
                return NotFound();
            return Ok(option);
        }

        // Option için mağaza bazlı stok özeti
        [HttpGet("option/stock")]
        public async Task<IActionResult> GetOptionStockSummary([FromQuery] string model, [FromQuery] string color, [FromQuery] int? shopId = null)
        {
            var stockSummary = await _productService.GetOptionStockSummaryAsync(model, color, shopId);
            return Ok(stockSummary);
        }

        // Option için beden listesi
        [HttpGet("option/sizes")]
        public async Task<IActionResult> GetSizesByOption([FromQuery] string model, [FromQuery] string color, [FromQuery] int? shopId = null)
        {
            var sizes = await _productService.GetSizesByOptionAsync(model, color, shopId);
            return Ok(sizes);
        }

        // ==================== Mevcut Endpoints ====================

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetDetailById(int id)
        {
            var product = await _productService.GetDetailByIdAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDto dto)
        {
            // EAN validation - must be exactly 13 digits
            if (string.IsNullOrEmpty(dto.Ean) || dto.Ean.Length != 13 || !dto.Ean.All(char.IsDigit))
            {
                return BadRequest(new { message = "EAN kodu tam 13 haneli rakam olmalıdır." });
            }

            var result = await _productService.AddAsync(dto);
            return Created(nameof(GetById), result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDto dto)
        {
            // EAN validation - must be exactly 13 digits
            if (string.IsNullOrEmpty(dto.Ean) || dto.Ean.Length != 13 || !dto.Ean.All(char.IsDigit))
            {
                return BadRequest(new { message = "EAN kodu tam 13 haneli rakam olmalıdır." });
            }

            dto.Id = id;
            var result = await _productService.UpdateAsync(dto);
            if (!result)
                return NotFound();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id);
            if (!result)
                return NotFound();
            return Ok();
        }

        // ProductShelf endpoints
        [HttpGet("{productId}/shelves")]
        public async Task<IActionResult> GetProductShelves(int productId, [FromQuery] int? shopId = null)
        {
            var shelves = await _productService.GetProductShelvesAsync(productId, shopId);
            return Ok(shelves);
        }

        [HttpGet("shelf/{id}")]
        public async Task<IActionResult> GetProductShelfById(int id)
        {
            var productShelf = await _productService.GetProductShelfByIdAsync(id);
            if (productShelf == null)
                return NotFound();
            return Ok(productShelf);
        }

        [HttpPost("shelf")]
        public async Task<IActionResult> AddProductShelf([FromBody] ProductShelfDto dto)
        {
            var result = await _productService.AddProductShelfAsync(dto);
            return Created(nameof(GetProductShelfById), result);
        }

        [HttpPut("shelf/{id}")]
        public async Task<IActionResult> UpdateProductShelf(int id, [FromBody] ProductShelfDto dto)
        {
            dto.Id = id;
            var result = await _productService.UpdateProductShelfAsync(dto);
            if (!result)
                return NotFound();
            return Ok();
        }

        [HttpDelete("shelf/{id}")]
        public async Task<IActionResult> DeleteProductShelf(int id)
        {
            var result = await _productService.DeleteProductShelfAsync(id);
            if (!result)
                return NotFound();
            return Ok();
        }

        // En fazla stok bulunan mağazayı getirir
        [HttpGet("{productId}/max-stock-shop")]
        public async Task<IActionResult> GetShopWithMaxStock(int productId, [FromQuery] int? excludeShopId = null)
        {
            var shopStock = await _productService.GetShopWithMaxStockAsync(productId, excludeShopId);
            if (shopStock == null)
                return NotFound(new { message = "Bu ürün için stok bulunamadı." });
            return Ok(shopStock);
        }
    }
}
