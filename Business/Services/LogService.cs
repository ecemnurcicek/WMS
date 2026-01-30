using Business.Exceptions;
using Business.Interfaces;
using Core.Entities;
using Data.Context;

namespace Business.Services;

/// <summary>
/// Hataları ve logları veritabanına kaydeden servis
/// </summary>
public class LogService : ILogService
{
    private readonly ApplicationContext _context;

    public LogService(ApplicationContext context)
    {
        _context = context;
    }

    /// <summary>
    /// BusinessException ile fırlatılan hataları loglama
    /// </summary>
    public async Task LogExceptionAsync(BusinessException exception, int? userId = null)
    {
        try
        {
            var logEntry = new Log
            {
                Description = $"[{exception.GetType().Name}] {exception.Message}" +
                              (exception.InnerException != null ? $"\nİç Hata: {exception.InnerException.Message}" : ""),
                ControllerName = exception.ControllerName ?? "Unknown",
                LineNumber = exception.LineNumber,
                CreatedBy = userId,
                CreatedAt = exception.OccurredAt
            };

            _context.Logs.Add(logEntry);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Logging başarısız olursa console'a yazdır
            Console.Error.WriteLine($"Log kaydı başarısız: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Orijinal Exception: {exception.Message}");
        }
    }

    /// <summary>
    /// Bilgi mesajını loglama
    /// </summary>
    public async Task LogInfoAsync(string message, string? controllerName = null, int? userId = null)
    {
        try
        {
            var logEntry = new Log
            {
                Description = $"[INFO] {message}",
                ControllerName = controllerName ?? "Unknown",
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            };

            _context.Logs.Add(logEntry);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Log kaydı başarısız: {ex.Message}");
        }
    }

    /// <summary>
    /// Uyarı mesajını loglama
    /// </summary>
    public async Task LogWarningAsync(string message, string? controllerName = null, int? userId = null)
    {
        try
        {
            var logEntry = new Log
            {
                Description = $"[WARNING] {message}",
                ControllerName = controllerName ?? "Unknown",
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            };

            _context.Logs.Add(logEntry);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Log kaydı başarısız: {ex.Message}");
        }
    }

    /// <summary>
    /// Genel hata loglama
    /// </summary>
    public async Task LogErrorAsync(string message, Exception? exception = null, string? controllerName = null, int? userId = null)
    {
        try
        {
            var description = $"[ERROR] {message}";
            if (exception != null)
            {
                description += $"\nException: {exception.Message}\nStackTrace: {exception.StackTrace}";
            }

            var logEntry = new Log
            {
                Description = description,
                ControllerName = controllerName ?? "Unknown",
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            };

            _context.Logs.Add(logEntry);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Log kaydı başarısız: {ex.Message}");
        }
    }
}

