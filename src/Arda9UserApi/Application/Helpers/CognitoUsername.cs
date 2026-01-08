namespace Arda9UserApi.Application.Helpers;

/// <summary>
/// Helper para construir usernames compostos no padrão tenant#email
/// </summary>
public static class CognitoUsername
{
    /// <summary>
    /// Constrói username composto: {tenantId}#{email}
    /// </summary>
    /// <param name="tenantId">ID do tenant</param>
    /// <param name="email">Email do usuário</param>
    /// <returns>Username no formato: guid-sem-hifens#email-lowercase</returns>
    public static string Build(Guid tenantId, string email)
    {
        var tenantKey = tenantId.ToString("N"); // formato sem hífens
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return $"{tenantKey}#{normalizedEmail}";
    }

    /// <summary>
    /// Extrai o email do username composto
    /// </summary>
    public static string ExtractEmail(string compositeUsername)
    {
        var parts = compositeUsername.Split('#');
        return parts.Length == 2 ? parts[1] : compositeUsername;
    }

    /// <summary>
    /// Extrai o TenantId do username composto
    /// </summary>
    public static Guid ExtractTenantId(string compositeUsername)
    {
        var parts = compositeUsername.Split('#');
        return parts.Length == 2 && Guid.TryParse(parts[0], out var tenantId) 
            ? tenantId 
            : Guid.Empty;
    }
}