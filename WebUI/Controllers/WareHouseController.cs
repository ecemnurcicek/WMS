using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace WebUI.Controllers
{
    public class WareHouseController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;

        public WareHouseController(IHttpClientFactory httpClientFactory, IUserService userService)
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
            ViewData["Title"] = "Depo Yönetimi";
            ViewData["UserName"] = user?.Name ?? "Kullanıcı";
            ViewData["UserEmail"] = user?.Email ?? "email@example.com";

            var response = await _httpClient.GetAsync("/api/warehouse");
            if (!response.IsSuccessStatusCode)
                return View(new List<WareHouseDto>());

            var wareHouses = await response.Content.ReadFromJsonAsync<IEnumerable<WareHouseDto>>();

            // Bölgeleri al
            var regionResponse = await _httpClient.GetAsync("/api/region");
            if (regionResponse.IsSuccessStatusCode)
            {
                var regions = await regionResponse.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>();
                ViewBag.Regions = regions ?? new List<RegionDto>();
            }

            return View(wareHouses ?? new List<WareHouseDto>());
        }

        [HttpGet]
        public async Task<IActionResult> CreateForm()
        {
            // Bölgeleri al
            var regionResponse = await _httpClient.GetAsync("/api/region");
            var regions = regionResponse.IsSuccessStatusCode
                ? await regionResponse.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>()
                : new List<RegionDto>();

            ViewBag.Regions = regions;
            ViewBag.Cities = new List<CityDto>();
            ViewBag.Towns = new List<TownDto>();
            ViewBag.Shops = new List<ShopDto>();

            var emptyWareHouse = new WareHouseDto { IsActive = true };
            return PartialView("_FormModal", emptyWareHouse);
        }

        [HttpGet]
        public async Task<IActionResult> EditForm(int id)
        {
            var response = await _httpClient.GetAsync($"/api/warehouse/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var wareHouse = await response.Content.ReadFromJsonAsync<WareHouseDto>();

            // Bölgeleri al
            var regionResponse = await _httpClient.GetAsync("/api/region");
            if (regionResponse.IsSuccessStatusCode)
            {
                var regions = await regionResponse.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>();
                ViewBag.Regions = regions;
            }

            // Edit modda seçili bölgeye göre şehirleri al
            if (wareHouse?.RegionId > 0)
            {
                var citiesResponse = await _httpClient.GetAsync($"/api/city/region/{wareHouse.RegionId}");
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
            if (wareHouse?.CityId > 0)
            {
                var townsResponse = await _httpClient.GetAsync($"/api/town/city/{wareHouse.CityId}");
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

            // Edit modda seçili ilçeye göre mağazaları al
            if (wareHouse?.TownId > 0)
            {
                var shopsResponse = await _httpClient.GetAsync($"/api/shop/town/{wareHouse.TownId}");
                if (shopsResponse.IsSuccessStatusCode)
                {
                    var shops = await shopsResponse.Content.ReadFromJsonAsync<IEnumerable<ShopDto>>();
                    ViewBag.Shops = shops;
                }
            }
            else
            {
                ViewBag.Shops = new List<ShopDto>();
            }

            return PartialView("_FormModal", wareHouse);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var response = await _httpClient.GetAsync($"/api/warehouse/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var wareHouse = await response.Content.ReadFromJsonAsync<WareHouseDto>();
            return PartialView("_DeleteModal", wareHouse);
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

        [HttpGet]
        public async Task<IActionResult> GetShopsByTown(int townId)
        {
            var response = await _httpClient.GetAsync($"/api/shop/town/{townId}");
            if (response.IsSuccessStatusCode)
            {
                var shops = await response.Content.ReadFromJsonAsync<IEnumerable<ShopDto>>();
                return Json(shops);
            }
            return Json(new List<ShopDto>());
        }

        [HttpPost]
        public async Task<IActionResult> CreateJson([FromForm] WareHouseDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/warehouse", dto);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Depo başarıyla eklendi." });
            
            return Json(new { success = false, message = "Bir hata oluştu." });
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromForm] WareHouseDto dto)
        {
            dto.Id = id;
            var response = await _httpClient.PutAsJsonAsync($"/api/warehouse/{id}", dto);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Depo başarıyla güncellendi." });
            
            return Json(new { success = false, message = "Bir hata oluştu." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteJson(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/warehouse/{id}");
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Depo başarıyla silindi." });
            
            return Json(new { success = false, message = "Bir hata oluştu." });
        }
    }
}
