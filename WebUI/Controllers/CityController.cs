using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace WebUI.Controllers
{
    public class CityController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;

        public CityController(IHttpClientFactory httpClientFactory, IUserService userService)
        {
            _httpClient = httpClientFactory.CreateClient("WebAPI");
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userIdObj = HttpContext.Session.GetInt32("UserId");
            if (!userIdObj.HasValue)
                return RedirectToAction("Login", "Account");

            var user = await _userService.GetUserByIdAsync(userIdObj.Value);

            ViewData["Title"] = "Şehir Yönetimi";
            ViewData["UserName"] = user?.Name ?? "Kullanıcı";
            ViewData["UserEmail"] = user?.Email ?? "email@example.com";

            var cityResponse = await _httpClient.GetAsync("/api/city");
            if (!cityResponse.IsSuccessStatusCode)
                return View(new List<CityDto>());

            var cities = await cityResponse.Content.ReadFromJsonAsync<IEnumerable<CityDto>>();

            var regionResponse = await _httpClient.GetAsync("/api/region");
            if (regionResponse.IsSuccessStatusCode)
            {
                var regions = await regionResponse.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>();
                ViewBag.Regions = regions ?? new List<RegionDto>();
            }

            return View(cities ?? new List<CityDto>());
        }

        [HttpGet]
        public async Task<IActionResult> CreateForm()
        {
            var regionResponse = await _httpClient.GetAsync("/api/region");
            var regions = regionResponse.IsSuccessStatusCode
                ? await regionResponse.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>()
                : new List<RegionDto>();

            ViewBag.Regions = regions;

            var emptyCity = new CityDto { IsActive = true };
            return PartialView("_FormModal", emptyCity);
        }

        [HttpGet]
        public async Task<IActionResult> EditForm(int id)
        {
            var cityResponse = await _httpClient.GetAsync($"/api/city/{id}");
            if (!cityResponse.IsSuccessStatusCode)
                return NotFound();

            var city = await cityResponse.Content.ReadFromJsonAsync<CityDto>();

            var regionResponse = await _httpClient.GetAsync("/api/region");
            if (regionResponse.IsSuccessStatusCode)
            {
                var regions = await regionResponse.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>();
                ViewBag.Regions = regions;
            }

            return PartialView("_FormModal", city);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var response = await _httpClient.GetAsync($"/api/city/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var city = await response.Content.ReadFromJsonAsync<CityDto>();
            return PartialView("_DeleteModal", city);
        }

        [HttpPost]
        public async Task<IActionResult> CreateJson([FromForm] CityDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/city", dto);

            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Şehir başarıyla eklendi." });

            return Json(new { success = false, message = "Bir hata oluştu." });
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromForm] CityDto dto)
        {
            dto.Id = id;
            var response = await _httpClient.PutAsJsonAsync($"/api/city/{id}", dto);

            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Şehir başarıyla güncellendi." });

            return Json(new { success = false, message = "Bir hata oluştu." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteJson(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/city/{id}");

            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Şehir başarıyla silindi." });

            return Json(new { success = false, message = "Bir hata oluştu." });
        }
    }
}





