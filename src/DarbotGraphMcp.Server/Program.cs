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
    var configuration = provider.GetRequiredService<IConfiguration>();
    var logger = provider.GetRequiredService<ILogger<Program>>();
    
    var clientId = configuration["AzureAd:ClientId"];
    var clientSecret = configuration["AzureAd:ClientSecret"];
    var tenantId = configuration["AzureAd:TenantId"];
    
    // Always use placeholder values for the Graph client construction
    // Real credential validation happens at startup and in the enhanced service
    var placeholderTenantId = "00000000-0000-0000-0000-000000000000";
    var placeholderClientId = "00000000-0000-0000-0000-000000000000";
    var placeholderSecret = "placeholder-secret";
    
    var credential = new ClientSecretCredential(placeholderTenantId, placeholderClientId, placeholderSecret);
    
    return new Microsoft.Graph.Beta.GraphServiceClient(credential);
});

// Add Enhanced Graph service
builder.Services.AddScoped<IGraphServiceEnhanced, GraphServiceEnhanced>();

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

// Tool call request model
public record ToolCallRequest(string Name, JsonElement? Arguments);