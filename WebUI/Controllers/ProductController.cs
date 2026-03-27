using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;

        public ProductController(IHttpClientFactory httpClientFactory, IUserService userService)
        {
            _httpClient = httpClientFactory.CreateClient("WebAPI");
            _userService = userService;
        }

        // Ana sayfa - Option bazlı listeleme
        public async Task<IActionResult> Index()
        {
            var userIdObj = HttpContext.Session.GetInt32("UserId");
            if (!userIdObj.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userService.GetUserByIdAsync(userIdObj.Value);
            ViewData["Title"] = "Ürün Yönetimi";
            ViewData["UserName"] = user?.Name ?? "Kullanıcı";
            ViewData["UserEmail"] = user?.Email ?? "email@example.com";
            ViewData["UserId"] = userIdObj.Value;
            ViewData["UserShopId"] = user?.ShopId;

            // Kullanıcının ShopId'si varsa sadece o mağazanın stokunu göster
            var shopId = user?.ShopId;
            var url = shopId.HasValue 
                ? $"/api/product/options?shopId={shopId.Value}" 
                : "/api/product/options";

            // Option bazlı listeleme
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return View(new List<ProductOptionDto>());

            var options = await response.Content.ReadFromJsonAsync<IEnumerable<ProductOptionDto>>();
            return View(options ?? new List<ProductOptionDto>());
        }

        // Option detayı - Beden listesi ile
        [HttpGet]
        public async Task<IActionResult> OptionDetailModal(string model, string color)
        {
            var userIdObj = HttpContext.Session.GetInt32("UserId");
            if (!userIdObj.HasValue)
                return Unauthorized();

            var user = await _userService.GetUserByIdAsync(userIdObj.Value);
            var shopId = user?.ShopId;

            var encodedModel = Uri.EscapeDataString(model ?? "");
            var encodedColor = Uri.EscapeDataString(color ?? "");
            
            var url = shopId.HasValue
                ? $"/api/product/option?model={encodedModel}&color={encodedColor}&shopId={shopId.Value}"
                : $"/api/product/option?model={encodedModel}&color={encodedColor}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var optionDetail = await response.Content.ReadFromJsonAsync<ProductOptionDetailDto>();

            // Brand listesini al
            var brandResponse = await _httpClient.GetAsync("/api/brand");
            var brands = brandResponse.IsSuccessStatusCode
                ? await brandResponse.Content.ReadFromJsonAsync<IEnumerable<BrandDto>>()
                : new List<BrandDto>();
            ViewBag.Brands = brands;

            // Region listesini al - raf seçimi için
            var regionResponse = await _httpClient.GetAsync("/api/region");
            var regions = regionResponse.IsSuccessStatusCode
                ? await regionResponse.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>()
                : new List<RegionDto>();
            ViewBag.Regions = regions;

            return PartialView("_OptionDetailModal", optionDetail);
        }

        // Option için stok özeti
        [HttpGet]
        public async Task<IActionResult> GetOptionStockSummary(string model, string color)
        {
            var userIdObj = HttpContext.Session.GetInt32("UserId");
            if (!userIdObj.HasValue)
                return Json(new List<ProductStockSummaryDto>());

            var user = await _userService.GetUserByIdAsync(userIdObj.Value);
            var shopId = user?.ShopId;

            var encodedModel = Uri.EscapeDataString(model ?? "");
            var encodedColor = Uri.EscapeDataString(color ?? "");
            
            var url = shopId.HasValue
                ? $"/api/product/option/stock?model={encodedModel}&color={encodedColor}&shopId={shopId.Value}"
                : $"/api/product/option/stock?model={encodedModel}&color={encodedColor}";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var stockSummary = await response.Content.ReadFromJsonAsync<IEnumerable<ProductStockSummaryDto>>();
                return Json(stockSummary);
            }
            return Json(new List<ProductStockSummaryDto>());
        }

        // Beden listesi
        [HttpGet]
        public async Task<IActionResult> GetSizesByOption(string model, string color)
        {
            var userIdObj = HttpContext.Session.GetInt32("UserId");
            if (!userIdObj.HasValue)
                return Json(new List<ProductSizeDto>());

            var user = await _userService.GetUserByIdAsync(userIdObj.Value);
            var shopId = user?.ShopId;

            var encodedModel = Uri.EscapeDataString(model ?? "");
            var encodedColor = Uri.EscapeDataString(color ?? "");
            
            var url = shopId.HasValue
                ? $"/api/product/option/sizes?model={encodedModel}&color={encodedColor}&shopId={shopId.Value}"
                : $"/api/product/option/sizes?model={encodedModel}&color={encodedColor}";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var sizes = await response.Content.ReadFromJsonAsync<IEnumerable<ProductSizeDto>>();
                return Json(sizes);
            }
            return Json(new List<ProductSizeDto>());
        }

        // Yeni ürün (beden) ekleme formu - Option'a beden ekleme
        [HttpGet]
        public async Task<IActionResult> SizeCreateForm(string model, string color)
        {
            // Brand listesini al
            var brandResponse = await _httpClient.GetAsync("/api/brand");
            var brands = brandResponse.IsSuccessStatusCode
                ? await brandResponse.Content.ReadFromJsonAsync<IEnumerable<BrandDto>>()
                : new List<BrandDto>();

            ViewBag.Brands = brands;

            var newSize = new ProductDto 
            { 
                Model = model,
                Color = color,
                IsActive = true 
            };
            return PartialView("_SizeFormModal", newSize);
        }

        // Beden düzenleme formu
        [HttpGet]
        public async Task<IActionResult> SizeEditForm(int id)
        {
            var response = await _httpClient.GetAsync($"/api/product/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var product = await response.Content.ReadFromJsonAsync<ProductDto>();

            // Brand listesini al
            var brandResponse = await _httpClient.GetAsync("/api/brand");
            if (brandResponse.IsSuccessStatusCode)
            {
                var brands = await brandResponse.Content.ReadFromJsonAsync<IEnumerable<BrandDto>>();
                ViewBag.Brands = brands;
            }
            else
            {
                ViewBag.Brands = new List<BrandDto>();
            }

            return PartialView("_SizeFormModal", product);
        }

        // Beden silme onayı
        [HttpGet]
        public async Task<IActionResult> SizeDeleteConfirm(int id)
        {
            var response = await _httpClient.GetAsync($"/api/product/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var product = await response.Content.ReadFromJsonAsync<ProductDto>();
            return PartialView("_SizeDeleteModal", product);
        }

        // Yeni option oluşturma formu
        [HttpGet]
        public async Task<IActionResult> CreateForm()
        {
            // Brand listesini al
            var brandResponse = await _httpClient.GetAsync("/api/brand");
            var brands = brandResponse.IsSuccessStatusCode
                ? await brandResponse.Content.ReadFromJsonAsync<IEnumerable<BrandDto>>()
                : new List<BrandDto>();

            ViewBag.Brands = brands;

            var emptyProduct = new ProductDto { IsActive = true };
            return PartialView("_FormModal", emptyProduct);
        }

        [HttpGet]
        public async Task<IActionResult> EditForm(int id)
        {
            var response = await _httpClient.GetAsync($"/api/product/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var product = await response.Content.ReadFromJsonAsync<ProductDto>();

            // Brand listesini al
            var brandResponse = await _httpClient.GetAsync("/api/brand");
            if (brandResponse.IsSuccessStatusCode)
            {
                var brands = await brandResponse.Content.ReadFromJsonAsync<IEnumerable<BrandDto>>();
                ViewBag.Brands = brands;
            }
            else
            {
                ViewBag.Brands = new List<BrandDto>();
            }

            return PartialView("_FormModal", product);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var response = await _httpClient.GetAsync($"/api/product/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var product = await response.Content.ReadFromJsonAsync<ProductDto>();
            return PartialView("_DeleteModal", product);
        }

        [HttpGet]
        public async Task<IActionResult> DetailModal(int id)
        {
            var response = await _httpClient.GetAsync($"/api/product/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var product = await response.Content.ReadFromJsonAsync<ProductDto>();

            // Region listesini al
            var regionResponse = await _httpClient.GetAsync("/api/region");
            var regions = regionResponse.IsSuccessStatusCode
                ? await regionResponse.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>()
                : new List<RegionDto>();

            ViewBag.Regions = regions;

            return PartialView("_DetailModal", product);
        }

        // ProductShelf için modal formları
        [HttpGet]
        public async Task<IActionResult> ProductShelfCreateForm(int productId)
        {
            var userIdObj = HttpContext.Session.GetInt32("UserId");
            if (!userIdObj.HasValue)
                return Unauthorized();

            var user = await _userService.GetUserByIdAsync(userIdObj.Value);
            var shopId = user?.ShopId;

            // Eğer kullanıcının ShopId'si varsa, mağaza bilgilerini al
            if (shopId.HasValue)
            {
                var shopResponse = await _httpClient.GetAsync($"/api/shop/{shopId.Value}");
                if (shopResponse.IsSuccessStatusCode)
                {
                    var shop = await shopResponse.Content.ReadFromJsonAsync<ShopDto>();
                    if (shop != null)
                    {
                        ViewBag.UserShopId = shopId.Value;
                        ViewBag.PreSelectedShopId = shop.Id;
                        ViewBag.PreSelectedTownId = shop.TownId;
                        ViewBag.PreSelectedCityId = shop.CityId;
                        ViewBag.PreSelectedRegionId = shop.RegionId;

                        // Depo ve raf listelerini yükle
                        var warehousesResponse = await _httpClient.GetAsync($"/api/warehouse/shop/{shopId.Value}");
                        var warehouses = warehousesResponse.IsSuccessStatusCode
                            ? await warehousesResponse.Content.ReadFromJsonAsync<IEnumerable<WareHouseDto>>()
                            : new List<WareHouseDto>();

                        ViewBag.Warehouses = warehouses;
                        ViewBag.Shelves = new List<ShelfDto>();
                        ViewBag.Regions = new List<RegionDto>();
                        ViewBag.Cities = new List<CityDto>();
                        ViewBag.Towns = new List<TownDto>();
                        ViewBag.Shops = new List<ShopDto>();

                        var emptyShelf = new ProductShelfDto { ProductId = productId, Quantity = 1, ShopId = shopId.Value };
                        return PartialView("_ProductShelfFormModal", emptyShelf);
                    }
                }
            }

            // Admin kullanıcılar için tüm dropdown'ları yükle
            var regionResponse = await _httpClient.GetAsync("/api/region");
            var regions = regionResponse.IsSuccessStatusCode
                ? await regionResponse.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>()
                : new List<RegionDto>();

            ViewBag.Regions = regions;
            ViewBag.Cities = new List<CityDto>();
            ViewBag.Towns = new List<TownDto>();
            ViewBag.Shops = new List<ShopDto>();
            ViewBag.Warehouses = new List<WareHouseDto>();
            ViewBag.Shelves = new List<ShelfDto>();
            ViewBag.UserShopId = null;

            var emptyShelfDto = new ProductShelfDto { ProductId = productId, Quantity = 1 };
            return PartialView("_ProductShelfFormModal", emptyShelfDto);
        }

        [HttpGet]
        public async Task<IActionResult> ProductShelfEditForm(int id)
        {
            var userIdObj = HttpContext.Session.GetInt32("UserId");
            if (!userIdObj.HasValue)
                return Unauthorized();

            var user = await _userService.GetUserByIdAsync(userIdObj.Value);
            var userShopId = user?.ShopId;

            var response = await _httpClient.GetAsync($"/api/product/shelf/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var productShelf = await response.Content.ReadFromJsonAsync<ProductShelfDto>();

            // Kullanıcının ShopId'si varsa ve kaydın ShopId'si farklıysa, erişim izni verme
            if (userShopId.HasValue && productShelf?.ShopId != userShopId.Value)
                return Unauthorized();

            ViewBag.UserShopId = userShopId;

            // Edit modda mevcut değerlere göre dropdown'ları doldur
            // Bölgeleri al
            var regionResponse = await _httpClient.GetAsync("/api/region");
            if (regionResponse.IsSuccessStatusCode)
            {
                var regions = await regionResponse.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>();
                ViewBag.Regions = regions;
            }
            else
            {
                ViewBag.Regions = new List<RegionDto>();
            }

            // Edit modda seçili bölgeye göre şehirleri al
            if (productShelf?.RegionId > 0)
            {
                var citiesResponse = await _httpClient.GetAsync($"/api/city/region/{productShelf.RegionId}");
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
            if (productShelf?.CityId > 0)
            {
                var townsResponse = await _httpClient.GetAsync($"/api/town/city/{productShelf.CityId}");
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
            if (productShelf?.TownId > 0)
            {
                var shopsResponse = await _httpClient.GetAsync($"/api/shop/town/{productShelf.TownId}");
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
            if (productShelf?.ShopId > 0)
            {
                var warehousesResponse = await _httpClient.GetAsync($"/api/warehouse/shop/{productShelf.ShopId}");
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

            // Edit modda seçili depoya göre rafları al
            if (productShelf?.WarehouseId > 0)
            {
                var shelvesResponse = await _httpClient.GetAsync($"/api/shelf/warehouse/{productShelf.WarehouseId}");
                if (shelvesResponse.IsSuccessStatusCode)
                {
                    var shelves = await shelvesResponse.Content.ReadFromJsonAsync<IEnumerable<ShelfDto>>();
                    ViewBag.Shelves = shelves;
                }
            }
            else
            {
                ViewBag.Shelves = new List<ShelfDto>();
            }

            return PartialView("_ProductShelfFormModal", productShelf);
        }

        [HttpGet]
        public async Task<IActionResult> ProductShelfDeleteConfirm(int id)
        {
            var response = await _httpClient.GetAsync($"/api/product/shelf/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var productShelf = await response.Content.ReadFromJsonAsync<ProductShelfDto>();
            return PartialView("_ProductShelfDeleteModal", productShelf);
        }

        // Cascade dropdowns
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

        [HttpGet]
        public async Task<IActionResult> GetShelvesByWarehouse(int warehouseId)
        {
            var response = await _httpClient.GetAsync($"/api/shelf/warehouse/{warehouseId}");
            if (response.IsSuccessStatusCode)
            {
                var shelves = await response.Content.ReadFromJsonAsync<IEnumerable<ShelfDto>>();
                return Json(shelves);
            }
            return Json(new List<ShelfDto>());
        }

        [HttpGet]
        public async Task<IActionResult> GetProductShelves(int productId)
        {
            var userIdObj = HttpContext.Session.GetInt32("UserId");
            if (!userIdObj.HasValue)
                return Json(new List<ProductShelfDetailDto>());

            var user = await _userService.GetUserByIdAsync(userIdObj.Value);
            var shopId = user?.ShopId;

            var url = shopId.HasValue
                ? $"/api/product/{productId}/shelves?shopId={shopId.Value}"
                : $"/api/product/{productId}/shelves";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var shelves = await response.Content.ReadFromJsonAsync<IEnumerable<ProductShelfDetailDto>>();
                return Json(shelves);
            }
            return Json(new List<ProductShelfDetailDto>());
        }

        // CRUD Operations
        [HttpPost]
        public async Task<IActionResult> CreateJson([FromForm] ProductDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/product", dto);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Ürün başarıyla eklendi." });
            
            return Json(new { success = false, message = "Bir hata oluştu." });
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromForm] ProductDto dto)
        {
            dto.Id = id;
            var response = await _httpClient.PutAsJsonAsync($"/api/product/{id}", dto);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Ürün başarıyla güncellendi." });
            
            return Json(new { success = false, message = "Bir hata oluştu." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteJson(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/product/{id}");
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Ürün başarıyla silindi." });
            
            return Json(new { success = false, message = "Bir hata oluştu." });
        }

        // ProductShelf CRUD Operations
        [HttpPost]
        public async Task<IActionResult> CreateProductShelf([FromForm] ProductShelfDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/product/shelf", dto);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Ürün rafı başarıyla eklendi." });
            
            return Json(new { success = false, message = "Bir hata oluştu." });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProductShelf(int id, [FromForm] ProductShelfDto dto)
        {
            dto.Id = id;
            var response = await _httpClient.PutAsJsonAsync($"/api/product/shelf/{id}", dto);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Ürün rafı başarıyla güncellendi." });
            
            return Json(new { success = false, message = "Bir hata oluştu." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductShelf(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/product/shelf/{id}");
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Ürün rafı başarıyla silindi." });
            
            return Json(new { success = false, message = "Bir hata oluştu." });
        }

        // Quick Transfer endpoints
        [HttpGet]
        public async Task<IActionResult> GetShopWithMaxStock(int productId, int? excludeShopId = null)
        {
            var url = $"/api/product/{productId}/max-stock-shop";
            if (excludeShopId.HasValue)
                url += $"?excludeShopId={excludeShopId.Value}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = "Bu ürün için başka mağazalarda stok bulunamadı." });
            }

            var shopStock = await response.Content.ReadFromJsonAsync<Core.Dtos.ShopStockDto>();
            return Json(new { success = true, data = shopStock });
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuickTransfer([FromBody] QuickTransferRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/transfer/quick", request);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = "Transfer talebi oluşturulamadı." });
            }

            var result = await response.Content.ReadFromJsonAsync<QuickTransferResponse>();
            return Json(new { success = true, data = result });
        }
    }

    // Request/Response models
    public class QuickTransferRequest
    {
        public int FromShopId { get; set; }
        public int ToShopId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int UserId { get; set; }
    }

    public class QuickTransferResponse
    {
        public int TransferId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

