using Business.Interfaces;
using Business.Utilities;
using Core.Dtos;
using Core.Entities;
using Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ApplicationContext _context;

        public UserController(IUserService userService, ApplicationContext context)
        {
            _userService = userService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUserDtosAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetUserDtoByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.Set<Role>()
                .Where(r => r.IsActive)
                .OrderBy(r => r.Name)
                .Select(r => new { r.Id, r.Name })
                .ToListAsync();
            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDto dto)
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                ShopId = dto.ShopId,
                BrandId = dto.BrandId,
                BirthDate = dto.BirthDate != DateTime.MinValue ? dto.BirthDate : null,
                Password = PasswordHasherUtil.HashPassword(dto.Password),
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Now
            };

            var created = await _userService.CreateUserAsync(user);

            // Rol ataması
            if (dto.RoleId.HasValue && dto.RoleId.Value > 0)
            {
                var userRole = new UserRole
                {
                    UserId = created.Id,
                    RoleId = dto.RoleId.Value,
                    CreatedAt = DateTime.Now
                };
                await _context.Set<UserRole>().AddAsync(userRole);
                await _context.SaveChangesAsync();
            }

            return Created(nameof(GetById), new { id = created.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserDto dto)
        {
            var existingUser = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (existingUser == null)
                return NotFound();

            existingUser.Name = dto.Name;
            existingUser.Email = dto.Email;
            existingUser.Phone = dto.Phone;
            existingUser.ShopId = dto.ShopId;
            existingUser.BrandId = dto.BrandId;
            existingUser.BirthDate = dto.BirthDate != DateTime.MinValue ? dto.BirthDate : null;
            existingUser.IsActive = dto.IsActive;
            existingUser.UpdatedAt = DateTime.Now;

            // Şifre değiştirildiyse güncelle
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                existingUser.Password = PasswordHasherUtil.HashPassword(dto.Password);
            }

            await _context.SaveChangesAsync();

            // Rol güncelleme
            if (dto.RoleId.HasValue && dto.RoleId.Value > 0)
            {
                // Mevcut rolleri sil
                var existingRoles = _context.Set<UserRole>().Where(ur => ur.UserId == id);
                _context.Set<UserRole>().RemoveRange(existingRoles);

                // Yeni rol ekle
                var userRole = new UserRole
                {
                    UserId = id,
                    RoleId = dto.RoleId.Value,
                    CreatedAt = DateTime.Now
                };
                await _context.Set<UserRole>().AddAsync(userRole);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Önce kullanıcının rollerini sil
            var userRoles = _context.Set<UserRole>().Where(ur => ur.UserId == id);
            _context.Set<UserRole>().RemoveRange(userRoles);
            await _context.SaveChangesAsync();

            await _userService.DeleteUserAsync(id);
            return Ok();
        }
    }
}
