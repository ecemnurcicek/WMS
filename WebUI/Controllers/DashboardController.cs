using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
    /// <summary>
    /// Dashboard sayfası kontrolcüsü - Sadece login yapılan kullanıcılara açık
    /// </summary>
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogService _logService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardService dashboardService, ILogService logService, ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logService = logService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Session'dan kullanıcı ID'sini al
            var userIdObj = HttpContext.Session.GetInt32("UserId");
            
            // Eğer session'da kullanıcı yok, login sayfasına yönlendir
            if (!userIdObj.HasValue)
            {
                _logger.LogWarning("Dashboard'a yetkisiz erişim denemesi");
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var dashboardData = await _dashboardService.GetDashboardDataAsync(userIdObj.Value);
                await _logService.LogInfoAsync($"Kullanıcı {dashboardData.UserEmail} Dashboard'ı açtı.", nameof(DashboardController), userIdObj.Value);
                
                return View(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dashboard verisi getirilirken hata oluştu");
                return RedirectToAction("Login", "Account");
            }
        }
    }
}
