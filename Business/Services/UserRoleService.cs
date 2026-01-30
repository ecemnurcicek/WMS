using Business.Interfaces;
using Core.Dtos;
using Core.Entities;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly ApplicationContext _context;

        public UserRoleService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<UserRoleDto>> GetAllAsync(bool pActive = false)
        {
            return await _context.UserRoles
                .Select(ur => new UserRoleDto
                {
                    Id = ur.Id,
                    UserId = ur.UserId,
                    RoleId = ur.RoleId,
                    CreatedAt = ur.CreatedAt,
                    CreatedBy = ur.CreatedBy,
                    UpdatedAt = ur.UpdatedAt,
                    UpdatedBy = ur.UpdatedBy
                })
                .ToListAsync();
        }

        public async Task<UserRoleDto?> GetByIdAsync(int pId)
        {
            var userRole = await _context.UserRoles.FindAsync(pId);
            if (userRole == null)
                return null;

            return new UserRoleDto
            {
                Id = userRole.Id,
                UserId = userRole.UserId,
                RoleId = userRole.RoleId,
                CreatedAt = userRole.CreatedAt,
                CreatedBy = userRole.CreatedBy,
                UpdatedAt = userRole.UpdatedAt,
                UpdatedBy = userRole.UpdatedBy
            };
        }

        public async Task<UserRoleDto> AddAsync(UserRoleDto pModel)
        {
            var entity = new UserRole
            {
                UserId = pModel.UserId,
                RoleId = pModel.RoleId,
                CreatedAt = DateTime.Now,
                CreatedBy = pModel.CreatedBy
            };

            _context.UserRoles.Add(entity);
            await _context.SaveChangesAsync();

            pModel.Id = entity.Id;
            pModel.CreatedAt = entity.CreatedAt;

            return pModel;
        }

        public async Task<bool> UpdateAsync(UserRoleDto pModel)
        {
            var userRole = await _context.UserRoles.FindAsync(pModel.Id);
            if (userRole == null)
                throw new Exception("Kullanıcı rolü bulunamadı");

            userRole.UserId = pModel.UserId;
            userRole.RoleId = pModel.RoleId;
            userRole.UpdatedAt = DateTime.Now;
            userRole.UpdatedBy = pModel.UpdatedBy;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int pId)
        {
            var userRole = await _context.UserRoles.FindAsync(pId);
            if (userRole == null)
                return false;

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}



