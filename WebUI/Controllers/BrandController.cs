using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace WebUI.Controllers
{
    public class BrandController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;

        public BrandController(IHttpClientFactory httpClientFactory, IUserService userService)
        {
            _httpClient = httpClientFactory.CreateClient("WebAPI");
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
            ViewData["Title"] = "Marka Yönetimi";
            ViewData["UserName"] = user?.Name ?? "Kullanıcı";
            ViewData["UserEmail"] = user?.Email ?? "email@example.com";

            var response = await _httpClient.GetAsync("/api/brand");
            if (!response.IsSuccessStatusCode)
                return View(new List<BrandDto>());

            var brands = await response.Content.ReadFromJsonAsync<IEnumerable<BrandDto>>();
            return View(brands ?? new List<BrandDto>());
        }

        [HttpGet]
        public IActionResult CreateForm()
        {
            var emptyBrand = new BrandDto { IsActive = true };
            return PartialView("_FormModal", emptyBrand);
        }

        [HttpGet]
        public async Task<IActionResult> EditForm(int id)
        {
            var response = await _httpClient.GetAsync($"/api/brand/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var brand = await response.Content.ReadFromJsonAsync<BrandDto>();
            return PartialView("_FormModal", brand);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var response = await _httpClient.GetAsync($"/api/brand/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var brand = await response.Content.ReadFromJsonAsync<BrandDto>();
            return PartialView("_DeleteModal", brand);
        }

        [HttpPost]
        public async Task<IActionResult> CreateJson([FromForm] BrandDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/brand", dto);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Marka başarıyla eklendi." });
            
            return Json(new { success = false, message = "Bir hata oluştu." });
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromForm] BrandDto dto)
        {
            dto.Id = id;
            var response = await _httpClient.PutAsJsonAsync($"/api/brand/{id}", dto);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Marka başarıyla güncellendi." });
            
            return Json(new { success = false, message = "Bir hata oluştu." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteJson(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/brand/{id}");
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Marka başarıyla silindi." });
            
            return Json(new { success = false, message = "Bir hata oluştu." });
        }
    }
}
