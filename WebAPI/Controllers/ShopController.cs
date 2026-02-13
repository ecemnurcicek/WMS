using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopController : ControllerBase
    {
        private readonly IShopsService _shopService;

        public ShopController(IShopsService shopService)
        {
            _shopService = shopService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var shops = await _shopService.GetAllAsync();
            return Ok(shops);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var shop = await _shopService.GetByIdAsync(id);
            if (shop == null)
                return NotFound();
            return Ok(shop);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var shops = await _shopService.GetAllAsync(pActive: true);
            return Ok(shops);
        }

        [HttpGet("town/{townId}")]
        public async Task<IActionResult> GetByTownId(int townId)
        {
            var shops = await _shopService.GetByTownIdAsync(townId);
            return Ok(shops);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShopDto dto)
        {
            await _shopService.AddAsync(dto);
            return Created(nameof(GetById), dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ShopDto dto)
        {
            dto.Id = id;
            await _shopService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _shopService.DeleteAsync(id);
            return Ok();
        }
    }
}
