using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebUI.Controllers
{
    public class RegionController : Controller
    {
        private readonly IRegionService _regionService;
        private readonly IUserService _userService;

        public RegionController(IRegionService regionService, IUserService userService)
        {
            _regionService = regionService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userIdObj = HttpContext.Session.GetInt32("UserId");
            if (!userIdObj.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userService.GetUserByIdAsync(userIdObj.Value);
            ViewData["Title"] = "Bölge Yönetimi";
            ViewData["UserName"] = user?.Name ?? "Kullanıcı";
            ViewData["UserEmail"] = user?.Email ?? "email@example.com";

            var regions = await _regionService.GetAllAsync();
            return View(regions);
        }

        [HttpGet]
        public IActionResult CreateForm()
        {
            var emptyRegion = new RegionDto { IsActive = true };
            return PartialView("_FormModal", emptyRegion);
        }

        [HttpGet]
        public async Task<IActionResult> EditForm(int id)
        {
            var region = await _regionService.GetByIdAsync(id);
            if (region == null)
                return NotFound();
            return PartialView("_FormModal", region);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var region = await _regionService.GetByIdAsync(id);
            if (region == null)
                return NotFound();
            return PartialView("_DeleteModal", region);
        }

        [HttpPost]
        public async Task<IActionResult> CreateJson([FromForm] RegionDto dto)
        {
            await _regionService.AddAsync(dto);
            return Json(new { success = true, message = "Bölge başarıyla eklendi." });
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromForm] RegionDto dto)
        {
            dto.Id = id;
            await _regionService.UpdateAsync(dto);
            return Json(new { success = true, message = "Bölge başarıyla güncellendi." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteJson(int id)
        {
            await _regionService.DeleteAsync(id);
            return Json(new { success = true, message = "Bölge başarıyla silindi." });
        }
    }
}



