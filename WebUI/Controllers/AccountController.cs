using Business.Services;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILoginService _loginService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILoginService loginService, ILogger<AccountController> logger)
        {
            _loginService = loginService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            var result = await _loginService.LoginAsync(loginDto);

            if (result.Success)
            {
                // Store user info in session
                HttpContext.Session.SetInt32("UserId", result.UserId.Value);
                HttpContext.Session.SetString("UserName", result.UserName);
                HttpContext.Session.SetString("UserEmail", result.UserEmail);
                HttpContext.Session.SetString("UserRoles", string.Join(",", result.Roles));

                _logger.LogInformation($"Kullanıcı {result.UserEmail} başarıyla giriş yaptı.");
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", result.Message);
            _logger.LogWarning($"Başarısız giriş denemesi: {result.Message}");
            return View(loginDto);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            _logger.LogInformation("Kullanıcı çıkış yaptı.");
            return RedirectToAction("Index", "Home");
        }
    }
}
