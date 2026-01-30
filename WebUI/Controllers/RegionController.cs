using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WebUI.Controllers
{
    public class RegionController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;

        public RegionController(IHttpClientFactory httpClientFactory, IUserService userService)
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
            ViewData["Title"] = "Bölge Yönetimi";
            ViewData["UserName"] = user?.Name ?? "Kullanıcı";
            ViewData["UserEmail"] = user?.Email ?? "email@example.com";

            var response = await _httpClient.GetAsync("/api/region");
            if (!response.IsSuccessStatusCode)
                return View(new List<RegionDto>());

            var regions = await response.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>();
            return View(regions ?? new List<RegionDto>());
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
            var response = await _httpClient.GetAsync($"/api/region/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var region = await response.Content.ReadFromJsonAsync<RegionDto>();
            return PartialView("_FormModal", region);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var response = await _httpClient.GetAsync($"/api/region/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var region = await response.Content.ReadFromJsonAsync<RegionDto>();
            return PartialView("_DeleteModal", region);
        }

        [HttpPost]
        public async Task<IActionResult> CreateJson([FromForm] RegionDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/region", dto);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Bölge başarıyla eklendi." });
            
            return Json(new { success = false, message = "Bir hata oluştu." });
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromForm] RegionDto dto)
        {
            dto.Id = id;
            var response = await _httpClient.PutAsJsonAsync($"/api/region/{id}", dto);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Bölge başarıyla güncellendi." });
            
            return Json(new { success = false, message = "Bir hata oluştu." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteJson(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/region/{id}");
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Bölge başarıyla silindi." });
            
            return Json(new { success = false, message = "Bir hata oluştu." });
        }
    }
}




