using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cities = await _cityService.GetAllAsync();
            return Ok(cities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var city = await _cityService.GetByIdAsync(id);
            if (city == null)
                return NotFound();
            return Ok(city);
        }

        [HttpGet("region/{regionId}")]
        public async Task<IActionResult> GetByRegionId(int regionId)
        {
            var cities = await _cityService.GetByRegionIdAsync(regionId);
            return Ok(cities);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CityDto dto)
        {
            await _cityService.AddAsync(dto);
            return Created(nameof(GetById), dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CityDto dto)
        {
            dto.Id = id;
            await _cityService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _cityService.DeleteAsync(id);
            return Ok();
        }
    }
}
