using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebUI.Controllers
{
    public class CityController : Controller
    {
        private readonly ICityService _cityService;
        private readonly IUserService _userService;
        private readonly ILogger<CityController> _logger;

        public CityController(ICityService cityService, IUserService userService, ILogger<CityController> logger)
        {
            _cityService = cityService;
            _userService = userService;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            // Set user info in ViewData
            var userIdObj = HttpContext.Session.GetInt32("UserId");
            if (userIdObj.HasValue)
            {
                try
                {
                    var user = await _userService.GetUserByIdAsync(userIdObj.Value);
                    ViewData["UserName"] = user?.Name ?? "Kullanıcı";
                    ViewData["UserEmail"] = user?.Email ?? "email@example.com";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Kullanıcı bilgisi getirilirken hata oluştu");
                    ViewData["UserName"] = "Kullanıcı";
                    ViewData["UserEmail"] = "email@example.com";
                }
            }

            var cities = await _cityService.GetAllAsync();
            return View(cities);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CityDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);
            await _cityService.AddAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {
            var city = await _cityService.GetByIdAsync(id);
            if (city == null)
                return NotFound();

            return View(city);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CityDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _cityService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            await _cityService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}



