using Microsoft.Graph;
using Microsoft.Graph.Beta;
using Azure.Identity;
using System.Text.Json;
using System.Security;
using System.Text.RegularExpressions;
using DarbotGraphMcp.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddLogging();
builder.Services.AddControllers();


// Add Authentication service
builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();

// Configure Microsoft Graph client
builder.Services.AddScoped<Microsoft.Graph.GraphServiceClient>(provider =>
{
    var authService = provider.GetRequiredService<IAuthenticationService>();
    return authService.CreateGraphServiceClient();

// Add credential validation service

builder.Services.AddSingleton<ICredentialValidationService, CredentialValidationService>();

// Configure Microsoft Graph client with credential validation
builder.Services.AddScoped<Microsoft.Graph.GraphServiceClient>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var logger = provider.GetRequiredService<ILogger<Program>>();
    
    // Use ClientSecretCredential for app-only authentication
    var clientId = configuration["AzureAd:ClientId"];
    var clientSecret = configuration["AzureAd:ClientSecret"];
    var tenantId = configuration["AzureAd:TenantId"];
    

    // Security validation for tenant ID to prevent cross-tenant issues
    if (!string.IsNullOrEmpty(tenantId))
    {
        if (!IsValidTenantId(tenantId))
        {
            logger.LogError("Invalid tenant ID format detected. Must be a valid GUID.");
            throw new ArgumentException("Invalid tenant ID format. Must be a valid GUID.");
        }
        
        if (IsPlaceholderTenantId(tenantId))
        {
            logger.LogError("Security warning: Placeholder tenant ID detected. This could lead to cross-tenant security issues.");
            throw new SecurityException("Placeholder tenant ID not allowed. Use your organization's actual tenant ID.");
        }
    }
    
    // For demo/testing purposes, use placeholder values if not configured
    if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(tenantId))
    {
        logger.LogInformation("Azure AD credentials not configured. Running in demo mode.");
        clientId = "00000000-0000-0000-0000-000000000000";
        clientSecret = "placeholder-secret";
        tenantId = "00000000-0000-0000-0000-000000000000";
    }
    else
    {
        logger.LogInformation("Azure AD credentials configured for tenant: {TenantId}", tenantId);
    }

    
    // Always use placeholder values for the Graph client construction
    // Real credential validation happens at startup and in the enhanced service
    var placeholderTenantId = "00000000-0000-0000-0000-000000000000";
    var placeholderClientId = "00000000-0000-0000-0000-000000000000";
    var placeholderSecret = "placeholder-secret";

    
    var credential = new ClientSecretCredential(placeholderTenantId, placeholderClientId, placeholderSecret);
    
    return new Microsoft.Graph.GraphServiceClient(credential);

});

// Configure Microsoft Graph Beta client with credential validation
builder.Services.AddScoped<Microsoft.Graph.Beta.GraphServiceClient>(provider =>
{

    var authService = provider.GetRequiredService<IAuthenticationService>();
    return authService.CreateBetaGraphServiceClient();

    var configuration = provider.GetRequiredService<IConfiguration>();
    var logger = provider.GetRequiredService<ILogger<Program>>();
    
    var clientId = configuration["AzureAd:ClientId"];
    var clientSecret = configuration["AzureAd:ClientSecret"];
    var tenantId = configuration["AzureAd:TenantId"];
    

    // Security validation for tenant ID (same as regular Graph client)
    if (!string.IsNullOrEmpty(tenantId))
    {
        if (!IsValidTenantId(tenantId))
        {
            logger.LogError("Invalid tenant ID format detected for Beta client. Must be a valid GUID.");
            throw new ArgumentException("Invalid tenant ID format. Must be a valid GUID.");
        }
        
        if (IsPlaceholderTenantId(tenantId))
        {
            logger.LogError("Security warning: Placeholder tenant ID detected for Beta client.");
            throw new SecurityException("Placeholder tenant ID not allowed. Use your organization's actual tenant ID.");
        }
    }
    
    // For demo/testing purposes, use placeholder values if not configured
    if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(tenantId))
    {
        clientId = "00000000-0000-0000-0000-000000000000";
        clientSecret = "placeholder-secret";
        tenantId = "00000000-0000-0000-0000-000000000000";
    }

    
    var credential = new ClientSecretCredential(placeholderTenantId, placeholderClientId, placeholderSecret);
    
    return new Microsoft.Graph.Beta.GraphServiceClient(credential);

});

// Add Enhanced Graph service
builder.Services.AddScoped<IGraphServiceEnhanced, GraphServiceEnhanced>();

// Add Tenant Validation service for security
builder.Services.AddScoped<ITenantValidationService, TenantValidationService>();

var app = builder.Build();

// Perform credential validation on startup
using (var scope = app.Services.CreateScope())
{
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var credentialValidator = scope.ServiceProvider.GetRequiredService<ICredentialValidationService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    var tenantId = configuration["AzureAd:TenantId"];
    var clientId = configuration["AzureAd:ClientId"];
    var clientSecret = configuration["AzureAd:ClientSecret"];
    
    logger.LogInformation("=== Darbot Graph MCP Server - Credential Validation ===");
    
    try
    {
        var validationResult = await credentialValidator.ValidateCredentialsAsync(tenantId, clientId, clientSecret);
        
        // Log validation results
        switch (validationResult.Mode)
        {
            case ValidationMode.Demo:
                logger.LogInformation("✓ {Message}", validationResult.Message);
                foreach (var suggestion in validationResult.Suggestions)
                {
                    logger.LogInformation("  💡 {Suggestion}", suggestion);
                }
                break;
                
            case ValidationMode.Production:
                logger.LogInformation("✅ {Message}", validationResult.Message);
                foreach (var detail in validationResult.Details)
                {
                    logger.LogInformation("  ✓ {Detail}", detail);
                }
                break;
                
            case ValidationMode.VSCodeInputPrompt:
                logger.LogWarning("⚠️ {Message}", validationResult.Message);
                foreach (var detail in validationResult.Details)
                {
                    logger.LogWarning("  ⚠️ {Detail}", detail);
                }
                foreach (var suggestion in validationResult.Suggestions)
                {
                    logger.LogInformation("  💡 {Suggestion}", suggestion);
                }
                break;
                
            case ValidationMode.Invalid:
            case ValidationMode.AuthenticationFailed:
            case ValidationMode.InsufficientPermissions:
                logger.LogError("❌ {Message}", validationResult.Message);
                foreach (var detail in validationResult.Details)
                {
                    logger.LogError("  ❌ {Detail}", detail);
                }
                foreach (var suggestion in validationResult.Suggestions)
                {
                    logger.LogWarning("  💡 {Suggestion}", suggestion);
                }
                break;
                
            case ValidationMode.UnknownError:
                logger.LogError("🚨 {Message}", validationResult.Message);
                foreach (var detail in validationResult.Details)
                {
                    logger.LogError("  🚨 {Detail}", detail);
                }
                foreach (var suggestion in validationResult.Suggestions)
                {
                    logger.LogWarning("  💡 {Suggestion}", suggestion);
                }
                break;
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "🚨 Critical error during credential validation: {Message}", ex.Message);
        logger.LogWarning("💡 Server will continue in demo mode due to validation failure");
    }
    
    logger.LogInformation("=== Server Starting ===");
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.MapControllers();

// MCP Server endpoints
app.MapGet("/health", () => "Darbot Graph MCP Server - Enhanced");


// Authentication info endpoint
app.MapGet("/auth-info", (IAuthenticationService authService) =>
{
    return new
    {
        isConfigured = authService.IsConfigured,
        authenticationMethod = authService.AuthenticationMethod,
        timestamp = DateTime.UtcNow

// MCP validation endpoint - allows checking credential status
app.MapGet("/validate", async (ICredentialValidationService validationService) =>
{
    var result = await validationService.ValidateCredentialsAsync();
    return new
    {
        isValid = result.IsValid,
        summary = result.GetFormattedSummary(),
        validationItems = result.ValidationItems.Select(v => new 
        { 
            isValid = v.IsValid, 
            message = v.Message, 
            details = v.Details 
        }),
        suggestions = result.Suggestions

    };
});

// MCP protocol endpoints
app.MapPost("/sse", async (HttpContext context, IGraphServiceEnhanced graphService) =>
{
    context.Response.Headers["Content-Type"] = "text/event-stream";
    context.Response.Headers["Cache-Control"] = "no-cache";
    context.Response.Headers["Connection"] = "keep-alive";
    context.Response.Headers["Access-Control-Allow-Origin"] = "*";

    await context.Response.WriteAsync("data: Connected to Darbot Graph MCP Server - Enhanced\n\n");
    await context.Response.Body.FlushAsync();

    // Keep connection alive
    while (!context.RequestAborted.IsCancellationRequested)
    {
        await Task.Delay(30000, context.RequestAborted);
        await context.Response.WriteAsync("data: ping\n\n");
        await context.Response.Body.FlushAsync();
    }
});

// MCP Tools endpoint
app.MapGet("/tools", (IGraphServiceEnhanced graphService) =>
{
    return graphService.GetAvailableTools();
});

// MCP Call tool endpoint
app.MapPost("/call-tool", async (ToolCallRequest request, IGraphServiceEnhanced graphService) =>
{
    return await graphService.CallToolAsync(request.Name, request.Arguments);
});

// Perform credential validation on startup
using (var scope = app.Services.CreateScope())
{
    var validationService = scope.ServiceProvider.GetRequiredService<ICredentialValidationService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("=== Darbot Graph MCP Server Startup ===");
    logger.LogInformation("Starting credential validation...");
    
    try
    {
        var validationResult = await validationService.ValidateCredentialsAsync();
        
        logger.LogInformation("Credential validation completed");
        logger.LogInformation("Validation Summary:\n{Summary}", validationResult.GetFormattedSummary());
        
        if (!validationResult.IsValid)
        {
            logger.LogWarning("⚠️  Credential validation failed - server will run in demo mode");
            logger.LogWarning("Some MCP tools may not function correctly with invalid credentials");
        }
        else
        {
            logger.LogInformation("✅ All credential validations passed - server ready for production use");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Credential validation encountered an error");
        logger.LogWarning("Server will continue startup but may have limited functionality");
    }
    
    logger.LogInformation("=== Startup validation complete ===");
}

app.Run();

// Security validation functions
static bool IsValidTenantId(string tenantId)
{
    if (string.IsNullOrEmpty(tenantId)) return false;
    
    // GUID pattern validation - more permissive pattern
    var guidPattern = @"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$";
    return Regex.IsMatch(tenantId, guidPattern, RegexOptions.IgnoreCase);
}

static bool IsPlaceholderTenantId(string tenantId)
{
    if (string.IsNullOrEmpty(tenantId)) return false;
    
    // Common placeholder tenant IDs that should not be used in production
    var placeholderTenantIds = new[]
    {
        "00000000-0000-0000-0000-000000000000",
        "11111111-1111-1111-1111-111111111111",
        "ffffffff-ffff-ffff-ffff-ffffffffffff",
        "12345678-1234-1234-1234-123456789012"
    };
    
    return placeholderTenantIds.Contains(tenantId.ToLowerInvariant());
}

// Tool call request model
public record ToolCallRequest(string Name, JsonElement? Arguments);