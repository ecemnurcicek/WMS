namespace Business.Exceptions;

/// <summary>
/// Business katmanında fırlatılan temel exception sınıfı
/// </summary>
public class BusinessException : Exception
{
    public int? LineNumber { get; set; }
    public string? ControllerName { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.Now;

    public BusinessException(string message) : base(message)
    {
    }

    public BusinessException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public BusinessException(string message, string? controllerName = null, int? lineNumber = null) 
        : base(message)
    {
        ControllerName = controllerName;
        LineNumber = lineNumber;
    }
}

/// <summary>
/// Validasyon hatası için özel exception
/// </summary>
public class ValidationException : BusinessException
{
    public ValidationException(string message) : base(message)
    {
    }

    public ValidationException(string message, string? controllerName = null, int? lineNumber = null)
        : base(message, controllerName, lineNumber)
    {
    }
}

/// <summary>
/// Veritabanı işlemi başarısız olduğunda fırlatılan exception
/// </summary>
public class DataAccessException : BusinessException
{
    public DataAccessException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    public DataAccessException(string message, string? controllerName = null, int? lineNumber = null)
        : base(message, controllerName, lineNumber)
    {
    }
}

/// <summary>
/// Kayıt bulunamadığında fırlatılan exception
/// </summary>
public class NotFoundException : BusinessException
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string message, string? controllerName = null, int? lineNumber = null)
        : base(message, controllerName, lineNumber)
    {
    }
}

/// <summary>
/// İş kuralı ihlali durumunda fırlatılan exception
/// </summary>
public class BusinessRuleException : BusinessException
{
    public BusinessRuleException(string message) : base(message)
    {
    }

    public BusinessRuleException(string message, string? controllerName = null, int? lineNumber = null)
        : base(message, controllerName, lineNumber)
    {
    }
}
