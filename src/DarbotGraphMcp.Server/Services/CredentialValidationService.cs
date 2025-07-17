using Microsoft.Graph;
using Azure.Identity;
using System.Text.RegularExpressions;

namespace DarbotGraphMcp.Server.Services;

public interface ICredentialValidationService
{
    Task<CredentialValidationResult> ValidateCredentialsAsync(string? tenantId, string? clientId, string? clientSecret);
    bool IsValidGuid(string? value);
    bool AreCredentialsConfigured(string? tenantId, string? clientId, string? clientSecret);
    bool IsVSCodeInputPromptConfiguration(string? tenantId, string? clientId, string? clientSecret);
}

public class CredentialValidationService : ICredentialValidationService
{
    private readonly ILogger<CredentialValidationService> _logger;
    private static readonly Regex GuidRegex = new(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", RegexOptions.Compiled);

    public CredentialValidationService(ILogger<CredentialValidationService> logger)
    {
        _logger = logger;
    }

    public async Task<CredentialValidationResult> ValidateCredentialsAsync(string? tenantId, string? clientId, string? clientSecret)
    {
        var result = new CredentialValidationResult();
        
        // Check for VS Code input prompt configuration
        if (IsVSCodeInputPromptConfiguration(tenantId, clientId, clientSecret))
        {
            result.IsValid = false;
            result.Mode = ValidationMode.VSCodeInputPrompt;
            result.Message = "VS Code input prompt configuration detected";
            result.Details.Add("Configuration contains VS Code input prompt variables (${input:...})");
            result.Suggestions.Add("This configuration is for VS Code MCP installation with user prompts");
            result.Suggestions.Add("Use direct credential values in appsettings.json for server-side deployment");
            result.Suggestions.Add("For VS Code, use the one-click installation buttons in the README");
            
            _logger.LogWarning("VS Code input prompt configuration detected in credentials");
            return result;
        }
        
        // Check if credentials are configured
        if (!AreCredentialsConfigured(tenantId, clientId, clientSecret))
        {
            result.IsValid = false;
            result.Mode = ValidationMode.Demo;
            result.Message = "Azure AD credentials not configured - running in demo mode";
            result.Suggestions.Add("Configure Azure AD credentials in appsettings.json or environment variables to access real Microsoft 365 data");
            result.Suggestions.Add("See documentation for Azure AD app registration steps");
            
            _logger.LogInformation("Credential validation: {Message}", result.Message);
            return result;
        }

        // Validate GUID formats
        if (!IsValidGuid(tenantId))
        {
            result.IsValid = false;
            result.Mode = ValidationMode.Invalid;
            result.Message = "Invalid Azure AD Tenant ID format";
            result.Details.Add($"Tenant ID '{tenantId}' is not a valid GUID format");
            result.Suggestions.Add("Tenant ID must be in GUID format (e.g., 12345678-1234-1234-1234-123456789012)");
            result.Suggestions.Add("Find your Tenant ID in Azure Portal > Azure Active Directory > Overview");
            
            _logger.LogError("Credential validation failed: {Message} - TenantId: {TenantId}", result.Message, tenantId);
            return result;
        }

        if (!IsValidGuid(clientId))
        {
            result.IsValid = false;
            result.Mode = ValidationMode.Invalid;
            result.Message = "Invalid Azure AD Client ID format";
            result.Details.Add($"Client ID '{clientId}' is not a valid GUID format");
            result.Suggestions.Add("Client ID must be in GUID format (e.g., 12345678-1234-1234-1234-123456789012)");
            result.Suggestions.Add("Find your Client ID in Azure Portal > Azure Active Directory > App registrations > [Your App] > Overview");
            
            _logger.LogError("Credential validation failed: {Message} - ClientId: {ClientId}", result.Message, clientId);
            return result;
        }

        if (string.IsNullOrWhiteSpace(clientSecret))
        {
            result.IsValid = false;
            result.Mode = ValidationMode.Invalid;
            result.Message = "Azure AD Client Secret is empty";
            result.Details.Add("Client Secret is required for authentication");
            result.Suggestions.Add("Generate a new client secret in Azure Portal > Azure Active Directory > App registrations > [Your App] > Certificates & secrets");
            result.Suggestions.Add("Ensure the client secret has not expired");
            
            _logger.LogError("Credential validation failed: {Message}", result.Message);
            return result;
        }

        // Test authentication with Microsoft Graph
        try
        {
            _logger.LogInformation("Testing Azure AD authentication with Tenant ID: {TenantId}, Client ID: {ClientId}", tenantId, clientId);
            
            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            var graphClient = new GraphServiceClient(credential);
            
            // Test with a simple Graph API call that requires minimal permissions
            var testResult = await graphClient.Applications.GetAsync(config =>
            {
                config.QueryParameters.Top = 1;
            });
            
            result.IsValid = true;
            result.Mode = ValidationMode.Production;
            result.Message = "Azure AD credentials validated successfully";
            result.Details.Add($"Successfully authenticated with tenant: {tenantId}");
            result.Details.Add("Microsoft Graph API access confirmed");
            
            _logger.LogInformation("Credential validation successful: {Message}", result.Message);
            return result;
        }
        catch (Azure.Identity.AuthenticationFailedException ex)
        {
            result.IsValid = false;
            result.Mode = ValidationMode.AuthenticationFailed;
            result.Message = "Azure AD authentication failed";
            result.Details.Add($"Authentication error: {ex.Message}");
            
            // Provide specific suggestions based on common authentication failures
            if (ex.Message.Contains("AADSTS7000215"))
            {
                result.Suggestions.Add("Invalid client secret provided. Generate a new client secret in Azure Portal");
            }
            else if (ex.Message.Contains("AADSTS700016"))
            {
                result.Suggestions.Add("Application not found in tenant. Verify the Client ID is correct");
            }
            else if (ex.Message.Contains("AADSTS90002"))
            {
                result.Suggestions.Add("Tenant not found. Verify the Tenant ID is correct");
            }
            else
            {
                result.Suggestions.Add("Verify your Azure AD app registration settings");
                result.Suggestions.Add("Ensure the client secret has not expired");
                result.Suggestions.Add("Check that the app has required API permissions and admin consent");
            }
            
            _logger.LogError(ex, "Azure AD authentication failed: {Message}", ex.Message);
            return result;
        }
        catch (ServiceException ex) when ((int)ex.ResponseStatusCode == 403)
        {
            result.IsValid = false;
            result.Mode = ValidationMode.InsufficientPermissions;
            result.Message = "Azure AD authentication succeeded but insufficient permissions";
            result.Details.Add($"Graph API error: {ex.Message}");
            result.Suggestions.Add("Grant admin consent for required Microsoft Graph API permissions");
            result.Suggestions.Add("Required permissions: Application.Read.All (for basic validation)");
            result.Suggestions.Add("See documentation for complete list of required permissions");
            
            _logger.LogWarning(ex, "Authentication succeeded but insufficient Graph API permissions: {Message}", ex.Message);
            return result;
        }
        catch (Exception ex)
        {
            result.IsValid = false;
            result.Mode = ValidationMode.UnknownError;
            result.Message = "Unexpected error during credential validation";
            result.Details.Add($"Error: {ex.Message}");
            result.Suggestions.Add("Check network connectivity to Microsoft Graph API");
            result.Suggestions.Add("Verify firewall settings allow HTTPS traffic to graph.microsoft.com");
            result.Suggestions.Add("Review application logs for detailed error information");
            
            _logger.LogError(ex, "Unexpected error during credential validation: {Message}", ex.Message);
            return result;
        }
    }

    public bool IsValidGuid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;
            
        return GuidRegex.IsMatch(value);
    }

    public bool AreCredentialsConfigured(string? tenantId, string? clientId, string? clientSecret)
    {
        return !string.IsNullOrWhiteSpace(tenantId) && 
               !string.IsNullOrWhiteSpace(clientId) && 
               !string.IsNullOrWhiteSpace(clientSecret) &&
               tenantId != "00000000-0000-0000-0000-000000000000" &&
               clientId != "00000000-0000-0000-0000-000000000000" &&
               clientSecret != "placeholder-secret";
    }

    public bool IsVSCodeInputPromptConfiguration(string? tenantId, string? clientId, string? clientSecret)
    {
        return (!string.IsNullOrWhiteSpace(tenantId) && tenantId.Contains("${input:")) ||
               (!string.IsNullOrWhiteSpace(clientId) && clientId.Contains("${input:")) ||
               (!string.IsNullOrWhiteSpace(clientSecret) && clientSecret.Contains("${input:"));
    }
}

public class CredentialValidationResult
{
    public bool IsValid { get; set; }
    public ValidationMode Mode { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Details { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
}

public enum ValidationMode
{
    Demo,
    Production,
    Invalid,
    AuthenticationFailed,
    InsufficientPermissions,
    UnknownError,
    VSCodeInputPrompt
}