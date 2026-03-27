using Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IUserRoleService
    {
        Task<List<UserRoleDto>> GetAllAsync(bool pActive = false);
        Task<UserRoleDto?> GetByIdAsync(int pId);

        Task<UserRoleDto> AddAsync(UserRoleDto pModel);
        Task<bool> UpdateAsync(UserRoleDto pModel);
        Task<bool> DeleteAsync(int pId);
    }
}
