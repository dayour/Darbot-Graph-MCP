using DarbotGraphMcp.Server.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Moq;
using Azure.Identity;

namespace DarbotGraphMcp.Tests;

public class TenantValidationServiceTests
{
    private readonly TenantValidationService _tenantValidationService;

    public TenantValidationServiceTests()
    {
        // Create a real configuration for testing
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            {"Security:RequireConfirmationForAllMutations", "false"}
        });
        var configuration = configBuilder.Build();
        
        // Create a real Graph client that will fail gracefully in demo mode
        var credential = new ClientSecretCredential("demo-tenant", "demo-client", "demo-secret");
        var graphClient = new GraphServiceClient(credential);
        
        var mockLogger = new Mock<ILogger<TenantValidationService>>();
        
        _tenantValidationService = new TenantValidationService(
            graphClient,
            configuration,
            mockLogger.Object);
    }

    [Fact]
    public void IsCorporateTenant_WithMicrosoftTenantId_ReturnsTrue()
    {
        // Arrange
        var microsoftTenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";

        // Act
        var result = _tenantValidationService.IsCorporateTenant(microsoftTenantId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsCorporateTenant_WithNonCorporateTenantId_ReturnsFalse()
    {
        // Arrange
        var regularTenantId = "6b104499-c49f-45dc-b3a2-df95efd6eeb4";

        // Act
        var result = _tenantValidationService.IsCorporateTenant(regularTenantId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void RequiresAdditionalConfirmation_WithCorporateTenantAndHighRiskOperation_ReturnsTrue()
    {
        // Arrange
        var microsoftTenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";
        var highRiskOperation = "app-create";

        // Act
        var result = _tenantValidationService.RequiresAdditionalConfirmation(microsoftTenantId, highRiskOperation);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void RequiresAdditionalConfirmation_WithRegularTenantAndHighRiskOperation_ReturnsFalse()
    {
        // Arrange
        var regularTenantId = "6b104499-c49f-45dc-b3a2-df95efd6eeb4";
        var highRiskOperation = "app-create";

        // Act
        var result = _tenantValidationService.RequiresAdditionalConfirmation(regularTenantId, highRiskOperation);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateTenantForOperationAsync_WithAppCreateOperation_ReturnsValidationResult()
    {
        // Arrange
        var operationType = "app-create";

        // Act - This will fail gracefully and return demo mode results
        var result = await _tenantValidationService.ValidateTenantForOperationAsync(operationType);

        // Assert
        Assert.True(result.IsValid);
        Assert.Contains("🚨 High-risk operation: app-create", result.SecurityMessages);
    }

    [Fact]
    public async Task GetCurrentTenantInfoAsync_WithDemoClient_ReturnsDemoTenantInfo()
    {
        // Act - This will fail gracefully and return demo mode
        var result = await _tenantValidationService.GetCurrentTenantInfoAsync();

        // Assert
        Assert.Equal("demo-tenant-id", result.Id);
        Assert.Equal("Demo Mode", result.DisplayName);
        Assert.Equal("Demo", result.TenantType);
        Assert.False(result.IsCorporate);
    }

    [Theory]
    [InlineData("app-create")]
    [InlineData("user-create")]
    [InlineData("group-create")]
    [InlineData("role-assign")]
    [InlineData("permission-grant")]
    public void RequiresAdditionalConfirmation_WithCorporateTenantAndVariousHighRiskOperations_ReturnsTrue(string operation)
    {
        // Arrange
        var microsoftTenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";

        // Act
        var result = _tenantValidationService.RequiresAdditionalConfirmation(microsoftTenantId, operation);

        // Assert
        Assert.True(result);
    }
}