using Business.Services;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILoginService _loginService;
        private readonly ILogService _logService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILoginService loginService, ILogService logService, ILogger<AccountController> logger)
        {
            _loginService = loginService;
            _logService = logService;
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

            // Exception handling otomatik olarak ExceptionHandlingMiddleware tarafından yapılacak
            var result = await _loginService.LoginAsync(loginDto);

            // Store user info in session
            HttpContext.Session.SetInt32("UserId", result.UserId.Value);
            HttpContext.Session.SetString("UserName", result.UserName);
            HttpContext.Session.SetString("UserEmail", result.UserEmail);
            HttpContext.Session.SetString("UserRoles", string.Join(",", result.Roles));

            await _logService.LogInfoAsync($"Kullanıcı {result.UserEmail} başarıyla giriş yaptı.", nameof(AccountController), result.UserId);
            _logger.LogInformation($"Kullanıcı {result.UserEmail} başarıyla giriş yaptı.");
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            
            HttpContext.Session.Clear();
            
            if (userId.HasValue && !string.IsNullOrEmpty(userEmail))
            {
                await _logService.LogInfoAsync($"Kullanıcı {userEmail} çıkış yaptı.", nameof(AccountController), userId);
            }
            
            _logger.LogInformation("Kullanıcı çıkış yaptı.");
            return RedirectToAction("Index", "Home");
        }
    }
}
