using Microsoft.Graph;
using Microsoft.Graph.Beta;
using Azure.Identity;
using System.Text.Json;
using DarbotGraphMcp.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddLogging();
builder.Services.AddControllers();

// Add credential validation service
builder.Services.AddScoped<ICredentialValidationService, CredentialValidationService>();

// Configure Microsoft Graph client
builder.Services.AddScoped<Microsoft.Graph.GraphServiceClient>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var logger = provider.GetRequiredService<ILogger<Program>>();
    
    // Use ClientSecretCredential for app-only authentication
    var clientId = configuration["AzureAd:ClientId"];
    var clientSecret = configuration["AzureAd:ClientSecret"];
    var tenantId = configuration["AzureAd:TenantId"];
    
    // Check if we have real credentials or if we're in demo mode
    bool isDemoMode = string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(tenantId);
    
    if (isDemoMode)
    {
        logger.LogWarning("Azure AD credentials not configured - running in demo mode");
        // For demo/testing purposes, use placeholder values if not configured
        clientId = "00000000-0000-0000-0000-000000000000";
        clientSecret = "placeholder-secret";
        tenantId = "00000000-0000-0000-0000-000000000000";
    }
    else
    {
        logger.LogInformation("Azure AD credentials configured - production mode");
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
    
    // Check if we have real credentials or if we're in demo mode
    bool isDemoMode = string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(tenantId);
    
    if (isDemoMode)
    {
        // For demo/testing purposes, use placeholder values if not configured
        clientId = "00000000-0000-0000-0000-000000000000";
        clientSecret = "placeholder-secret";
        tenantId = "00000000-0000-0000-0000-000000000000";
    }
    
    var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
    
    return new Microsoft.Graph.Beta.GraphServiceClient(credential);
});

// Add Enhanced Graph service
builder.Services.AddScoped<IGraphServiceEnhanced, GraphServiceEnhanced>();

// Add Tenant Validation service for security
builder.Services.AddScoped<ITenantValidationService, TenantValidationService>();

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

// Tool call request model
public record ToolCallRequest(string Name, JsonElement? Arguments);