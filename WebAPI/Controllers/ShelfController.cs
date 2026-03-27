using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShelfController : ControllerBase
    {
        private readonly IShelfService _shelfService;

        public ShelfController(IShelfService shelfService)
        {
            _shelfService = shelfService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var shelves = await _shelfService.GetAllAsync();
            return Ok(shelves);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var shelf = await _shelfService.GetByIdAsync(id);
            if (shelf == null)
                return NotFound();
            return Ok(shelf);
        }

        [HttpGet("warehouse/{warehouseId}")]
        public async Task<IActionResult> GetByWarehouseId(int warehouseId)
        {
            var shelves = await _shelfService.GetByWarehouseIdAsync(warehouseId);
            return Ok(shelves);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShelfDto dto)
        {
            await _shelfService.AddAsync(dto);
            return Created(nameof(GetById), dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ShelfDto dto)
        {
            dto.Id = id;
            await _shelfService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _shelfService.DeleteAsync(id);
            return Ok();
        }
    }
}
