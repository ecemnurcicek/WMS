using Business.Exceptions;
using Business.Services;
using System.Net;

namespace WebUI.Middleware;

/// <summary>
/// Business katmanından fırlatılan tüm exceptionları yakalayan ve loglayan middleware
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ILogService logService)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception, logService);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogService logService)
    {
        context.Response.ContentType = "application/json";

        var controllerName = context.Request.RouteValues["controller"]?.ToString() ?? "Unknown";
        var userId = context.User?.FindFirst("UserId")?.Value;
        int? userIdInt = null;

        if (int.TryParse(userId, out var parsedUserId))
        {
            userIdInt = parsedUserId;
        }

        switch (exception)
        {
            case ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                _ = logService.LogExceptionAsync(validationEx, userIdInt);
                return context.Response.WriteAsJsonAsync(new
                {
                    error = "Validasyon Hatası",
                    message = validationEx.Message,
                    statusCode = context.Response.StatusCode
                });

            case NotFoundException notFoundEx:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                _ = logService.LogExceptionAsync(notFoundEx, userIdInt);
                return context.Response.WriteAsJsonAsync(new
                {
                    error = "Kayıt Bulunamadı",
                    message = notFoundEx.Message,
                    statusCode = context.Response.StatusCode
                });

            case DataAccessException dataAccessEx:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                _ = logService.LogExceptionAsync(dataAccessEx, userIdInt);
                return context.Response.WriteAsJsonAsync(new
                {
                    error = "Veritabanı Hatası",
                    message = "Veritabanı işlemi sırasında bir hata oluştu.",
                    statusCode = context.Response.StatusCode
                });

            case BusinessRuleException businessRuleEx:
                context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                _ = logService.LogExceptionAsync(businessRuleEx, userIdInt);
                return context.Response.WriteAsJsonAsync(new
                {
                    error = "İş Kuralı Hatası",
                    message = businessRuleEx.Message,
                    statusCode = context.Response.StatusCode
                });

            case BusinessException businessEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                _ = logService.LogExceptionAsync(businessEx, userIdInt);
                return context.Response.WriteAsJsonAsync(new
                {
                    error = "İşlem Hatası",
                    message = businessEx.Message,
                    statusCode = context.Response.StatusCode
                });

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                _ = logService.LogErrorAsync(
                    "Beklenmeyen bir hata oluştu",
                    exception,
                    controllerName,
                    userIdInt);

                return context.Response.WriteAsJsonAsync(new
                {
                    error = "Sistem Hatası",
                    message = "Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.",
                    statusCode = context.Response.StatusCode
                });
        }
    }
}
