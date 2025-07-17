using Microsoft.Graph;

namespace DarbotGraphMcp.Server.Services;

public interface ITenantValidationService
{
    Task<TenantValidationResult> ValidateTenantForOperationAsync(string operationType);
    Task<TenantInfo> GetCurrentTenantInfoAsync();
    bool IsCorporateTenant(string tenantId);
    bool RequiresAdditionalConfirmation(string tenantId, string operationType);
}

public class TenantValidationResult
{
    public bool IsValid { get; set; }
    public bool IsCorporate { get; set; }
    public bool RequiresConfirmation { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string TenantName { get; set; } = string.Empty;
    public string Warning { get; set; } = string.Empty;
    public List<string> SecurityMessages { get; set; } = new();
}

public class TenantInfo
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string TenantType { get; set; } = string.Empty;
    public bool IsCorporate { get; set; }
    public DateTime ValidationTime { get; set; }
}