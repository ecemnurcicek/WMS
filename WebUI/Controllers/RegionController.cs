using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebUI.Controllers
{
    public class RegionController : Controller
    {
        private readonly IRegionService _regionService;

        public RegionController(IRegionService regionService)
        {
            _regionService = regionService;
        }
        public async Task<IActionResult> Index()
        {
            var regions = await _regionService.GetAllAsync();
            return View(regions);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegionDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _regionService.AddAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {
            var region = await _regionService.GetByIdAsync(id);
            if (region == null)
                return NotFound();

            return View(region);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RegionDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _regionService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            await _regionService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}



