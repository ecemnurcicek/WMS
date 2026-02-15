using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Json;
using Core.Dtos;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("WebAPI");
        }

        public async Task<IActionResult> Index()
        {
            // Marka ve Mağaza listelerini çek
            try
            {
                var brandsResponse = await _httpClient.GetAsync("/api/brand");
                var brands = brandsResponse.IsSuccessStatusCode
                    ? await brandsResponse.Content.ReadFromJsonAsync<List<BrandDto>>() ?? new List<BrandDto>()
                    : new List<BrandDto>();

                ViewBag.Brands = brands;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading brands and shops");
                ViewBag.Brands = new List<BrandDto>();
            }

            return View();
        }

        [HttpGet]
        public IActionResult CheckLoginAndRedirect()
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            
            if (userId.HasValue)
            {
                // User is logged in, redirect to Dashboard
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                // User is not logged in, redirect to Login
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchProducts(string? query, int? brandId)
        {
            try
            {
                var parameters = new List<string>();
                if (!string.IsNullOrWhiteSpace(query)) parameters.Add($"query={Uri.EscapeDataString(query)}");
                if (brandId.HasValue) parameters.Add($"brandId={brandId.Value}");

                var url = $"/api/product/search?{string.Join("&", parameters)}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Content(content, "application/json");
                }

                return Json(new { success = false, message = "API hatası" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products");
                return Json(new { success = false, message = "Arama sırasında hata oluştu" });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
