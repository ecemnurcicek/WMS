using Business.Exceptions;
using Business.Repositories;
using Core.Entities;
using Data.Context;

namespace Business.Interfaces;

/// <summary>
/// User yönetimi servis interface
/// </summary>
public interface IUserService
{
    Task<User> GetUserByIdAsync(int userId);
    Task<User> GetUserByEmailAsync(string email);
    Task<List<User>> GetAllUsersAsync();
    Task<List<Core.Dtos.UserDto>> GetAllUserDtosAsync();
    Task<Core.Dtos.UserDto?> GetUserDtoByIdAsync(int userId);
    Task<User> CreateUserAsync(User user);
    Task<User> UpdateUserAsync(User user);
    Task DeleteUserAsync(int userId);
}
