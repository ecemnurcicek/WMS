using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.ViewComponents;

public class SidebarViewComponent : ViewComponent
{
    private readonly IMenuService _menuService;

    public SidebarViewComponent(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var roleIdsString = HttpContext.Session.GetString("UserRoleIds") ?? "";
        var roleIds = string.IsNullOrEmpty(roleIdsString) 
            ? new List<int>() 
            : roleIdsString.Split(',').Select(int.Parse).ToList();

        var menus = roleIds.Any()
            ? await _menuService.GetMenusByRoleIdsAsync(roleIds)
            : new List<Core.Dtos.MenuDto>();

        // Report ve Settings menülerini kaldır
        menus = menus.Where(m => 
            !m.Path.Equals("/Report", StringComparison.OrdinalIgnoreCase) && 
            !m.Path.Equals("/Settings", StringComparison.OrdinalIgnoreCase))
            .ToList();

        return View(menus);
    }
}
