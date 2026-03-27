using Core.Dtos;

namespace Business.Interfaces;

/// <summary>
/// Dashboard işlemleri için service interface
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Dashboard ana verilerini getirir
    /// </summary>
    Task<DashboardDataDto> GetDashboardDataAsync(int userId);
}
