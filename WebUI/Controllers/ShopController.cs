using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace WebUI.Controllers
{
    public class ShopController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;

        public ShopController(IHttpClientFactory httpClientFactory, IUserService userService)
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
            ViewData["Title"] = "Mağaza Yönetimi";
            ViewData["UserName"] = user?.Name ?? "Kullanıcı";
            ViewData["UserEmail"] = user?.Email ?? "email@example.com";

            var response = await _httpClient.GetAsync("/api/shop");
            if (!response.IsSuccessStatusCode)
                return View(new List<ShopDto>());

            var shops = await response.Content.ReadFromJsonAsync<IEnumerable<ShopDto>>();

            // Bölgeleri al
            var regionResponse = await _httpClient.GetAsync("/api/region");
            if (regionResponse.IsSuccessStatusCode)
            {
                var regions = await regionResponse.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>();
                ViewBag.Regions = regions ?? new List<RegionDto>();
            }

            // Markaları al
            var brandResponse = await _httpClient.GetAsync("/api/brand");
            if (brandResponse.IsSuccessStatusCode)
            {
                var brands = await brandResponse.Content.ReadFromJsonAsync<IEnumerable<BrandDto>>();
                ViewBag.Brands = brands ?? new List<BrandDto>();
            }

            return View(shops ?? new List<ShopDto>());
        }

        [HttpGet]
        public async Task<IActionResult> CreateForm()
        {
            // Bölgeleri al
            var regionResponse = await _httpClient.GetAsync("/api/region");
            var regions = regionResponse.IsSuccessStatusCode
                ? await regionResponse.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>()
                : new List<RegionDto>();

            // Markaları al
            var brandResponse = await _httpClient.GetAsync("/api/brand");
            var brands = brandResponse.IsSuccessStatusCode
                ? await brandResponse.Content.ReadFromJsonAsync<IEnumerable<BrandDto>>()
                : new List<BrandDto>();

            ViewBag.Regions = regions;
            ViewBag.Brands = brands;
            ViewBag.Cities = new List<CityDto>();
            ViewBag.Towns = new List<TownDto>();

            var emptyShop = new ShopDto { IsActive = true };
            return PartialView("_FormModal", emptyShop);
        }

        [HttpGet]
        public async Task<IActionResult> EditForm(int id)
        {
            var response = await _httpClient.GetAsync($"/api/shop/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var shop = await response.Content.ReadFromJsonAsync<ShopDto>();

            // Bölgeleri al
            var regionResponse = await _httpClient.GetAsync("/api/region");
            if (regionResponse.IsSuccessStatusCode)
            {
                var regions = await regionResponse.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>();
                ViewBag.Regions = regions;
            }

            // Markaları al
            var brandResponse = await _httpClient.GetAsync("/api/brand");
            if (brandResponse.IsSuccessStatusCode)
            {
                var brands = await brandResponse.Content.ReadFromJsonAsync<IEnumerable<BrandDto>>();
                ViewBag.Brands = brands;
            }

            // Edit modda seçili bölgeye göre şehirleri al
            if (shop?.RegionId > 0)
            {
                var citiesResponse = await _httpClient.GetAsync($"/api/city/region/{shop.RegionId}");
                if (citiesResponse.IsSuccessStatusCode)
                {
                    var cities = await citiesResponse.Content.ReadFromJsonAsync<IEnumerable<CityDto>>();
                    ViewBag.Cities = cities;
                }
            }
            else
            {
                ViewBag.Cities = new List<CityDto>();
            }

            // Edit modda seçili şehire göre ilçeleri al
            if (shop?.CityId > 0)
            {
                var townsResponse = await _httpClient.GetAsync($"/api/town/city/{shop.CityId}");
                if (townsResponse.IsSuccessStatusCode)
                {
                    var towns = await townsResponse.Content.ReadFromJsonAsync<IEnumerable<TownDto>>();
                    ViewBag.Towns = towns;
                }
            }
            else
            {
                ViewBag.Towns = new List<TownDto>();
            }

            return PartialView("_FormModal", shop);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var response = await _httpClient.GetAsync($"/api/shop/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var shop = await response.Content.ReadFromJsonAsync<ShopDto>();
            return PartialView("_DeleteModal", shop);
        }

        [HttpGet]
        public async Task<IActionResult> GetCitiesByRegion(int regionId)
        {
            var response = await _httpClient.GetAsync($"/api/city/region/{regionId}");
            if (response.IsSuccessStatusCode)
            {
                var cities = await response.Content.ReadFromJsonAsync<IEnumerable<CityDto>>();
                return Json(cities);
            }
            return Json(new List<CityDto>());
        }

        [HttpGet]
        public async Task<IActionResult> GetTownsByCity(int cityId)
        {
            var response = await _httpClient.GetAsync($"/api/town/city/{cityId}");
            if (response.IsSuccessStatusCode)
            {
                var towns = await response.Content.ReadFromJsonAsync<IEnumerable<TownDto>>();
                return Json(towns);
            }
            return Json(new List<TownDto>());
        }

        [HttpPost]
        public async Task<IActionResult> CreateJson([FromForm] ShopDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/shop", dto);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Mağaza başarıyla eklendi." });
            
            return Json(new { success = false, message = "Bir hata oluştu." });
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromForm] ShopDto dto)
        {
            dto.Id = id;
            var response = await _httpClient.PutAsJsonAsync($"/api/shop/{id}", dto);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Mağaza başarıyla güncellendi." });
            
            return Json(new { success = false, message = "Bir hata oluştu." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteJson(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/shop/{id}");
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Mağaza başarıyla silindi." });
            
            return Json(new { success = false, message = "Bir hata oluştu." });
        }
    }
}


