using Business.Exceptions;
using Business.Interfaces;
using Business.Repositories;
using Core.Entities;
using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

/// <summary>
/// User yönetimi işlemlerini gerçekleştiren servis
/// </summary>
public class UserService : IUserService
{
    private readonly ApplicationContext _context;
    private readonly IRepository<User> _userRepository;

    public UserService(ApplicationContext context)
    {
        _context = context;
        _userRepository = new Repository<User>(context);
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        if (userId <= 0)
        {
            throw new ValidationException(
                "Kullanıcı ID'si 0'dan büyük olmalıdır.",
                nameof(UserService),
                18
            );
        }

        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                throw new NotFoundException(
                    $"Kullanıcı (ID: {userId}) bulunamadı.",
                    nameof(UserService),
                    27
                );
            }

            return user;
        }
        catch (BusinessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DataAccessException(
                "Kullanıcı sorgulanırken hata oluştu.",
                ex
            );
        }
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ValidationException(
                "Email boş olamaz.",
                nameof(UserService),
                47
            );
        }

        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            
            if (user == null)
            {
                throw new NotFoundException(
                    $"Email '{email}' ile kayıtlı kullanıcı bulunamadı.",
                    nameof(UserService),
                    57
                );
            }

            return user;
        }
        catch (BusinessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DataAccessException(
                "Kullanıcı email ile sorgulanırken hata oluştu.",
                ex
            );
        }
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        try
        {
            return await _userRepository.GetAllAsync();
        }
        catch (BusinessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DataAccessException(
                "Kullanıcı listesi alınırken hata oluştu.",
                ex
            );
        }
    }

    public async Task<List<Core.Dtos.UserDto>> GetAllUserDtosAsync()
    {
        try
        {
            return await _context.Users
                .Include(u => u.Shop).ThenInclude(s => s!.Brand)
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new Core.Dtos.UserDto
                {
                    Id = u.Id,
                    ShopId = u.ShopId,
                    Name = u.Name,
                    Email = u.Email ?? "",
                    Phone = u.Phone ?? "",
                    BirthDate = u.BirthDate ?? DateTime.MinValue,
                    Password = "",
                    IsActive = u.IsActive,
                    CreateAt = u.CreatedAt,
                    CreateBy = u.CreatedBy ?? 0,
                    UpdateAt = u.UpdatedAt,
                    UpdateBy = u.UpdatedBy,
                    RoleId = u.UserRoles.Any() ? u.UserRoles.First().RoleId : (int?)null,
                    ShopName = u.Shop != null ? u.Shop.Name : "",
                    BrandName = u.Shop != null && u.Shop.Brand != null ? u.Shop.Brand.Name : "",
                    RoleName = u.UserRoles.Any() 
                        ? string.Join(", ", u.UserRoles.Select(ur => ur.Role.Name))
                        : "Rol Yok"
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new DataAccessException(
                "Kullanıcı listesi alınırken hata oluştu.",
                ex
            );
        }
    }

    public async Task<Core.Dtos.UserDto?> GetUserDtoByIdAsync(int userId)
    {
        try
        {
            return await _context.Users
                .Include(u => u.Shop).ThenInclude(s => s!.Brand)
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Where(u => u.Id == userId)
                .Select(u => new Core.Dtos.UserDto
                {
                    Id = u.Id,
                    ShopId = u.ShopId,
                    Name = u.Name,
                    Email = u.Email ?? "",
                    Phone = u.Phone ?? "",
                    BirthDate = u.BirthDate ?? DateTime.MinValue,
                    Password = "",
                    IsActive = u.IsActive,
                    CreateAt = u.CreatedAt,
                    CreateBy = u.CreatedBy ?? 0,
                    UpdateAt = u.UpdatedAt,
                    UpdateBy = u.UpdatedBy,
                    RoleId = u.UserRoles.Any() ? u.UserRoles.First().RoleId : (int?)null,
                    ShopName = u.Shop != null ? u.Shop.Name : "",
                    BrandName = u.Shop != null && u.Shop.Brand != null ? u.Shop.Brand.Name : "",
                    RoleName = u.UserRoles.Any() 
                        ? string.Join(", ", u.UserRoles.Select(ur => ur.Role.Name))
                        : "Rol Yok"
                })
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw new DataAccessException(
                $"Kullanıcı (ID: {userId}) sorgulanırken hata oluştu.",
                ex
            );
        }
    }

    public async Task<User> CreateUserAsync(User user)
    {
        if (user == null)
        {
            throw new ValidationException(
                "Kullanıcı nesnesi null olamaz.",
                nameof(UserService),
                83
            );
        }

        if (string.IsNullOrWhiteSpace(user.Email))
        {
            throw new ValidationException(
                "Kullanıcı email'i boş olamaz.",
                nameof(UserService),
                89
            );
        }

        // Email'in benzersiz olup olmadığını kontrol et
        try
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                throw new BusinessRuleException(
                    $"Email '{user.Email}' zaten kullanılmaktadır.",
                    nameof(UserService),
                    99
                );
            }

            return await _userRepository.AddAsync(user);
        }
        catch (BusinessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DataAccessException(
                "Kullanıcı oluşturulurken hata oluştu.",
                ex
            );
        }
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        if (user == null || user.Id <= 0)
        {
            throw new ValidationException(
                "Geçersiz kullanıcı nesnesi.",
                nameof(UserService),
                119
            );
        }

        try
        {
            // Kullanıcının var olup olmadığını kontrol et
            var existingUser = await _userRepository.GetByIdAsync(user.Id);
            if (existingUser == null)
            {
                throw new NotFoundException(
                    $"Güncellenecek kullanıcı (ID: {user.Id}) bulunamadı.",
                    nameof(UserService),
                    132
                );
            }

            return await _userRepository.UpdateAsync(user);
        }
        catch (BusinessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DataAccessException(
                "Kullanıcı güncellenirken hata oluştu.",
                ex
            );
        }
    }

    public async Task DeleteUserAsync(int userId)
    {
        if (userId <= 0)
        {
            throw new ValidationException(
                "Kullanıcı ID'si 0'dan büyük olmalıdır.",
                nameof(UserService),
                153
            );
        }

        try
        {
            await _userRepository.DeleteAsync(userId);
        }
        catch (BusinessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DataAccessException(
                "Kullanıcı silinirken hata oluştu.",
                ex
            );
        }
    }
}
