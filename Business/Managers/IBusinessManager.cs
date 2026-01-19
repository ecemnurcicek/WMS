using Business.Services;

namespace Business.Managers;

/// <summary>
/// Business katmanı yönetiminin merkezi noktası
/// Tüm business işlemleri bu manager aracılığıyla yapılır
/// </summary>
public interface IBusinessManager
{
    ILoginService LoginService { get; }
    ILogService LogService { get; }
}
