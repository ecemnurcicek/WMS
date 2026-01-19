using Business.Services;
using Data.Context;

namespace Business.Managers;

/// <summary>
/// Tüm business servislerini yönetir ve koordine eder
/// </summary>
public class BusinessManager : IBusinessManager
{
    private readonly ApplicationContext _context;
    private readonly ILoginService _loginService;
    private readonly ILogService _logService;

    public ILoginService LoginService => _loginService;
    public ILogService LogService => _logService;

    public BusinessManager(ApplicationContext context)
    {
        _context = context;
        _logService = new LogService(context);
        _loginService = new LoginService(context);
    }
}
