using Core.Dtos;
using Data.Context;
using Business.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Business.Interfaces;

namespace Business.Services;

public class LoginService : ILoginService
{
    private readonly ApplicationContext _context;
    private readonly PasswordHasher<object> _passwordHasher;

    public LoginService(ApplicationContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<object>();
    }

    public async Task<UserLoginResultDto> LoginAsync(LoginDto loginDto)
    {
        // Validasyon
        if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
        { 
            throw new ValidationException(
                "Email ve şifre gereklidir.",
                nameof(LoginService),
                25
            );
        }

        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.IsActive);

        if (user == null)
        {
            throw new NotFoundException(
                "Geçersiz email veya şifre.",
                nameof(LoginService),
                35
            );
        }

        // Password verification using PasswordHasher
        var result = _passwordHasher.VerifyHashedPassword(null, user.Password, loginDto.Password);
        
        if (result == PasswordVerificationResult.Failed)
        {
            throw new ValidationException(
                "Geçersiz email veya şifre.",
                nameof(LoginService),
                43
            );
        }

        var loginResult = new UserLoginResultDto
        {
            Success = true,
            Message = "Başarıyla giriş yapıldı.",
            UserId = user.Id,
            UserName = user.Name,
            UserEmail = user.Email,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };

        return loginResult;
    }
}
