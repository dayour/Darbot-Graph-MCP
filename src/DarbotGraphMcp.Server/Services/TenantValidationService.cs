using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace DarbotGraphMcp.Server.Services;

public class TenantValidationService : ITenantValidationService
{
    private readonly GraphServiceClient _graphClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TenantValidationService> _logger;
    
    // Known corporate tenant IDs from security incident
    private readonly HashSet<string> _corporateTenantIds = new()
    {
        "72f988bf-86f1-41af-91ab-2d7cd011db47", // Microsoft corporate tenant
    };
    
    // High-risk operations that require additional validation
    private readonly HashSet<string> _highRiskOperations = new()
    {
        "app-create", "user-create", "group-create", "role-assign", "permission-grant"
    };

    public TenantValidationService(
        GraphServiceClient graphClient, 
        IConfiguration configuration,
        ILogger<TenantValidationService> logger)
    {
        _graphClient = graphClient;
        _configuration = configuration;
        _logger = logger;
        
        // Load additional corporate tenant IDs from configuration
        var configCorporateTenants = _configuration.GetSection("Security:CorporateTenantIds").Get<string[]>();
        if (configCorporateTenants != null)
        {
            foreach (var tenantId in configCorporateTenants)
            {
                _corporateTenantIds.Add(tenantId);
            }
        }
    }

    public async Task<TenantValidationResult> ValidateTenantForOperationAsync(string operationType)
    {
        var result = new TenantValidationResult();
        
        try
        {
            var tenantInfo = await GetCurrentTenantInfoAsync();
            
            result.TenantId = tenantInfo.Id;
            result.TenantName = tenantInfo.DisplayName;
            result.IsCorporate = IsCorporateTenant(tenantInfo.Id);
            result.RequiresConfirmation = RequiresAdditionalConfirmation(tenantInfo.Id, operationType);
            result.IsValid = true;

            // Add security messages based on tenant and operation type
            if (result.IsCorporate)
            {
                result.SecurityMessages.Add("⚠️  CORPORATE TENANT DETECTED");
                result.SecurityMessages.Add($"Operating in corporate tenant: {tenantInfo.DisplayName}");
                result.Warning = "This operation will affect a CORPORATE production environment. Exercise extreme caution.";
            }

            if (result.RequiresConfirmation)
            {
                result.SecurityMessages.Add("🔒 CONFIRMATION REQUIRED");
                result.SecurityMessages.Add($"High-risk operation '{operationType}' requires explicit confirmation");
            }

            if (_highRiskOperations.Contains(operationType))
            {
                result.SecurityMessages.Add($"🚨 High-risk operation: {operationType}");
            }

            _logger.LogInformation("Tenant validation completed for operation {OperationType} in tenant {TenantId} ({TenantName})", 
                operationType, tenantInfo.Id, tenantInfo.DisplayName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to validate tenant for operation {OperationType}, operating in demo mode", operationType);
            
            // In demo mode, create safe mock response
            result.IsValid = true;
            result.TenantId = "demo-tenant-id";
            result.TenantName = "Demo Mode (No Azure AD Connection)";
            result.IsCorporate = false;
            result.RequiresConfirmation = false;
            result.SecurityMessages.Add("🧪 Demo Mode - No real tenant validation");
        }

        return result;
    }

    public async Task<TenantInfo> GetCurrentTenantInfoAsync()
    {
        try
        {
            var organization = await _graphClient.Organization.GetAsync();
            var org = organization?.Value?.FirstOrDefault();
            
            if (org != null)
            {
                return new TenantInfo
                {
                    Id = org.Id ?? "unknown",
                    DisplayName = org.DisplayName ?? "Unknown Organization",
                    TenantType = org.TenantType ?? "Unknown",
                    IsCorporate = IsCorporateTenant(org.Id ?? ""),
                    ValidationTime = DateTime.UtcNow
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not retrieve tenant information, operating in demo mode");
        }

        // Return demo tenant info when Graph API is not available
        return new TenantInfo
        {
            Id = "demo-tenant-id",
            DisplayName = "Demo Mode",
            TenantType = "Demo",
            IsCorporate = false,
            ValidationTime = DateTime.UtcNow
        };
    }

    public bool IsCorporateTenant(string tenantId)
    {
        return !string.IsNullOrEmpty(tenantId) && _corporateTenantIds.Contains(tenantId);
    }

    public bool RequiresAdditionalConfirmation(string tenantId, string operationType)
    {
        // Always require confirmation for corporate tenants on high-risk operations
        if (IsCorporateTenant(tenantId) && _highRiskOperations.Contains(operationType))
        {
            return true;
        }

        // Check configuration for additional confirmation requirements
        var requireConfirmationForAll = _configuration.GetValue<bool>("Security:RequireConfirmationForAllMutations", false);
        return requireConfirmationForAll;
    }
}