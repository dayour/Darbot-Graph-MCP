using Azure.Identity;
using Azure.Core;
using Microsoft.Graph;
using Microsoft.Graph.Beta;
using System.Text.Json;

namespace DarbotGraphMcp.Server.Services;

public interface IAuthenticationService
{
    Microsoft.Graph.GraphServiceClient CreateGraphServiceClient();
    Microsoft.Graph.Beta.GraphServiceClient CreateBetaGraphServiceClient();
    bool IsConfigured { get; }
    string AuthenticationMethod { get; }
}

public class AuthenticationService : IAuthenticationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly bool _isConfigured;
    private readonly string _authenticationMethod;

    public AuthenticationService(IConfiguration configuration, ILogger<AuthenticationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        (_isConfigured, _authenticationMethod) = DetermineAuthenticationMethod();
    }

    public bool IsConfigured => _isConfigured;
    public string AuthenticationMethod => _authenticationMethod;

    public Microsoft.Graph.GraphServiceClient CreateGraphServiceClient()
    {
        var credential = CreateCredential();
        return new Microsoft.Graph.GraphServiceClient(credential);
    }

    public Microsoft.Graph.Beta.GraphServiceClient CreateBetaGraphServiceClient()
    {
        var credential = CreateCredential();
        return new Microsoft.Graph.Beta.GraphServiceClient(credential);
    }

    private TokenCredential CreateCredential()
    {
        return _authenticationMethod switch
        {
            "ClientSecret" => CreateClientSecretCredential(),
            "AzureCLI" => CreateAzureCliCredential(),
            "ManagedIdentity" => CreateManagedIdentityCredential(),
            "VSCodeCredential" => CreateVSCodeCredential(),
            "DefaultAzure" => CreateDefaultAzureCredential(),
            _ => CreateDemoCredential()
        };
    }

    private (bool isConfigured, string method) DetermineAuthenticationMethod()
    {
        _logger.LogInformation("Determining authentication method...");
        
        // Check for client secret authentication (highest priority)
        if (HasClientSecretConfig())
        {
            _logger.LogInformation("Using ClientSecret authentication");
            return (true, "ClientSecret");
        }

        // Check for Azure CLI authentication
        if (HasAzureCliConfig())
        {
            _logger.LogInformation("Using Azure CLI authentication");
            return (true, "AzureCLI");
        }

        // Check for Managed Identity
        if (HasManagedIdentityConfig())
        {
            _logger.LogInformation("Using Managed Identity authentication");
            return (true, "ManagedIdentity");
        }

        // Check for VS Code authentication
        if (HasVSCodeConfig())
        {
            _logger.LogInformation("Using VS Code authentication");
            return (true, "VSCodeCredential");
        }

        // Check for default Azure credential chain
        if (HasDefaultAzureConfig())
        {
            _logger.LogInformation("Using Default Azure credential chain");
            return (true, "DefaultAzure");
        }

        _logger.LogWarning("No valid authentication configuration found, using demo mode");
        return (false, "Demo");
    }

    private bool HasClientSecretConfig()
    {
        var tenantId = GetConfigValue("TenantId");
        var clientId = GetConfigValue("ClientId");
        var clientSecret = GetConfigValue("ClientSecret");

        return !string.IsNullOrEmpty(tenantId) && 
               !string.IsNullOrEmpty(clientId) && 
               !string.IsNullOrEmpty(clientSecret) &&
               tenantId != "00000000-0000-0000-0000-000000000000" &&
               clientSecret != "placeholder-secret";
    }

    private bool HasAzureCliConfig()
    {
        var useAzureCli = GetBoolConfigValue("UseAzureCli");
        var tenantId = GetConfigValue("TenantId");
        
        _logger.LogDebug("Azure CLI config check: UseAzureCli={UseAzureCli}, TenantId={TenantId}", useAzureCli, !string.IsNullOrEmpty(tenantId) ? "present" : "missing");
        
        return useAzureCli && !string.IsNullOrEmpty(tenantId);
    }

    private bool HasManagedIdentityConfig()
    {
        var useManagedIdentity = GetBoolConfigValue("UseManagedIdentity");
        return useManagedIdentity;
    }

    private bool HasVSCodeConfig()
    {
        var useVSCode = GetBoolConfigValue("UseVSCode");
        var tenantId = GetConfigValue("TenantId");
        
        return useVSCode && !string.IsNullOrEmpty(tenantId);
    }

    private bool HasDefaultAzureConfig()
    {
        var useDefaultChain = GetBoolConfigValue("UseDefaultChain");
        return useDefaultChain;
    }

    private string GetConfigValue(string key)
    {
        // Try command line arguments first (from npm wrapper)
        var envKey = $"AzureAd__{key}";
        var envValue = Environment.GetEnvironmentVariable(envKey);
        if (!string.IsNullOrEmpty(envValue))
        {
            _logger.LogDebug("Using environment variable for {Key}: {Value}", key, envValue);
            return envValue;
        }

        // Try standard environment variables
        var standardEnvKey = $"AZURE_AD_{key.ToUpperInvariant()}";
        var standardEnvValue = Environment.GetEnvironmentVariable(standardEnvKey);
        if (!string.IsNullOrEmpty(standardEnvValue))
        {
            _logger.LogDebug("Using standard environment variable for {Key}: {Value}", key, standardEnvValue);
            return standardEnvValue;
        }

        // Fall back to configuration
        var configValue = _configuration[$"AzureAd:{key}"];
        if (!string.IsNullOrEmpty(configValue))
        {
            _logger.LogDebug("Using configuration setting for {Key}: {Value}", key, configValue);
            return configValue;
        }

        _logger.LogDebug("No value found for {Key}", key);
        return string.Empty;
    }

    private bool GetBoolConfigValue(string key)
    {
        // Try command line arguments first (from npm wrapper)
        var envKey = $"AzureAd__{key}";
        var envValue = Environment.GetEnvironmentVariable(envKey);
        if (!string.IsNullOrEmpty(envValue))
        {
            _logger.LogDebug("Using environment variable for {Key}: {Value}", key, envValue);
            return IsTrueValue(envValue);
        }

        // Try standard environment variables
        var standardEnvKey = $"AZURE_AD_{key.ToUpperInvariant()}";
        var standardEnvValue = Environment.GetEnvironmentVariable(standardEnvKey);
        if (!string.IsNullOrEmpty(standardEnvValue))
        {
            _logger.LogDebug("Using standard environment variable for {Key}: {Value}", key, standardEnvValue);
            return IsTrueValue(standardEnvValue);
        }

        // Fall back to configuration
        var configValue = _configuration.GetValue<bool>($"AzureAd:{key}");
        _logger.LogDebug("Using configuration setting for {Key}: {Value}", key, configValue);
        return configValue;
    }

    private bool IsTrueValue(string value)
    {
        if (string.IsNullOrEmpty(value)) return false;
        
        var normalizedValue = value.Trim().ToLowerInvariant();
        return normalizedValue == "true" || normalizedValue == "yes" || normalizedValue == "1" || normalizedValue == "on";
    }

    private ClientSecretCredential CreateClientSecretCredential()
    {
        var tenantId = GetConfigValue("TenantId");
        var clientId = GetConfigValue("ClientId");
        var clientSecret = GetConfigValue("ClientSecret");

        var options = new ClientSecretCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
        };

        return new ClientSecretCredential(tenantId, clientId, clientSecret, options);
    }

    private AzureCliCredential CreateAzureCliCredential()
    {
        var tenantId = GetConfigValue("TenantId");
        var options = new AzureCliCredentialOptions();
        
        if (!string.IsNullOrEmpty(tenantId))
        {
            options.TenantId = tenantId;
        }

        return new AzureCliCredential(options);
    }

    private ManagedIdentityCredential CreateManagedIdentityCredential()
    {
        var clientId = GetConfigValue("ClientId");
        
        if (!string.IsNullOrEmpty(clientId))
        {
            return new ManagedIdentityCredential(clientId);
        }

        return new ManagedIdentityCredential();
    }

    private VisualStudioCodeCredential CreateVSCodeCredential()
    {
        var tenantId = GetConfigValue("TenantId");
        var options = new VisualStudioCodeCredentialOptions();
        
        if (!string.IsNullOrEmpty(tenantId))
        {
            options.TenantId = tenantId;
        }

        return new VisualStudioCodeCredential(options);
    }

    private DefaultAzureCredential CreateDefaultAzureCredential()
    {
        var options = new DefaultAzureCredentialOptions
        {
            ExcludeInteractiveBrowserCredential = true,
            ExcludeSharedTokenCacheCredential = false,
            ExcludeVisualStudioCredential = false,
            ExcludeVisualStudioCodeCredential = false,
            ExcludeAzureCliCredential = false,
            ExcludeManagedIdentityCredential = false
        };

        var tenantId = GetConfigValue("TenantId");
        if (!string.IsNullOrEmpty(tenantId))
        {
            options.VisualStudioCodeTenantId = tenantId;
            options.VisualStudioTenantId = tenantId;
        }

        return new DefaultAzureCredential(options);
    }

    private ClientSecretCredential CreateDemoCredential()
    {
        // Return placeholder credential for demo mode
        return new ClientSecretCredential(
            "00000000-0000-0000-0000-000000000000",
            "00000000-0000-0000-0000-000000000000",
            "placeholder-secret"
        );
    }
}