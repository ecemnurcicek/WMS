using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegionController : ControllerBase
    {
        private readonly IRegionService _regionService;

        public RegionController(IRegionService regionService)
        {
            _regionService = regionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var regions = await _regionService.GetAllAsync();
            return Ok(regions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var region = await _regionService.GetByIdAsync(id);
            if (region == null)
                return NotFound();
            return Ok(region);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RegionDto dto)
        {
            await _regionService.AddAsync(dto);
            return Created(nameof(GetById), dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RegionDto dto)
        {
            dto.Id = id;
            await _regionService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _regionService.DeleteAsync(id);
            return Ok();
        }
    }
}
