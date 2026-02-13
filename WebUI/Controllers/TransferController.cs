using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace WebUI.Controllers
{
    public class TransferController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;

        public TransferController(IHttpClientFactory httpClientFactory, IUserService userService)
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
            var userRoles = HttpContext.Session.GetString("UserRoles") ?? "";

            ViewData["Title"] = "Transfer Yönetimi";
            ViewData["UserName"] = user?.Name ?? "Kullanıcı";
            ViewData["UserEmail"] = user?.Email ?? "email@example.com";
            ViewData["UserRoles"] = userRoles;
            ViewData["UserShopId"] = user?.ShopId;
            ViewData["UserBrandId"] = user?.BrandId;
            ViewData["UserId"] = userIdObj.Value;

            // Kullanıcı rollerine göre transfer listesi
            List<TransferDto> transfers;
            
            var isBrandManager = IsBrandManager(userRoles);
            
            // Admin, bölge sorumlusu tüm transferleri görebilir
            // Marka Sorumlusu sadece kendi markasına ait transferleri görebilir
            if (IsAdminOrRegionManager(userRoles))
            {
                var response = await _httpClient.GetAsync("/api/transfer");
                transfers = response.IsSuccessStatusCode 
                    ? await response.Content.ReadFromJsonAsync<List<TransferDto>>() ?? new List<TransferDto>()
                    : new List<TransferDto>();

                ViewBag.OutgoingTransfers = new List<TransferDto>();
                ViewBag.IncomingTransfers = new List<TransferDto>();
            }
            else if (isBrandManager && user?.BrandId.HasValue == true)
            {
                // Marka Sorumlusu: Sadece kendi markasına ait transferleri görebilir
                var response = await _httpClient.GetAsync($"/api/transfer/brand/{user.BrandId}");
                transfers = response.IsSuccessStatusCode 
                    ? await response.Content.ReadFromJsonAsync<List<TransferDto>>() ?? new List<TransferDto>()
                    : new List<TransferDto>();

                ViewBag.OutgoingTransfers = new List<TransferDto>();
                ViewBag.IncomingTransfers = new List<TransferDto>();
            }
            else if (user?.ShopId.HasValue == true)
            {
                // Göndereceğim: ToShopId = myShopId (İptal edilenler hariç)
                var outgoingResponse = await _httpClient.GetAsync($"/api/transfer/shop/{user.ShopId}/outgoing");
                var outgoing = outgoingResponse.IsSuccessStatusCode
                    ? await outgoingResponse.Content.ReadFromJsonAsync<List<TransferDto>>() ?? new List<TransferDto>()
                    : new List<TransferDto>();

                // Beklediğim: FromShopId = myShopId
                var incomingResponse = await _httpClient.GetAsync($"/api/transfer/shop/{user.ShopId}/incoming");
                var incoming = incomingResponse.IsSuccessStatusCode
                    ? await incomingResponse.Content.ReadFromJsonAsync<List<TransferDto>>() ?? new List<TransferDto>()
                    : new List<TransferDto>();

                ViewBag.OutgoingTransfers = outgoing;
                ViewBag.IncomingTransfers = incoming;
                transfers = new List<TransferDto>();
            }
            else
            {
                transfers = new List<TransferDto>();
                ViewBag.OutgoingTransfers = new List<TransferDto>();
                ViewBag.IncomingTransfers = new List<TransferDto>();
            }

            // Markaları al
            var brandResponse = await _httpClient.GetAsync("/api/brand");
            if (brandResponse.IsSuccessStatusCode)
            {
                var brands = await brandResponse.Content.ReadFromJsonAsync<IEnumerable<BrandDto>>();
                ViewBag.Brands = brands ?? new List<BrandDto>();
            }
            else
            {
                ViewBag.Brands = new List<BrandDto>();
            }

            return View(transfers);
        }

        [HttpGet]
        public async Task<IActionResult> CreateForm()
        {
            var userIdObj = HttpContext.Session.GetInt32("UserId");
            if (!userIdObj.HasValue)
                return Unauthorized();

            var user = await _userService.GetUserByIdAsync(userIdObj.Value);
            var userRoles = HttpContext.Session.GetString("UserRoles") ?? "";

            // Markaları al
            var brandResponse = await _httpClient.GetAsync("/api/brand");
            var brands = brandResponse.IsSuccessStatusCode
                ? await brandResponse.Content.ReadFromJsonAsync<IEnumerable<BrandDto>>()
                : new List<BrandDto>();
            ViewBag.Brands = brands;

            // Admin/Manager: Tüm mağazalar gösterilir
            // Marka Sorumlusu: Sadece kendi markasının mağazaları
            // Normal kullanıcı: FromShopId otomatik atanır, sadece ToShop seçer
            ViewBag.IsAdminOrManager = IsAdminOrManager(userRoles);
            ViewBag.IsBrandManager = IsBrandManager(userRoles);
            ViewBag.UserShopId = user?.ShopId;
            ViewBag.UserBrandId = user?.BrandId;
            ViewBag.UserId = userIdObj.Value;

            // Kullanıcının mağazası varsa, o mağazayı al
            if (user?.ShopId.HasValue == true)
            {
                var shopResponse = await _httpClient.GetAsync($"/api/shop/{user.ShopId}");
                if (shopResponse.IsSuccessStatusCode)
                {
                    var userShop = await shopResponse.Content.ReadFromJsonAsync<ShopDto>();
                    ViewBag.UserShop = userShop;
                }
            }

            // Ürünleri al (transfer detayı için)
            var productResponse = await _httpClient.GetAsync("/api/product");
            var products = productResponse.IsSuccessStatusCode
                ? await productResponse.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>()
                : new List<ProductDto>();
            ViewBag.Products = products;

            var emptyTransfer = new TransferDto();
            return PartialView("_FormModal", emptyTransfer);
        }

        [HttpGet]
        public async Task<IActionResult> DetailForm(int id)
        {
            var response = await _httpClient.GetAsync($"/api/transfer/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var transfer = await response.Content.ReadFromJsonAsync<TransferDto>();
            
            var userRoles = HttpContext.Session.GetString("UserRoles") ?? "";
            ViewBag.IsAdminOrManager = IsAdminOrManager(userRoles);
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");

            var detailUser = await _userService.GetUserByIdAsync(HttpContext.Session.GetInt32("UserId") ?? 0);
            ViewBag.UserShopId = detailUser?.ShopId;

            return PartialView("_DetailModal", transfer);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var response = await _httpClient.GetAsync($"/api/transfer/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var transfer = await response.Content.ReadFromJsonAsync<TransferDto>();
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            
            return PartialView("_DeleteModal", transfer);
        }

        [HttpGet]
        public async Task<IActionResult> StatusConfirm(int id, int status)
        {
            var response = await _httpClient.GetAsync($"/api/transfer/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var transfer = await response.Content.ReadFromJsonAsync<TransferDto>();
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.NewStatus = status;
            
            return PartialView("_StatusModal", transfer);
        }

        [HttpGet]
        public async Task<IActionResult> GetShopsByBrand(int brandId)
        {
            var response = await _httpClient.GetAsync("/api/shop");
            if (!response.IsSuccessStatusCode)
                return Json(new List<ShopDto>());

            var allShops = await response.Content.ReadFromJsonAsync<List<ShopDto>>();
            var filteredShops = allShops?.Where(s => s.BrandId == brandId && s.IsActive).ToList();
            
            return Json(filteredShops ?? new List<ShopDto>());
        }

        [HttpGet]
        public async Task<IActionResult> SearchProducts(string term)
        {
            var response = await _httpClient.GetAsync("/api/product");
            if (!response.IsSuccessStatusCode)
                return Json(new List<ProductDto>());

            var allProducts = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
            var filtered = allProducts?
                .Where(p => p.IsActive && 
                    (p.Model.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                     p.Ean.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                     p.Color.Contains(term, StringComparison.OrdinalIgnoreCase)))
                .Take(20)
                .ToList();

            return Json(filtered ?? new List<ProductDto>());
        }

        private bool IsAdminOrManager(string userRoles)
        {
            var roles = userRoles.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return roles.Any(r => 
                r.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                r.Equals("Bölge Sorumlusu", StringComparison.OrdinalIgnoreCase) ||
                r.Equals("Marka Sorumlusu", StringComparison.OrdinalIgnoreCase) ||
                r.Equals("RegionManager", StringComparison.OrdinalIgnoreCase) ||
                r.Equals("BrandManager", StringComparison.OrdinalIgnoreCase));
        }

        private bool IsAdminOrRegionManager(string userRoles)
        {
            var roles = userRoles.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return roles.Any(r => 
                r.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                r.Equals("Bölge Sorumlusu", StringComparison.OrdinalIgnoreCase) ||
                r.Equals("RegionManager", StringComparison.OrdinalIgnoreCase));
        }

        private bool IsBrandManager(string userRoles)
        {
            var roles = userRoles.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return roles.Any(r => 
                r.Equals("Marka Sorumlusu", StringComparison.OrdinalIgnoreCase) ||
                r.Equals("BrandManager", StringComparison.OrdinalIgnoreCase));
        }
    }
}
