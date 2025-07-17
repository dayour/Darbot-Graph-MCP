using System;
using System.Threading.Tasks;

namespace DarbotGraphMcp.Server.Services;

/// <summary>
/// Service for validating Azure AD credentials and configuration
/// </summary>
public interface ICredentialValidationService
{
    /// <summary>
    /// Validates all Azure AD configuration and credentials
    /// </summary>
    /// <returns>Validation result with success status and detailed messages</returns>
    Task<CredentialValidationResult> ValidateCredentialsAsync();
    
    /// <summary>
    /// Validates the format of a GUID string
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <param name="fieldName">The name of the field being validated</param>
    /// <returns>Validation result for the GUID format</returns>
    ValidationItem ValidateGuidFormat(string? value, string fieldName);
    
    /// <summary>
    /// Tests authentication with the configured credentials
    /// </summary>
    /// <returns>Authentication test result</returns>
    Task<ValidationItem> TestAuthenticationAsync();
}

/// <summary>
/// Result of credential validation containing all validation checks
/// </summary>
public class CredentialValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationItem> ValidationItems { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
    
    /// <summary>
    /// Gets all successful validation items
    /// </summary>
    public IEnumerable<ValidationItem> SuccessItems => ValidationItems.Where(v => v.IsValid);
    
    /// <summary>
    /// Gets all failed validation items
    /// </summary>
    public IEnumerable<ValidationItem> FailedItems => ValidationItems.Where(v => !v.IsValid);
    
    /// <summary>
    /// Gets a formatted summary of the validation results
    /// </summary>
    public string GetFormattedSummary()
    {
        var summary = new List<string>();
        
        foreach (var item in SuccessItems)
        {
            summary.Add($"✓ {item.Message}");
        }
        
        foreach (var item in FailedItems)
        {
            summary.Add($"✗ {item.Message}");
        }
        
        if (Suggestions.Any())
        {
            summary.Add("");
            summary.Add("Suggestions:");
            foreach (var suggestion in Suggestions)
            {
                summary.Add($"  • {suggestion}");
            }
        }
        
        return string.Join(Environment.NewLine, summary);
    }
}

/// <summary>
/// Individual validation item result
/// </summary>
public class ValidationItem
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public Exception? Exception { get; set; }
}