using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace WebUI.Controllers
{
    public class TownController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;

        public TownController(IHttpClientFactory httpClientFactory, IUserService userService)
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

            ViewData["Title"] = "İlçe Yönetimi";
            ViewData["UserName"] = user?.Name ?? "Kullanıcı";
            ViewData["UserEmail"] = user?.Email ?? "email@example.com";

            var townResponse = await _httpClient.GetAsync("/api/town");
            if (!townResponse.IsSuccessStatusCode)
                return View(new List<TownDto>());

            var towns = await townResponse.Content.ReadFromJsonAsync<IEnumerable<TownDto>>();

            // Bölgeleri al
            var regionResponse = await _httpClient.GetAsync("/api/region");
            if (regionResponse.IsSuccessStatusCode)
            {
                var regions = await regionResponse.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>();
                ViewBag.Regions = regions ?? new List<RegionDto>();
            }

            // Şehirleri al
            var cityResponse = await _httpClient.GetAsync("/api/city");
            if (cityResponse.IsSuccessStatusCode)
            {
                var cities = await cityResponse.Content.ReadFromJsonAsync<IEnumerable<CityDto>>();
                ViewBag.Cities = cities ?? new List<CityDto>();
            }

            return View(towns ?? new List<TownDto>());
        }

        [HttpGet]
        public async Task<IActionResult> CreateForm()
        {
            var regionResponse = await _httpClient.GetAsync("/api/region");
            var regions = regionResponse.IsSuccessStatusCode
                ? await regionResponse.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>()
                : new List<RegionDto>();

            var cityResponse = await _httpClient.GetAsync("/api/city");
            var cities = cityResponse.IsSuccessStatusCode
                ? await cityResponse.Content.ReadFromJsonAsync<IEnumerable<CityDto>>()
                : new List<CityDto>();

            ViewBag.Regions = regions;
            ViewBag.Cities = cities;

            var emptyTown = new TownDto { IsActive = true };
            return PartialView("_FormModal", emptyTown);
        }

        [HttpGet]
        public async Task<IActionResult> EditForm(int id)
        {
            var townResponse = await _httpClient.GetAsync($"/api/town/{id}");
            if (!townResponse.IsSuccessStatusCode)
                return NotFound();

            var town = await townResponse.Content.ReadFromJsonAsync<TownDto>();

            var regionResponse = await _httpClient.GetAsync("/api/region");
            if (regionResponse.IsSuccessStatusCode)
            {
                var regions = await regionResponse.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>();
                ViewBag.Regions = regions;
            }

            var cityResponse = await _httpClient.GetAsync("/api/city");
            if (cityResponse.IsSuccessStatusCode)
            {
                var cities = await cityResponse.Content.ReadFromJsonAsync<IEnumerable<CityDto>>();
                ViewBag.Cities = cities;
            }

            return PartialView("_FormModal", town);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var response = await _httpClient.GetAsync($"/api/town/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var town = await response.Content.ReadFromJsonAsync<TownDto>();
            return PartialView("_DeleteModal", town);
        }

        [HttpGet]
        public async Task<IActionResult> GetCitiesByRegion(int regionId)
        {
            var response = await _httpClient.GetAsync($"/api/city/region/{regionId}");
            if (!response.IsSuccessStatusCode)
                return Json(new List<CityDto>());

            var cities = await response.Content.ReadFromJsonAsync<IEnumerable<CityDto>>();
            return Json(cities);
        }

        [HttpPost]
        public async Task<IActionResult> CreateJson([FromForm] TownDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/town", dto);

            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "İlçe başarıyla eklendi." });

            return Json(new { success = false, message = "Bir hata oluştu." });
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromForm] TownDto dto)
        {
            dto.Id = id;
            var response = await _httpClient.PutAsJsonAsync($"/api/town/{id}", dto);

            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "İlçe başarıyla güncellendi." });

            return Json(new { success = false, message = "Bir hata oluştu." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteJson(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/town/{id}");

            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "İlçe başarıyla silindi." });

            return Json(new { success = false, message = "Bir hata oluştu." });
        }
    }
}


