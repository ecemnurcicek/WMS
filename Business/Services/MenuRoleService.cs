using Business.Interfaces;
using Core.Dtos;
using Core.Entities;
using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public class MenuRoleService : IMenuRoleService
{
    private readonly ApplicationContext _context;

    public MenuRoleService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<List<MenuRoleDto>> GetAllAsync()
    {
        var menuRoles = await _context.MenuRoles
            .Include(mr => mr.Menu)
            .Include(mr => mr.Role)
            .ToListAsync();

        return menuRoles.Select(MapToDto).ToList();
    }

    public async Task<MenuRoleDto?> GetByIdAsync(int id)
    {
        var menuRole = await _context.MenuRoles
            .Include(mr => mr.Menu)
            .Include(mr => mr.Role)
            .FirstOrDefaultAsync(mr => mr.Id == id);

        return menuRole != null ? MapToDto(menuRole) : null;
    }

    public async Task<List<MenuRoleDto>> GetByMenuIdAsync(int menuId)
    {
        var menuRoles = await _context.MenuRoles
            .Include(mr => mr.Menu)
            .Include(mr => mr.Role)
            .Where(mr => mr.MenuId == menuId)
            .ToListAsync();

        return menuRoles.Select(MapToDto).ToList();
    }

    public async Task<List<MenuRoleDto>> GetByRoleIdAsync(int roleId)
    {
        var menuRoles = await _context.MenuRoles
            .Include(mr => mr.Menu)
            .Include(mr => mr.Role)
            .Where(mr => mr.RoleId == roleId)
            .ToListAsync();

        return menuRoles.Select(MapToDto).ToList();
    }

    public async Task<int> CreateAsync(MenuRoleDto dto)
    {
        var menuRole = new MenuRole
        {
            MenuId = dto.MenuId,
            RoleId = dto.RoleId,
            CreatedAt = DateTime.Now,
            CreatedBy = dto.CreatedBy
        };

        _context.MenuRoles.Add(menuRole);
        await _context.SaveChangesAsync();

        return menuRole.Id;
    }

    public async Task<bool> UpdateAsync(MenuRoleDto dto)
    {
        var menuRole = await _context.MenuRoles.FindAsync(dto.Id);
        if (menuRole == null) return false;

        menuRole.MenuId = dto.MenuId;
        menuRole.RoleId = dto.RoleId;
        menuRole.UpdatedAt = DateTime.Now;
        menuRole.UpdatedBy = dto.UpdatedBy;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var menuRole = await _context.MenuRoles.FindAsync(id);
        if (menuRole == null) return false;

        _context.MenuRoles.Remove(menuRole);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteByMenuIdAsync(int menuId)
    {
        var menuRoles = await _context.MenuRoles
            .Where(mr => mr.MenuId == menuId)
            .ToListAsync();

        _context.MenuRoles.RemoveRange(menuRoles);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AssignRolesToMenuAsync(int menuId, List<int> roleIds, int? createdBy)
    {
        // First delete existing assignments
        await DeleteByMenuIdAsync(menuId);

        // Then create new assignments
        foreach (var roleId in roleIds)
        {
            var menuRole = new MenuRole
            {
                MenuId = menuId,
                RoleId = roleId,
                CreatedAt = DateTime.Now,
                CreatedBy = createdBy
            };

            _context.MenuRoles.Add(menuRole);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    private static MenuRoleDto MapToDto(MenuRole menuRole)
    {
        return new MenuRoleDto
        {
            Id = menuRole.Id,
            MenuId = menuRole.MenuId,
            RoleId = menuRole.RoleId,
            CreatedAt = menuRole.CreatedAt,
            CreatedBy = menuRole.CreatedBy,
            UpdatedAt = menuRole.UpdatedAt,
            UpdatedBy = menuRole.UpdatedBy,
            MenuName = menuRole.Menu?.Name,
            RoleName = menuRole.Role?.Name
        };
    }
}
