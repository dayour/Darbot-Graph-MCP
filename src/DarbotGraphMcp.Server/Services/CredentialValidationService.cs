using Microsoft.Graph;
using Azure.Identity;
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