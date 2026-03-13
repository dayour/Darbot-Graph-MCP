using System.Threading.Tasks;

namespace DarbotGraphMcp.Server.Services;

/// <summary>
/// Service for validating Azure AD credentials and configuration
/// </summary>
public interface ICredentialValidationService
{
    /// <summary>
    /// Validates Azure AD credentials using the provided tenant, client, and secret values.
    /// </summary>
    Task<CredentialValidationResult> ValidateCredentialsAsync(string? tenantId, string? clientId, string? clientSecret);

    /// <summary>
    /// Returns true if the value is a valid GUID.
    /// </summary>
    bool IsValidGuid(string? value);

    /// <summary>
    /// Returns true if all three credential values are non-empty and non-placeholder.
    /// </summary>
    bool AreCredentialsConfigured(string? tenantId, string? clientId, string? clientSecret);

    /// <summary>
    /// Returns true if any credential value contains a VS Code input prompt variable.
    /// </summary>
    bool IsVSCodeInputPromptConfiguration(string? tenantId, string? clientId, string? clientSecret);
}

/// <summary>
/// Result of credential validation
/// </summary>
public class CredentialValidationResult
{
    public bool IsValid { get; set; }
    public ValidationMode Mode { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Details { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
}

/// <summary>
/// Describes the outcome mode of a credential validation
/// </summary>
public enum ValidationMode
{
    /// <summary>No credentials configured; server runs in demo/read-only mode.</summary>
    Demo,
    /// <summary>Credentials validated successfully; server is ready for production use.</summary>
    Production,
    /// <summary>Credential values are present but invalid (e.g., malformed GUID).</summary>
    Invalid,
    /// <summary>Credentials are well-formed but authentication with Azure AD failed.</summary>
    AuthenticationFailed,
    /// <summary>Authentication succeeded but the app lacks required Graph API permissions.</summary>
    InsufficientPermissions,
    /// <summary>An unexpected error occurred during validation.</summary>
    UnknownError,
    /// <summary>Credentials contain VS Code input prompt variables (${input:...}) instead of real values.</summary>
    VSCodeInputPrompt
}
