using Microsoft.Graph;
using Microsoft.Graph.Beta;
using Azure.Identity;
using System.Text.Json;
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
});

// Configure Microsoft Graph Beta client
builder.Services.AddScoped<Microsoft.Graph.Beta.GraphServiceClient>(provider =>
{
    var authService = provider.GetRequiredService<IAuthenticationService>();
    return authService.CreateBetaGraphServiceClient();
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

// Authentication info endpoint
app.MapGet("/auth-info", (IAuthenticationService authService) =>
{
    return new
    {
        isConfigured = authService.IsConfigured,
        authenticationMethod = authService.AuthenticationMethod,
        timestamp = DateTime.UtcNow
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

app.Run();

// Tool call request model
public record ToolCallRequest(string Name, JsonElement? Arguments);