using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WareHouseController : ControllerBase
    {
        private readonly IWareHouseService _wareHouseService;

        public WareHouseController(IWareHouseService wareHouseService)
        {
            _wareHouseService = wareHouseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var wareHouses = await _wareHouseService.GetAllAsync();
            return Ok(wareHouses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var wareHouse = await _wareHouseService.GetByIdAsync(id);
            if (wareHouse == null)
                return NotFound();
            return Ok(wareHouse);
        }

        [HttpGet("shop/{shopId}")]
        public async Task<IActionResult> GetByShopId(int shopId)
        {
            var wareHouses = await _wareHouseService.GetByShopIdAsync(shopId);
            return Ok(wareHouses);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WareHouseDto dto)
        {
            await _wareHouseService.AddAsync(dto);
            return Created(nameof(GetById), dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] WareHouseDto dto)
        {
            dto.Id = id;
            await _wareHouseService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _wareHouseService.DeleteAsync(id);
            return Ok();
        }
    }
}
