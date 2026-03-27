using Microsoft.AspNetCore.Identity;

namespace Business.Utilities;

/// <summary>
/// Parola hash'lama ve doğrulama işlemleri için yardımcı sınıf
/// </summary>
public class PasswordHasherUtil
{
    private static readonly PasswordHasher<object> _hasher = new PasswordHasher<object>();

    /// <summary>
    /// Verilen parolayı hash'ler
    /// Örnek: "Ab12345@" -> "AQAAAAIAAYagAAAAEEIjquu66mpnTlOKt60v4HecxCVFfE8Oj8/R8uhjaW8n8tS29xaAnKv/Dr2XV/R3iA=="
    /// </summary>
    public static string HashPassword(string password)
    {
        return _hasher.HashPassword(null, password);
    }

    /// <summary>
    /// Verilen parolayı hash'lenmiş version ile doğrular
    /// </summary>
    public static bool VerifyPassword(string hashedPassword, string password)
    {
        var result = _hasher.VerifyHashedPassword(null, hashedPassword, password);
        return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
