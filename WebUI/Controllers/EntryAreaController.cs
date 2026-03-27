using Business.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers;

public class EntryAreaController : Controller
{
    private readonly IEntryAreaService _entryAreaService;
    private readonly IUserService _userService;
    private readonly IShelfService _shelfService;

    public EntryAreaController(
        IEntryAreaService entryAreaService, 
        IUserService userService,
        IShelfService shelfService)
    {
        _entryAreaService = entryAreaService;
        _userService = userService;
        _shelfService = shelfService;
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

        ViewData["Title"] = "Giriş Alanı - Raf Ataması";
        ViewData["UserName"] = user?.Name ?? "Kullanıcı";
        ViewData["UserEmail"] = user?.Email ?? "email@example.com";
        ViewData["UserRoles"] = userRoles;
        ViewData["UserShopId"] = user?.ShopId;
        ViewData["UserId"] = userIdObj.Value;

        if (user?.ShopId == null)
        {
            TempData["Error"] = "Kullanıcıya ait mağaza bulunamadı.";
            return View(new List<EntryAreaItemDto>());
        }

        // Bekleyen ürünleri getir
        var items = await _entryAreaService.GetPendingItemsByShopIdAsync(user.ShopId.Value);

        // Mağazanın raflarını getir (dropdown için)
        var shelves = await _shelfService.GetAllAsync();
        var shopShelves = shelves.Where(s => s.ShopId == user.ShopId.Value).ToList();
        ViewBag.Shelves = shopShelves;

        return View(items);
    }

    [HttpPost]
    public async Task<IActionResult> AssignShelf(int transferDetailId, int productId, int shelfId, int quantity)
    {
        try
        {
            var userIdObj = HttpContext.Session.GetInt32("UserId");
            if (!userIdObj.HasValue)
            {
                return Json(new { success = false, message = "Oturum bulunamadı" });
            }

            await _entryAreaService.AssignShelfAsync(transferDetailId, productId, shelfId, quantity, userIdObj.Value);

            return Json(new { success = true, message = "Raf ataması başarıyla yapıldı" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
