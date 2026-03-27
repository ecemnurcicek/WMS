using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace WebUI.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;

        public UserController(IHttpClientFactory httpClientFactory, IUserService userService)
        {
            _httpClient = httpClientFactory.CreateClient("WebAPI");
            _userService = userService;
        }

        private bool IsAdmin()
        {
            var userRoles = HttpContext.Session.GetString("UserRoles") ?? "";
            return userRoles.Contains("Admin", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<IActionResult> Index()
        {
            var userIdObj = HttpContext.Session.GetInt32("UserId");
            if (!userIdObj.HasValue)
                return RedirectToAction("Login", "Account");

            if (!IsAdmin())
                return RedirectToAction("Index", "Dashboard");

            var user = await _userService.GetUserByIdAsync(userIdObj.Value);
            ViewData["Title"] = "Kullanıcı Yönetimi";
            ViewData["UserName"] = user?.Name ?? "Kullanıcı";
            ViewData["UserEmail"] = user?.Email ?? "email@example.com";

            var response = await _httpClient.GetAsync("/api/user");
            if (!response.IsSuccessStatusCode)
                return View(new List<UserDto>());

            var users = await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>();
            return View(users ?? new List<UserDto>());
        }

        [HttpGet]
        public async Task<IActionResult> CreateForm()
        {
            await LoadDropdowns();
            var emptyUser = new UserDto { IsActive = true };
            return PartialView("_FormModal", emptyUser);
        }

        [HttpGet]
        public async Task<IActionResult> EditForm(int id)
        {
            var response = await _httpClient.GetAsync($"/api/user/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            await LoadDropdowns();
            return PartialView("_FormModal", user);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var response = await _httpClient.GetAsync($"/api/user/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            return PartialView("_DeleteModal", user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateJson([FromForm] UserDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/user", dto);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Kullanıcı başarıyla eklendi." });

            var error = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, message = "Bir hata oluştu: " + error });
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromForm] UserDto dto)
        {
            dto.Id = id;
            var response = await _httpClient.PutAsJsonAsync($"/api/user/{id}", dto);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Kullanıcı başarıyla güncellendi." });

            var error = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, message = "Bir hata oluştu: " + error });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteJson(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/user/{id}");
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Kullanıcı başarıyla silindi." });

            return Json(new { success = false, message = "Bir hata oluştu." });
        }

        private async Task LoadDropdowns()
        {
            // Sadece aktif mağazaları al
            var shopResponse = await _httpClient.GetAsync("/api/shop/active");
            if (shopResponse.IsSuccessStatusCode)
            {
                var shops = await shopResponse.Content.ReadFromJsonAsync<IEnumerable<ShopDto>>();
                ViewBag.Shops = shops ?? new List<ShopDto>();
            }
            else
            {
                ViewBag.Shops = new List<ShopDto>();
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

            // Rolleri al
            var roleResponse = await _httpClient.GetAsync("/api/user/roles");
            if (roleResponse.IsSuccessStatusCode)
            {
                var rolesJson = await roleResponse.Content.ReadAsStringAsync();
                var roles = System.Text.Json.JsonSerializer.Deserialize<List<RoleItem>>(rolesJson,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                ViewBag.Roles = roles ?? new List<RoleItem>();
            }
            else
            {
                ViewBag.Roles = new List<RoleItem>();
            }
        }

        public class RoleItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
        }
    }
}
