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

// Configure Microsoft Graph client
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
    
    var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
    
    return new Microsoft.Graph.GraphServiceClient(credential);
});

// Configure Microsoft Graph Beta client
builder.Services.AddScoped<Microsoft.Graph.Beta.GraphServiceClient>(provider =>
{
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
    
    var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
    
    return new Microsoft.Graph.Beta.GraphServiceClient(credential);
});

// Add Enhanced Graph service
builder.Services.AddScoped<IGraphServiceEnhanced, GraphServiceEnhanced>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.MapControllers();

// MCP Server endpoints
app.MapGet("/health", () => "Darbot Graph MCP Server - Enhanced");

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