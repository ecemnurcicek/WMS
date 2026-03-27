using Core.Dtos;

namespace Business.Interfaces;

public interface IMenuService
{
    Task<List<MenuDto>> GetAllAsync();
    Task<MenuDto?> GetByIdAsync(int id);
    Task<List<MenuDto>> GetActiveMenusAsync();
    Task<List<MenuDto>> GetMenusByRoleIdAsync(int roleId);
    Task<List<MenuDto>> GetMenusByRoleIdsAsync(List<int> roleIds);
    Task<int> CreateAsync(MenuDto dto);
    Task<bool> UpdateAsync(MenuDto dto);
    Task<bool> DeleteAsync(int id);
}
