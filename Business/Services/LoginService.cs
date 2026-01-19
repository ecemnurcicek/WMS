using Core.Dtos;
using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public class LoginService : ILoginService
{
    private readonly ApplicationContext _context;

    public LoginService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<UserLoginResultDto> LoginAsync(LoginDto loginDto)
    {
        var result = new UserLoginResultDto();

        try
        {
            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                result.Success = false;
                result.Message = "Email ve şifre gereklidir.";
                return result;
            }

            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.IsActive);

            if (user == null)
            {
                result.Success = false;
                result.Message = "Geçersiz email veya şifre.";
                return result;
            }

            // Password verification (should use proper hashing in production)
            if (user.Password != loginDto.Password)
            {
                result.Success = false;
                result.Message = "Geçersiz email veya şifre.";
                return result;
            }

            result.Success = true;
            result.Message = "Başarıyla giriş yapıldı.";
            result.UserId = user.Id;
            result.UserName = user.Name;
            result.UserEmail = user.Email;
            result.Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = $"Bir hata oluştu: {ex.Message}";
            return result;
        }
    }
}
