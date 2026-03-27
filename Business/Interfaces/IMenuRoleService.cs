using Core.Dtos;

namespace Business.Interfaces;

public interface IMenuRoleService
{
    Task<List<MenuRoleDto>> GetAllAsync();
    Task<MenuRoleDto?> GetByIdAsync(int id);
    Task<List<MenuRoleDto>> GetByMenuIdAsync(int menuId);
    Task<List<MenuRoleDto>> GetByRoleIdAsync(int roleId);
    Task<int> CreateAsync(MenuRoleDto dto);
    Task<bool> UpdateAsync(MenuRoleDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteByMenuIdAsync(int menuId);
    Task<bool> AssignRolesToMenuAsync(int menuId, List<int> roleIds, int? createdBy);
}
