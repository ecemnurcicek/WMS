using Core.Dtos;
using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationContext _context;

    public DashboardService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<DashboardDataDto> GetDashboardDataAsync(int userId)
    {
        // Kullanıcı bilgilerini getir
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new ArgumentException("Kullanıcı bulunamadı", nameof(userId));
        }

        // Dashboard istatistiklerini getir
        var totalProducts = await _context.Products.CountAsync(p => p.IsActive);
        var totalWarehouses = await _context.Warehouses.CountAsync(w => w.IsActive);
        var totalTransfers = await _context.Transfers.CountAsync();
        var activeTransfers = await _context.Transfers
            .CountAsync(t => t.IsSent == false);

        return new DashboardDataDto
        {
            UserId = user.Id,
            UserName = user.Name,
            UserEmail = user.Email,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
            TotalProducts = totalProducts,
            TotalWarehouses = totalWarehouses,
            TotalTransfers = totalTransfers,
            ActiveTransfers = activeTransfers,
            LastLoginTime = DateTime.Now
        };
    }
}
