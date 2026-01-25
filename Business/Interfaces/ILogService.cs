using Business.Exceptions;

namespace Business.Interfaces;

/// <summary>
/// Log kaydı işlemleri için interface
/// </summary>
public interface ILogService
{
    /// <summary>
    /// Exception kaydını veritabanına loglama
    /// </summary>
    Task LogExceptionAsync(BusinessException exception, int? userId = null);

    /// <summary>
    /// Bilgi mesajını loglama
    /// </summary>
    Task LogInfoAsync(string message, string? controllerName = null, int? userId = null);

    /// <summary>
    /// Uyarı mesajını loglama
    /// </summary>
    Task LogWarningAsync(string message, string? controllerName = null, int? userId = null);

    /// <summary>
    /// Genel hata loglama
    /// </summary>
    Task LogErrorAsync(string message, Exception? exception = null, string? controllerName = null, int? userId = null);
}
