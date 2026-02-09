using Business.Interfaces;
using Core.Dtos;
using Core.Entities;
using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public class MenuService : IMenuService
{
    private readonly ApplicationContext _context;

    public MenuService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<List<MenuDto>> GetAllAsync()
    {
        var menus = await _context.Menus
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync();

        return menus.Select(MapToDto).ToList();
    }

    public async Task<MenuDto?> GetByIdAsync(int id)
    {
        var menu = await _context.Menus.FindAsync(id);
        return menu != null ? MapToDto(menu) : null;
    }

    public async Task<List<MenuDto>> GetActiveMenusAsync()
    {
        var menus = await _context.Menus
            .Where(m => m.IsActive)
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync();

        return menus.Select(MapToDto).ToList();
    }

    public async Task<List<MenuDto>> GetMenusByRoleIdAsync(int roleId)
    {
        var menus = await _context.Menus
            .Include(m => m.MenuRoles)
            .Where(m => m.IsActive && m.MenuRoles.Any(mr => mr.RoleId == roleId))
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync();

        return menus.Select(MapToDto).ToList();
    }

    public async Task<List<MenuDto>> GetMenusByRoleIdsAsync(List<int> roleIds)
    {
        var menus = await _context.Menus
            .Include(m => m.MenuRoles)
            .Where(m => m.IsActive && m.MenuRoles.Any(mr => roleIds.Contains(mr.RoleId)))
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync();

        return menus.Select(MapToDto).ToList();
    }

    public async Task<int> CreateAsync(MenuDto dto)
    {
        var menu = new Menu
        {
            Name = dto.Name,
            Path = dto.Path,
            IconName = dto.IconName,
            DisplayOrder = dto.DisplayOrder,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.Now,
            CreatedBy = dto.CreatedBy
        };

        _context.Menus.Add(menu);
        await _context.SaveChangesAsync();

        return menu.Id;
    }

    public async Task<bool> UpdateAsync(MenuDto dto)
    {
        var menu = await _context.Menus.FindAsync(dto.Id);
        if (menu == null) return false;

        menu.Name = dto.Name;
        menu.Path = dto.Path;
        menu.IconName = dto.IconName;
        menu.DisplayOrder = dto.DisplayOrder;
        menu.IsActive = dto.IsActive;
        menu.UpdatedAt = DateTime.Now;
        menu.UpdatedBy = dto.UpdatedBy;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var menu = await _context.Menus.FindAsync(id);
        if (menu == null) return false;

        _context.Menus.Remove(menu);
        await _context.SaveChangesAsync();

        return true;
    }

    private static MenuDto MapToDto(Menu menu)
    {
        return new MenuDto
        {
            Id = menu.Id,
            Name = menu.Name,
            Path = menu.Path,
            IconName = menu.IconName,
            DisplayOrder = menu.DisplayOrder,
            IsActive = menu.IsActive,
            CreatedAt = menu.CreatedAt,
            CreatedBy = menu.CreatedBy,
            UpdatedAt = menu.UpdatedAt,
            UpdatedBy = menu.UpdatedBy
        };
    }
}
