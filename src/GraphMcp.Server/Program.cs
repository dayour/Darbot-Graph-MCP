using Microsoft.Graph;
using Azure.Identity;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddLogging();
builder.Services.AddControllers();

// Configure Microsoft Graph client
builder.Services.AddScoped<GraphServiceClient>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    
    // Use ClientSecretCredential for app-only authentication
    var clientId = configuration["AzureAd:ClientId"] ?? throw new InvalidOperationException("Missing AzureAd:ClientId");
    var clientSecret = configuration["AzureAd:ClientSecret"] ?? throw new InvalidOperationException("Missing AzureAd:ClientSecret");
    var tenantId = configuration["AzureAd:TenantId"] ?? throw new InvalidOperationException("Missing AzureAd:TenantId");
    
    var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
    
    return new GraphServiceClient(credential);
});

// Add Graph service
builder.Services.AddScoped<IGraphService, GraphService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.MapControllers();

// MCP Server endpoints
app.MapGet("/health", () => "Microsoft Graph MCP Server - Healthy");

// MCP protocol endpoints
app.MapPost("/sse", async (HttpContext context, IGraphService graphService) =>
{
    context.Response.Headers["Content-Type"] = "text/event-stream";
    context.Response.Headers["Cache-Control"] = "no-cache";
    context.Response.Headers["Connection"] = "keep-alive";
    context.Response.Headers["Access-Control-Allow-Origin"] = "*";

    await context.Response.WriteAsync("data: Connected to Microsoft Graph MCP Server\n\n");
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
app.MapGet("/tools", (IGraphService graphService) =>
{
    return graphService.GetAvailableTools();
});

// MCP Call tool endpoint
app.MapPost("/call-tool", async (ToolCallRequest request, IGraphService graphService) =>
{
    return await graphService.CallToolAsync(request.Name, request.Arguments);
});

app.Run();

// Graph Service Interface
public interface IGraphService
{
    List<object> GetAvailableTools();
    Task<object> CallToolAsync(string toolName, JsonElement? arguments);
}

// Graph Service Implementation
public class GraphService : IGraphService
{
    private readonly GraphServiceClient _graphClient;
    private readonly ILogger<GraphService> _logger;

    public GraphService(GraphServiceClient graphClient, ILogger<GraphService> logger)
    {
        _graphClient = graphClient;
        _logger = logger;
    }

    public List<object> GetAvailableTools()
    {
        return new List<object>
        {
            new
            {
                name = "get_users",
                description = "Get a list of users from Microsoft Graph",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        top = new { type = "integer", description = "Number of users to return (max 999)" },
                        filter = new { type = "string", description = "OData filter expression" },
                        search = new { type = "string", description = "Search query" }
                    }
                }
            },
            new
            {
                name = "get_user",
                description = "Get a specific user by ID or UPN",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" }
                    },
                    required = new[] { "userId" }
                }
            },
            new
            {
                name = "create_user",
                description = "Create a new user",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        displayName = new { type = "string", description = "Display name of the user" },
                        userPrincipalName = new { type = "string", description = "User principal name (email)" },
                        mailNickname = new { type = "string", description = "Mail nickname" },
                        password = new { type = "string", description = "Temporary password" }
                    },
                    required = new[] { "displayName", "userPrincipalName", "mailNickname", "password" }
                }
            },
            new
            {
                name = "get_groups",
                description = "Get a list of groups from Microsoft Graph",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        top = new { type = "integer", description = "Number of groups to return (max 999)" },
                        filter = new { type = "string", description = "OData filter expression" }
                    }
                }
            },
            new
            {
                name = "get_group",
                description = "Get a specific group by ID",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        groupId = new { type = "string", description = "Group ID" }
                    },
                    required = new[] { "groupId" }
                }
            },
            new
            {
                name = "create_group",
                description = "Create a new group",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        displayName = new { type = "string", description = "Display name of the group" },
                        mailNickname = new { type = "string", description = "Mail nickname" },
                        description = new { type = "string", description = "Group description" },
                        groupType = new { type = "string", description = "Type of group (Security, Microsoft365)" }
                    },
                    required = new[] { "displayName", "mailNickname" }
                }
            },
            new
            {
                name = "send_mail",
                description = "Send an email",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        to = new { type = "array", items = new { type = "string" }, description = "Recipient email addresses" },
                        subject = new { type = "string", description = "Email subject" },
                        body = new { type = "string", description = "Email body content" },
                        bodyType = new { type = "string", description = "Body content type (Text or Html)", @enum = new[] { "Text", "Html" } }
                    },
                    required = new[] { "to", "subject", "body" }
                }
            }
        };
    }

    public async Task<object> CallToolAsync(string toolName, JsonElement? arguments)
    {
        try
        {
            return toolName switch
            {
                "get_users" => await GetUsersAsync(arguments),
                "get_user" => await GetUserAsync(arguments),
                "create_user" => await CreateUserAsync(arguments),
                "get_groups" => await GetGroupsAsync(arguments),
                "get_group" => await GetGroupAsync(arguments),
                "create_group" => await CreateGroupAsync(arguments),
                "send_mail" => await SendMailAsync(arguments),
                _ => new { error = $"Unknown tool: {toolName}" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling tool {ToolName}", toolName);
            return new { error = ex.Message };
        }
    }

    private async Task<object> GetUsersAsync(JsonElement? arguments)
    {
        var args = arguments?.Deserialize<GetUsersArgs>();
        
        var request = _graphClient.Users.GetAsync(requestConfig =>
        {
            if (args?.Top.HasValue == true)
                requestConfig.QueryParameters.Top = args.Top.Value;
            if (!string.IsNullOrEmpty(args?.Filter))
                requestConfig.QueryParameters.Filter = args.Filter;
            if (!string.IsNullOrEmpty(args?.Search))
                requestConfig.QueryParameters.Search = args.Search;
        });

        var users = await request;
        var userList = users?.Value?.Select(u => new
        {
            u.Id,
            u.DisplayName,
            u.UserPrincipalName,
            u.Mail,
            u.JobTitle,
            u.Department
        }).ToList();

        return new { users = userList };
    }

    private async Task<object> GetUserAsync(JsonElement? arguments)
    {
        var args = arguments?.Deserialize<GetUserArgs>();
        
        if (string.IsNullOrEmpty(args?.UserId))
        {
            return new { error = "UserId is required" };
        }

        var user = await _graphClient.Users[args.UserId].GetAsync();
        
        var userInfo = new
        {
            user?.Id,
            user?.DisplayName,
            user?.UserPrincipalName,
            user?.Mail,
            user?.JobTitle,
            user?.Department,
            user?.OfficeLocation,
            user?.MobilePhone,
            user?.BusinessPhones
        };

        return new { user = userInfo };
    }

    private async Task<object> CreateUserAsync(JsonElement? arguments)
    {
        var args = arguments?.Deserialize<CreateUserArgs>();
        
        if (string.IsNullOrEmpty(args?.DisplayName) || string.IsNullOrEmpty(args?.UserPrincipalName))
        {
            return new { error = "DisplayName and UserPrincipalName are required" };
        }

        var newUser = new Microsoft.Graph.Models.User
        {
            DisplayName = args.DisplayName,
            UserPrincipalName = args.UserPrincipalName,
            MailNickname = args.MailNickname ?? args.UserPrincipalName.Split('@')[0],
            AccountEnabled = true,
            PasswordProfile = new Microsoft.Graph.Models.PasswordProfile
            {
                Password = args.Password,
                ForceChangePasswordNextSignIn = true
            }
        };

        var createdUser = await _graphClient.Users.PostAsync(newUser);

        return new { message = $"User created successfully", userId = createdUser?.Id };
    }

    private async Task<object> GetGroupsAsync(JsonElement? arguments)
    {
        var args = arguments?.Deserialize<GetGroupsArgs>();
        
        var request = _graphClient.Groups.GetAsync(requestConfig =>
        {
            if (args?.Top.HasValue == true)
                requestConfig.QueryParameters.Top = args.Top.Value;
            if (!string.IsNullOrEmpty(args?.Filter))
                requestConfig.QueryParameters.Filter = args.Filter;
        });

        var groups = await request;
        var groupList = groups?.Value?.Select(g => new
        {
            g.Id,
            g.DisplayName,
            g.Description,
            g.Mail,
            g.GroupTypes
        }).ToList();

        return new { groups = groupList };
    }

    private async Task<object> GetGroupAsync(JsonElement? arguments)
    {
        var args = arguments?.Deserialize<GetGroupArgs>();
        
        if (string.IsNullOrEmpty(args?.GroupId))
        {
            return new { error = "GroupId is required" };
        }

        var group = await _graphClient.Groups[args.GroupId].GetAsync();
        
        var groupInfo = new
        {
            group?.Id,
            group?.DisplayName,
            group?.Description,
            group?.Mail,
            group?.GroupTypes,
            group?.CreatedDateTime
        };

        return new { group = groupInfo };
    }

    private async Task<object> CreateGroupAsync(JsonElement? arguments)
    {
        var args = arguments?.Deserialize<CreateGroupArgs>();
        
        if (string.IsNullOrEmpty(args?.DisplayName) || string.IsNullOrEmpty(args?.MailNickname))
        {
            return new { error = "DisplayName and MailNickname are required" };
        }

        var newGroup = new Microsoft.Graph.Models.Group
        {
            DisplayName = args.DisplayName,
            MailNickname = args.MailNickname,
            Description = args.Description,
            GroupTypes = args.GroupType?.ToLower() == "microsoft365" ? new List<string> { "Unified" } : new List<string>(),
            SecurityEnabled = true,
            MailEnabled = args.GroupType?.ToLower() == "microsoft365"
        };

        var createdGroup = await _graphClient.Groups.PostAsync(newGroup);

        return new { message = "Group created successfully", groupId = createdGroup?.Id };
    }

    private async Task<object> SendMailAsync(JsonElement? arguments)
    {
        var args = arguments?.Deserialize<SendMailArgs>();
        
        if (args?.To == null || !args.To.Any() || string.IsNullOrEmpty(args.Subject) || string.IsNullOrEmpty(args.Body))
        {
            return new { error = "To, Subject, and Body are required" };
        }

        var message = new Microsoft.Graph.Models.Message
        {
            Subject = args.Subject,
            Body = new Microsoft.Graph.Models.ItemBody
            {
                ContentType = args.BodyType?.ToLower() == "html" ? Microsoft.Graph.Models.BodyType.Html : Microsoft.Graph.Models.BodyType.Text,
                Content = args.Body
            },
            ToRecipients = args.To.Select(email => new Microsoft.Graph.Models.Recipient
            {
                EmailAddress = new Microsoft.Graph.Models.EmailAddress
                {
                    Address = email
                }
            }).ToList()
        };

        var sendMailRequest = new Microsoft.Graph.Me.SendMail.SendMailPostRequestBody
        {
            Message = message
        };

        await _graphClient.Me.SendMail.PostAsync(sendMailRequest);

        return new { message = "Email sent successfully" };
    }
}

// Request/Response models
public record GetUsersArgs(int? Top, string? Filter, string? Search);
public record GetUserArgs(string? UserId);
public record CreateUserArgs(string? DisplayName, string? UserPrincipalName, string? MailNickname, string? Password);
public record GetGroupsArgs(int? Top, string? Filter);
public record GetGroupArgs(string? GroupId);
public record CreateGroupArgs(string? DisplayName, string? MailNickname, string? Description, string? GroupType);
public record SendMailArgs(List<string>? To, string? Subject, string? Body, string? BodyType);
public record ToolCallRequest(string Name, JsonElement? Arguments);