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
=======
using System;
using System.Threading.Tasks;

namespace DarbotGraphMcp.Server.Services;

/// <summary>
/// Implementation of credential validation service for Azure AD and Microsoft Graph
/// </summary>
public class CredentialValidationService : ICredentialValidationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CredentialValidationService> _logger;

    public CredentialValidationService(IConfiguration configuration, ILogger<CredentialValidationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<CredentialValidationResult> ValidateCredentialsAsync()
    {
        var result = new CredentialValidationResult();
        
        _logger.LogInformation("Starting Azure AD credential validation...");

        // Get configuration values
        var tenantId = _configuration["AzureAd:TenantId"];
        var clientId = _configuration["AzureAd:ClientId"];
        var clientSecret = _configuration["AzureAd:ClientSecret"];

        // Check if configuration is present
        var hasConfig = ValidateConfigurationPresence(tenantId, clientId, clientSecret);
        result.ValidationItems.Add(hasConfig);

        if (!hasConfig.IsValid)
        {
            result.Suggestions.AddRange(new[]
            {
                "Set AzureAd:TenantId in appsettings.json or environment variables",
                "Set AzureAd:ClientId in appsettings.json or environment variables", 
                "Set AzureAd:ClientSecret in appsettings.json or environment variables",
                "Ensure your Azure AD app registration has the necessary Graph API permissions",
                "Check the Azure portal for your tenant ID and app registration details"
            });
            result.IsValid = false;
            return result;
        }

        // Validate GUID formats
        var tenantValidation = ValidateGuidFormat(tenantId, "Tenant ID");
        var clientValidation = ValidateGuidFormat(clientId, "Client ID");
        
        result.ValidationItems.Add(tenantValidation);
        result.ValidationItems.Add(clientValidation);

        // Validate client secret format
        var secretValidation = ValidateClientSecret(clientSecret);
        result.ValidationItems.Add(secretValidation);

        // If format validations fail, provide suggestions
        if (!tenantValidation.IsValid || !clientValidation.IsValid || !secretValidation.IsValid)
        {
            result.Suggestions.AddRange(new[]
            {
                "Tenant ID should be a valid GUID (e.g., 12345678-1234-1234-1234-123456789abc)",
                "Client ID should be a valid GUID from your Azure AD app registration",
                "Client Secret should be a non-empty string from your Azure AD app registration",
                "Check the Azure portal > Azure Active Directory > App registrations for correct values"
            });
            result.IsValid = false;
            return result;
        }

        // Test authentication
        var authTest = await TestAuthenticationAsync();
        result.ValidationItems.Add(authTest);

        if (!authTest.IsValid)
        {
            result.Suggestions.AddRange(new[]
            {
                "Verify the client secret is correct and not expired",
                "Ensure the app registration exists in the specified tenant",
                "Check that the app has the required Graph API permissions granted",
                "Verify admin consent has been provided for application permissions",
                "Common required permissions: User.Read.All, Group.Read.All, Mail.Send"
            });
            result.IsValid = false;
            return result;
        }

        // Test Graph API permissions
        var permissionsTest = await TestGraphPermissionsAsync();
        result.ValidationItems.Add(permissionsTest);

        if (!permissionsTest.IsValid)
        {
            result.Suggestions.AddRange(new[]
            {
                "Grant additional Graph API permissions in Azure portal",
                "Ensure admin consent is provided for application permissions",
                "Common MCP server permissions needed: User.Read.All, Group.Read.All, Mail.Send, Calendars.ReadWrite"
            });
        }

        result.IsValid = result.ValidationItems.All(v => v.IsValid);
        
        _logger.LogInformation("Credential validation completed. Success: {IsValid}", result.IsValid);
        
        return result;
    }

    public ValidationItem ValidateGuidFormat(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ValidationItem 
            { 
                IsValid = false, 
                Message = $"{fieldName} is missing or empty" 
            };
        }

        if (Guid.TryParse(value, out _))
        {
            return new ValidationItem 
            { 
                IsValid = true, 
                Message = $"{fieldName} format valid" 
            };
        }

        return new ValidationItem 
        { 
            IsValid = false, 
            Message = $"{fieldName} format invalid - must be a valid GUID",
            Details = $"Provided value: {value}"
        };
    }

    private ValidationItem ValidateConfigurationPresence(string? tenantId, string? clientId, string? clientSecret)
    {
        var missing = new List<string>();
        
        if (string.IsNullOrWhiteSpace(tenantId)) missing.Add("TenantId");
        if (string.IsNullOrWhiteSpace(clientId)) missing.Add("ClientId");
        if (string.IsNullOrWhiteSpace(clientSecret)) missing.Add("ClientSecret");

        if (missing.Any())
        {
            return new ValidationItem 
            { 
                IsValid = false, 
                Message = $"Missing Azure AD configuration: {string.Join(", ", missing)}" 
            };
        }

        return new ValidationItem 
        { 
            IsValid = true, 
            Message = "Azure AD configuration present" 
        };
    }

    private ValidationItem ValidateClientSecret(string? clientSecret)
    {
        if (string.IsNullOrWhiteSpace(clientSecret))
        {
            return new ValidationItem 
            { 
                IsValid = false, 
                Message = "Client secret is missing or empty" 
            };
        }

        if (clientSecret.Length < 10)
        {
            return new ValidationItem 
            { 
                IsValid = false, 
                Message = "Client secret appears to be too short (should be at least 10 characters)" 
            };
        }

        return new ValidationItem 
        { 
            IsValid = true, 
            Message = "Client secret format valid" 
        };
    }

    public async Task<ValidationItem> TestAuthenticationAsync()
    {
        try
        {
            var tenantId = _configuration["AzureAd:TenantId"];
            var clientId = _configuration["AzureAd:ClientId"];
            var clientSecret = _configuration["AzureAd:ClientSecret"];

            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            var graphClient = new GraphServiceClient(credential);

            // Test authentication by getting the service principal (application) info
            // This is a lightweight call that tests auth without requiring specific permissions
            var servicePrincipal = await graphClient.ServicePrincipals
                .GetAsync(requestConfig =>
                {
                    requestConfig.QueryParameters.Filter = $"appId eq '{clientId}'";
                    requestConfig.QueryParameters.Top = 1;
                });

            if (servicePrincipal?.Value?.Any() == true)
            {
                var sp = servicePrincipal.Value.First();
                return new ValidationItem 
                { 
                    IsValid = true, 
                    Message = "Authentication successful",
                    Details = $"Connected as: {sp.DisplayName ?? sp.AppDisplayName ?? clientId}"
                };
            }

            return new ValidationItem 
            { 
                IsValid = false, 
                Message = "Authentication failed: Service principal not found",
                Details = "The application may not exist in the specified tenant"
            };
        }
        catch (Azure.Identity.AuthenticationFailedException ex)
        {
            return new ValidationItem 
            { 
                IsValid = false, 
                Message = "Authentication failed: Invalid credentials",
                Details = ex.Message,
                Exception = ex
            };
        }
        catch (Microsoft.Graph.Models.ODataErrors.ODataError ex)
        {
            return new ValidationItem 
            { 
                IsValid = false, 
                Message = "Authentication failed: Graph API error",
                Details = ex.Error?.Message ?? ex.Message,
                Exception = ex
            };
        }
        catch (Exception ex)
        {
            return new ValidationItem 
            { 
                IsValid = false, 
                Message = "Authentication test failed",
                Details = ex.Message,
                Exception = ex
            };
        }
    }

    private async Task<ValidationItem> TestGraphPermissionsAsync()
    {
        try
        {
            var tenantId = _configuration["AzureAd:TenantId"];
            var clientId = _configuration["AzureAd:ClientId"];
            var clientSecret = _configuration["AzureAd:ClientSecret"];

            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            var graphClient = new GraphServiceClient(credential);

            // Test a common permission by trying to get users (requires User.Read.All)
            try
            {
                var users = await graphClient.Users
                    .GetAsync(requestConfig =>
                    {
                        requestConfig.QueryParameters.Top = 1;
                    });

                return new ValidationItem 
                { 
                    IsValid = true, 
                    Message = "Graph API permissions verified",
                    Details = "Successfully accessed user directory"
                };
            }
            catch (Microsoft.Graph.Models.ODataErrors.ODataError ex) when (ex.Error?.Code == "Forbidden" || ex.Error?.Code == "InsufficientPrivileges")
            {
                return new ValidationItem 
                { 
                    IsValid = false, 
                    Message = "Insufficient Graph API permissions",
                    Details = "The application needs User.Read.All permission or admin consent is required"
                };
            }
        }
        catch (Exception ex)
        {
            return new ValidationItem 
            { 
                IsValid = false, 
                Message = "Unable to test Graph API permissions",
                Details = ex.Message,
                Exception = ex
            };
        }
    }

}