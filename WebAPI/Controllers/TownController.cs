using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TownController : ControllerBase
    {
        private readonly ITownService _townService;

        public TownController(ITownService townService)
        {
            _townService = townService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var towns = await _townService.GetAllAsync();
            return Ok(towns);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var town = await _townService.GetByIdAsync(id);
            if (town == null)
                return NotFound();
            return Ok(town);
        }

        [HttpGet("city/{cityId}")]
        public async Task<IActionResult> GetByCityId(int cityId)
        {
            var towns = await _townService.GetByCityIdAsync(cityId);
            return Ok(towns);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TownDto dto)
        {
            await _townService.AddAsync(dto);
            return Created(nameof(GetById), dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TownDto dto)
        {
            dto.Id = id;
            await _townService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _townService.DeleteAsync(id);
            return Ok();
        }
    }
}
