using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace WebUI.Controllers
{
    public class ShelfController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;

        public ShelfController(IHttpClientFactory httpClientFactory, IUserService userService)
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
            ViewData["Title"] = "Raf Yönetimi";
            ViewData["UserName"] = user?.Name ?? "Kullanıcı";
            ViewData["UserEmail"] = user?.Email ?? "email@example.com";

            var response = await _httpClient.GetAsync("/api/shelf");
            if (!response.IsSuccessStatusCode)
                return View(new List<ShelfDto>());

            var shelves = await response.Content.ReadFromJsonAsync<IEnumerable<ShelfDto>>();

            // Bölgeleri al
            var regionResponse = await _httpClient.GetAsync("/api/region");
            if (regionResponse.IsSuccessStatusCode)
            {
                var regions = await regionResponse.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>();
                ViewBag.Regions = regions ?? new List<RegionDto>();
            }

            return View(shelves ?? new List<ShelfDto>());
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
            ViewBag.Warehouses = new List<WareHouseDto>();

            var emptyShelf = new ShelfDto { IsActive = true };
            return PartialView("_FormModal", emptyShelf);
        }

        [HttpGet]
        public async Task<IActionResult> EditForm(int id)
        {
            var response = await _httpClient.GetAsync($"/api/shelf/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var shelf = await response.Content.ReadFromJsonAsync<ShelfDto>();

            // Bölgeleri al
            var regionResponse = await _httpClient.GetAsync("/api/region");
            if (regionResponse.IsSuccessStatusCode)
            {
                var regions = await regionResponse.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>();
                ViewBag.Regions = regions;
            }

            // Edit modda seçili bölgeye göre şehirleri al
            if (shelf?.RegionId > 0)
            {
                var citiesResponse = await _httpClient.GetAsync($"/api/city/region/{shelf.RegionId}");
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
            if (shelf?.CityId > 0)
            {
                var townsResponse = await _httpClient.GetAsync($"/api/town/city/{shelf.CityId}");
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
            if (shelf?.TownId > 0)
            {
                var shopsResponse = await _httpClient.GetAsync($"/api/shop/town/{shelf.TownId}");
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

            // Edit modda seçili mağazaya göre depoları al
            if (shelf?.ShopId > 0)
            {
                var warehousesResponse = await _httpClient.GetAsync($"/api/warehouse/shop/{shelf.ShopId}");
                if (warehousesResponse.IsSuccessStatusCode)
                {
                    var warehouses = await warehousesResponse.Content.ReadFromJsonAsync<IEnumerable<WareHouseDto>>();
                    ViewBag.Warehouses = warehouses;
                }
            }
            else
            {
                ViewBag.Warehouses = new List<WareHouseDto>();
            }

            return PartialView("_FormModal", shelf);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var response = await _httpClient.GetAsync($"/api/shelf/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var shelf = await response.Content.ReadFromJsonAsync<ShelfDto>();
            return PartialView("_DeleteModal", shelf);
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

        [HttpGet]
        public async Task<IActionResult> GetWarehousesByShop(int shopId)
        {
            var response = await _httpClient.GetAsync($"/api/warehouse/shop/{shopId}");
            if (response.IsSuccessStatusCode)
            {
                var warehouses = await response.Content.ReadFromJsonAsync<IEnumerable<WareHouseDto>>();
                return Json(warehouses);
            }
            return Json(new List<WareHouseDto>());
        }

        [HttpPost]
        public async Task<IActionResult> CreateJson([FromForm] ShelfDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/shelf", dto);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Raf başarıyla eklendi." });
            
            return Json(new { success = false, message = "Bir hata oluştu." });
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromForm] ShelfDto dto)
        {
            dto.Id = id;
            var response = await _httpClient.PutAsJsonAsync($"/api/shelf/{id}", dto);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Raf başarıyla güncellendi." });
            
            return Json(new { success = false, message = "Bir hata oluştu." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteJson(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/shelf/{id}");
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Raf başarıyla silindi." });
            
            return Json(new { success = false, message = "Bir hata oluştu." });
        }
    }
}
