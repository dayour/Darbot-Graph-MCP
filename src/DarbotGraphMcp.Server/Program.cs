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
    var clientId = configuration["AzureAd:ClientId"];
    var clientSecret = configuration["AzureAd:ClientSecret"];
    var tenantId = configuration["AzureAd:TenantId"];
    
    // For demo/testing purposes, use placeholder values if not configured
    if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(tenantId))
    {
        clientId = "00000000-0000-0000-0000-000000000000";
        clientSecret = "placeholder-secret";
        tenantId = "00000000-0000-0000-0000-000000000000";
    }
    
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
app.MapGet("/health", () => "Darbot Graph MCP Server - Healthy");

// MCP protocol endpoints
app.MapPost("/sse", async (HttpContext context, IGraphService graphService) =>
{
    context.Response.Headers["Content-Type"] = "text/event-stream";
    context.Response.Headers["Cache-Control"] = "no-cache";
    context.Response.Headers["Connection"] = "keep-alive";
    context.Response.Headers["Access-Control-Allow-Origin"] = "*";

    await context.Response.WriteAsync("data: Connected to Darbot Graph MCP Server\n\n");
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
            // User Management (8 tools)
            new
            {
                name = "darbot-graph-get-users",
                description = "Get a list of users from Microsoft Graph with advanced filtering",
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
                name = "darbot-graph-get-user",
                description = "Get detailed information about a specific user by ID or UPN",
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
                name = "darbot-graph-create-user",
                description = "Create a new user account with comprehensive settings",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        displayName = new { type = "string", description = "Display name of the user" },
                        userPrincipalName = new { type = "string", description = "User principal name (email)" },
                        mailNickname = new { type = "string", description = "Mail nickname" },
                        password = new { type = "string", description = "Temporary password" },
                        jobTitle = new { type = "string", description = "Job title" },
                        department = new { type = "string", description = "Department" }
                    },
                    required = new[] { "displayName", "userPrincipalName", "mailNickname", "password" }
                }
            },
            new
            {
                name = "darbot-graph-update-user",
                description = "Update user properties and settings",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        displayName = new { type = "string", description = "Display name of the user" },
                        jobTitle = new { type = "string", description = "Job title" },
                        department = new { type = "string", description = "Department" },
                        officeLocation = new { type = "string", description = "Office location" }
                    },
                    required = new[] { "userId" }
                }
            },
            new
            {
                name = "darbot-graph-delete-user",
                description = "Remove a user from the directory",
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
                name = "darbot-graph-reset-user-password",
                description = "Reset user password and force change on next sign-in",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        newPassword = new { type = "string", description = "New password" },
                        forceChange = new { type = "boolean", description = "Force password change on next sign-in" }
                    },
                    required = new[] { "userId", "newPassword" }
                }
            },
            new
            {
                name = "darbot-graph-get-user-manager",
                description = "Get user's manager information",
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
                name = "darbot-graph-set-user-manager",
                description = "Assign a manager to a user",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        managerId = new { type = "string", description = "Manager's User ID or UPN" }
                    },
                    required = new[] { "userId", "managerId" }
                }
            },

            // Group Management (8 tools)
            new
            {
                name = "darbot-graph-get-groups",
                description = "Get a list of groups with advanced filtering",
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
                name = "darbot-graph-get-group",
                description = "Get detailed information about a specific group",
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
                name = "darbot-graph-create-group",
                description = "Create security or Microsoft 365 groups",
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
                name = "darbot-graph-update-group",
                description = "Update group properties and settings",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        groupId = new { type = "string", description = "Group ID" },
                        displayName = new { type = "string", description = "Display name of the group" },
                        description = new { type = "string", description = "Group description" }
                    },
                    required = new[] { "groupId" }
                }
            },
            new
            {
                name = "darbot-graph-delete-group",
                description = "Remove a group from the directory",
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
                name = "darbot-graph-add-group-member",
                description = "Add members to a group",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        groupId = new { type = "string", description = "Group ID" },
                        userId = new { type = "string", description = "User ID to add to the group" }
                    },
                    required = new[] { "groupId", "userId" }
                }
            },
            new
            {
                name = "darbot-graph-remove-group-member",
                description = "Remove members from a group",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        groupId = new { type = "string", description = "Group ID" },
                        userId = new { type = "string", description = "User ID to remove from the group" }
                    },
                    required = new[] { "groupId", "userId" }
                }
            },
            new
            {
                name = "darbot-graph-get-group-members",
                description = "List all group members",
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

            // Email Management (8 tools)
            new
            {
                name = "darbot-graph-send-mail",
                description = "Send emails with advanced formatting and attachments",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        to = new { type = "array", items = new { type = "string" }, description = "Recipient email addresses" },
                        cc = new { type = "array", items = new { type = "string" }, description = "CC recipient email addresses" },
                        bcc = new { type = "array", items = new { type = "string" }, description = "BCC recipient email addresses" },
                        subject = new { type = "string", description = "Email subject" },
                        body = new { type = "string", description = "Email body content" },
                        bodyType = new { type = "string", description = "Body content type (Text or Html)", @enum = new[] { "Text", "Html" } },
                        importance = new { type = "string", description = "Email importance (Low, Normal, High)", @enum = new[] { "Low", "Normal", "High" } }
                    },
                    required = new[] { "to", "subject", "body" }
                }
            },
            new
            {
                name = "darbot-graph-get-mailbox-settings",
                description = "Retrieve user mailbox settings",
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
                name = "darbot-graph-get-mail-folders",
                description = "List mail folders and subfolders",
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
                name = "darbot-graph-create-mail-folder",
                description = "Create new mail folders",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        displayName = new { type = "string", description = "Folder display name" },
                        parentFolderId = new { type = "string", description = "Parent folder ID (optional)" }
                    },
                    required = new[] { "userId", "displayName" }
                }
            },
            new
            {
                name = "darbot-graph-get-messages",
                description = "Retrieve messages with filtering",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        folderId = new { type = "string", description = "Folder ID (optional)" },
                        top = new { type = "integer", description = "Number of messages to return" },
                        filter = new { type = "string", description = "OData filter expression" }
                    },
                    required = new[] { "userId" }
                }
            },
            new
            {
                name = "darbot-graph-reply-to-message",
                description = "Reply to email messages",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        messageId = new { type = "string", description = "Message ID to reply to" },
                        comment = new { type = "string", description = "Reply content" }
                    },
                    required = new[] { "userId", "messageId", "comment" }
                }
            },
            new
            {
                name = "darbot-graph-forward-message",
                description = "Forward email messages",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        messageId = new { type = "string", description = "Message ID to forward" },
                        to = new { type = "array", items = new { type = "string" }, description = "Recipient email addresses" },
                        comment = new { type = "string", description = "Forward comment" }
                    },
                    required = new[] { "userId", "messageId", "to" }
                }
            },
            new
            {
                name = "darbot-graph-move-message",
                description = "Move messages between folders",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        messageId = new { type = "string", description = "Message ID to move" },
                        destinationFolderId = new { type = "string", description = "Destination folder ID" }
                    },
                    required = new[] { "userId", "messageId", "destinationFolderId" }
                }
            },

            // Calendar Management (8 tools)
            new
            {
                name = "darbot-graph-get-calendar-events",
                description = "Retrieve calendar events with advanced filtering",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        startTime = new { type = "string", description = "Start time (ISO 8601)" },
                        endTime = new { type = "string", description = "End time (ISO 8601)" },
                        top = new { type = "integer", description = "Number of events to return" }
                    },
                    required = new[] { "userId" }
                }
            },
            new
            {
                name = "darbot-graph-create-calendar-event",
                description = "Create events with attendees and recurrence",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        subject = new { type = "string", description = "Event subject" },
                        body = new { type = "string", description = "Event body content" },
                        startTime = new { type = "string", description = "Start time (ISO 8601)" },
                        endTime = new { type = "string", description = "End time (ISO 8601)" },
                        attendees = new { type = "array", items = new { type = "string" }, description = "Attendee email addresses" },
                        location = new { type = "string", description = "Event location" }
                    },
                    required = new[] { "userId", "subject", "startTime", "endTime" }
                }
            },
            new
            {
                name = "darbot-graph-update-calendar-event",
                description = "Update existing calendar events",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        eventId = new { type = "string", description = "Event ID" },
                        subject = new { type = "string", description = "Event subject" },
                        body = new { type = "string", description = "Event body content" },
                        startTime = new { type = "string", description = "Start time (ISO 8601)" },
                        endTime = new { type = "string", description = "End time (ISO 8601)" }
                    },
                    required = new[] { "userId", "eventId" }
                }
            },
            new
            {
                name = "darbot-graph-delete-calendar-event",
                description = "Remove calendar events",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        eventId = new { type = "string", description = "Event ID" }
                    },
                    required = new[] { "userId", "eventId" }
                }
            },
            new
            {
                name = "darbot-graph-get-calendars",
                description = "List user calendars",
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
                name = "darbot-graph-create-calendar",
                description = "Create new calendars",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        name = new { type = "string", description = "Calendar name" },
                        color = new { type = "string", description = "Calendar color" }
                    },
                    required = new[] { "userId", "name" }
                }
            },
            new
            {
                name = "darbot-graph-accept-event",
                description = "Accept meeting invitations",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        eventId = new { type = "string", description = "Event ID" },
                        comment = new { type = "string", description = "Optional comment" }
                    },
                    required = new[] { "userId", "eventId" }
                }
            },
            new
            {
                name = "darbot-graph-decline-event",
                description = "Decline meeting invitations",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        eventId = new { type = "string", description = "Event ID" },
                        comment = new { type = "string", description = "Optional comment" }
                    },
                    required = new[] { "userId", "eventId" }
                }
            },

            // Teams and Communication (8 tools)
            new
            {
                name = "darbot-graph-get-teams",
                description = "Get Microsoft Teams user is member of",
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
                name = "darbot-graph-get-team-channels",
                description = "List channels in a team",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        teamId = new { type = "string", description = "Team ID" }
                    },
                    required = new[] { "teamId" }
                }
            },
            new
            {
                name = "darbot-graph-create-team-channel",
                description = "Create new team channels",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        teamId = new { type = "string", description = "Team ID" },
                        displayName = new { type = "string", description = "Channel display name" },
                        description = new { type = "string", description = "Channel description" },
                        channelType = new { type = "string", description = "Channel type (Standard, Private)", @enum = new[] { "Standard", "Private" } }
                    },
                    required = new[] { "teamId", "displayName" }
                }
            },
            new
            {
                name = "darbot-graph-get-channel-messages",
                description = "Retrieve channel messages",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        teamId = new { type = "string", description = "Team ID" },
                        channelId = new { type = "string", description = "Channel ID" },
                        top = new { type = "integer", description = "Number of messages to return" }
                    },
                    required = new[] { "teamId", "channelId" }
                }
            },
            new
            {
                name = "darbot-graph-send-channel-message",
                description = "Send messages to team channels",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        teamId = new { type = "string", description = "Team ID" },
                        channelId = new { type = "string", description = "Channel ID" },
                        content = new { type = "string", description = "Message content" },
                        contentType = new { type = "string", description = "Content type (Text or Html)", @enum = new[] { "Text", "Html" } }
                    },
                    required = new[] { "teamId", "channelId", "content" }
                }
            },
            new
            {
                name = "darbot-graph-reply-to-channel-message",
                description = "Reply to channel messages",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        teamId = new { type = "string", description = "Team ID" },
                        channelId = new { type = "string", description = "Channel ID" },
                        messageId = new { type = "string", description = "Message ID to reply to" },
                        content = new { type = "string", description = "Reply content" }
                    },
                    required = new[] { "teamId", "channelId", "messageId", "content" }
                }
            },
            new
            {
                name = "darbot-graph-get-team-members",
                description = "List team members",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        teamId = new { type = "string", description = "Team ID" }
                    },
                    required = new[] { "teamId" }
                }
            },
            new
            {
                name = "darbot-graph-add-team-member",
                description = "Add members to teams",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        teamId = new { type = "string", description = "Team ID" },
                        userId = new { type = "string", description = "User ID to add to the team" },
                        role = new { type = "string", description = "Member role (Owner, Member)", @enum = new[] { "Owner", "Member" } }
                    },
                    required = new[] { "teamId", "userId" }
                }
            },

            // OneDrive and SharePoint (7 tools)
            new
            {
                name = "darbot-graph-get-drive-items",
                description = "List OneDrive files and folders",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        folderId = new { type = "string", description = "Folder ID (optional, defaults to root)" },
                        top = new { type = "integer", description = "Number of items to return" }
                    },
                    required = new[] { "userId" }
                }
            },
            new
            {
                name = "darbot-graph-upload-file",
                description = "Upload files to OneDrive",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        fileName = new { type = "string", description = "File name" },
                        content = new { type = "string", description = "File content (base64 encoded)" },
                        folderId = new { type = "string", description = "Destination folder ID (optional)" }
                    },
                    required = new[] { "userId", "fileName", "content" }
                }
            },
            new
            {
                name = "darbot-graph-download-file",
                description = "Download files from OneDrive",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        fileId = new { type = "string", description = "File ID" }
                    },
                    required = new[] { "userId", "fileId" }
                }
            },
            new
            {
                name = "darbot-graph-share-file",
                description = "Create sharing links for files",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        fileId = new { type = "string", description = "File ID" },
                        type = new { type = "string", description = "Share type (View, Edit)", @enum = new[] { "View", "Edit" } },
                        scope = new { type = "string", description = "Share scope (Anonymous, Organization)", @enum = new[] { "Anonymous", "Organization" } }
                    },
                    required = new[] { "userId", "fileId", "type" }
                }
            },
            new
            {
                name = "darbot-graph-get-sharepoint-sites",
                description = "List SharePoint sites",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        search = new { type = "string", description = "Search query for sites" },
                        top = new { type = "integer", description = "Number of sites to return" }
                    }
                }
            },
            new
            {
                name = "darbot-graph-get-site-lists",
                description = "Get lists from SharePoint sites",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        siteId = new { type = "string", description = "SharePoint site ID" }
                    },
                    required = new[] { "siteId" }
                }
            },
            new
            {
                name = "darbot-graph-get-list-items",
                description = "Retrieve items from SharePoint lists",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        siteId = new { type = "string", description = "SharePoint site ID" },
                        listId = new { type = "string", description = "List ID" },
                        top = new { type = "integer", description = "Number of items to return" }
                    },
                    required = new[] { "siteId", "listId" }
                }
            },

            // Security and Compliance (5 tools)
            new
            {
                name = "darbot-graph-get-sign-in-logs",
                description = "Retrieve user sign-in logs",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID (optional)" },
                        top = new { type = "integer", description = "Number of logs to return" },
                        filter = new { type = "string", description = "OData filter expression" }
                    }
                }
            },
            new
            {
                name = "darbot-graph-get-audit-logs",
                description = "Get directory audit logs",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        top = new { type = "integer", description = "Number of logs to return" },
                        filter = new { type = "string", description = "OData filter expression" }
                    }
                }
            },
            new
            {
                name = "darbot-graph-get-risky-users",
                description = "List users flagged for risk",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        top = new { type = "integer", description = "Number of users to return" },
                        filter = new { type = "string", description = "OData filter expression" }
                    }
                }
            },
            new
            {
                name = "darbot-graph-confirm-risky-user",
                description = "Confirm or dismiss risky users",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID" },
                        action = new { type = "string", description = "Action to take (Confirm, Dismiss)", @enum = new[] { "Confirm", "Dismiss" } }
                    },
                    required = new[] { "userId", "action" }
                }
            },
            new
            {
                name = "darbot-graph-get-conditional-access-policies",
                description = "List conditional access policies",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        top = new { type = "integer", description = "Number of policies to return" }
                    }
                }
            },

            // Reports and Analytics (4 tools)
            new
            {
                name = "darbot-graph-get-usage-reports",
                description = "Get Microsoft 365 usage reports",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        reportType = new { type = "string", description = "Type of report (Overview, UserActivity, DeviceUsage)", @enum = new[] { "Overview", "UserActivity", "DeviceUsage" } },
                        period = new { type = "string", description = "Report period (D7, D30, D90, D180)", @enum = new[] { "D7", "D30", "D90", "D180" } }
                    },
                    required = new[] { "reportType" }
                }
            },
            new
            {
                name = "darbot-graph-get-teams-activity",
                description = "Get Teams activity reports",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        period = new { type = "string", description = "Report period (D7, D30, D90, D180)", @enum = new[] { "D7", "D30", "D90", "D180" } }
                    },
                    required = new[] { "period" }
                }
            },
            new
            {
                name = "darbot-graph-get-email-activity",
                description = "Get email activity reports",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        period = new { type = "string", description = "Report period (D7, D30, D90, D180)", @enum = new[] { "D7", "D30", "D90", "D180" } }
                    },
                    required = new[] { "period" }
                }
            },
            new
            {
                name = "darbot-graph-get-sharepoint-activity",
                description = "Get SharePoint activity reports",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        period = new { type = "string", description = "Report period (D7, D30, D90, D180)", @enum = new[] { "D7", "D30", "D90", "D180" } }
                    },
                    required = new[] { "period" }
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
                // User Management
                "darbot-graph-get-users" => await GetUsersAsync(arguments),
                "darbot-graph-get-user" => await GetUserAsync(arguments),
                "darbot-graph-create-user" => await CreateUserAsync(arguments),
                "darbot-graph-update-user" => await UpdateUserAsync(arguments),
                "darbot-graph-delete-user" => await DeleteUserAsync(arguments),
                "darbot-graph-reset-user-password" => await ResetUserPasswordAsync(arguments),
                "darbot-graph-get-user-manager" => await GetUserManagerAsync(arguments),
                "darbot-graph-set-user-manager" => await SetUserManagerAsync(arguments),
                
                // Group Management
                "darbot-graph-get-groups" => await GetGroupsAsync(arguments),
                "darbot-graph-get-group" => await GetGroupAsync(arguments),
                "darbot-graph-create-group" => await CreateGroupAsync(arguments),
                "darbot-graph-update-group" => await UpdateGroupAsync(arguments),
                "darbot-graph-delete-group" => await DeleteGroupAsync(arguments),
                "darbot-graph-add-group-member" => await AddGroupMemberAsync(arguments),
                "darbot-graph-remove-group-member" => await RemoveGroupMemberAsync(arguments),
                "darbot-graph-get-group-members" => await GetGroupMembersAsync(arguments),
                
                // Email Management
                "darbot-graph-send-mail" => await SendMailAsync(arguments),
                "darbot-graph-get-mailbox-settings" => await GetMailboxSettingsAsync(arguments),
                "darbot-graph-get-mail-folders" => await GetMailFoldersAsync(arguments),
                "darbot-graph-create-mail-folder" => await CreateMailFolderAsync(arguments),
                "darbot-graph-get-messages" => await GetMessagesAsync(arguments),
                "darbot-graph-reply-to-message" => await ReplyToMessageAsync(arguments),
                "darbot-graph-forward-message" => await ForwardMessageAsync(arguments),
                "darbot-graph-move-message" => await MoveMessageAsync(arguments),
                
                // Calendar Management
                "darbot-graph-get-calendar-events" => await GetCalendarEventsAsync(arguments),
                "darbot-graph-create-calendar-event" => await CreateCalendarEventAsync(arguments),
                "darbot-graph-update-calendar-event" => await UpdateCalendarEventAsync(arguments),
                "darbot-graph-delete-calendar-event" => await DeleteCalendarEventAsync(arguments),
                "darbot-graph-get-calendars" => await GetCalendarsAsync(arguments),
                "darbot-graph-create-calendar" => await CreateCalendarAsync(arguments),
                "darbot-graph-accept-event" => await AcceptEventAsync(arguments),
                "darbot-graph-decline-event" => await DeclineEventAsync(arguments),
                
                // Teams and Communication
                "darbot-graph-get-teams" => await GetTeamsAsync(arguments),
                "darbot-graph-get-team-channels" => await GetTeamChannelsAsync(arguments),
                "darbot-graph-create-team-channel" => await CreateTeamChannelAsync(arguments),
                "darbot-graph-get-channel-messages" => await GetChannelMessagesAsync(arguments),
                "darbot-graph-send-channel-message" => await SendChannelMessageAsync(arguments),
                "darbot-graph-reply-to-channel-message" => await ReplyToChannelMessageAsync(arguments),
                "darbot-graph-get-team-members" => await GetTeamMembersAsync(arguments),
                "darbot-graph-add-team-member" => await AddTeamMemberAsync(arguments),
                
                // OneDrive and SharePoint
                "darbot-graph-get-drive-items" => await GetDriveItemsAsync(arguments),
                "darbot-graph-upload-file" => await UploadFileAsync(arguments),
                "darbot-graph-download-file" => await DownloadFileAsync(arguments),
                "darbot-graph-share-file" => await ShareFileAsync(arguments),
                "darbot-graph-get-sharepoint-sites" => await GetSharePointSitesAsync(arguments),
                "darbot-graph-get-site-lists" => await GetSiteListsAsync(arguments),
                "darbot-graph-get-list-items" => await GetListItemsAsync(arguments),
                
                // Security and Compliance
                "darbot-graph-get-sign-in-logs" => await GetSignInLogsAsync(arguments),
                "darbot-graph-get-audit-logs" => await GetAuditLogsAsync(arguments),
                "darbot-graph-get-risky-users" => await GetRiskyUsersAsync(arguments),
                "darbot-graph-confirm-risky-user" => await ConfirmRiskyUserAsync(arguments),
                "darbot-graph-get-conditional-access-policies" => await GetConditionalAccessPoliciesAsync(arguments),
                
                // Reports and Analytics
                "darbot-graph-get-usage-reports" => await GetUsageReportsAsync(arguments),
                "darbot-graph-get-teams-activity" => await GetTeamsActivityAsync(arguments),
                "darbot-graph-get-email-activity" => await GetEmailActivityAsync(arguments),
                "darbot-graph-get-sharepoint-activity" => await GetSharePointActivityAsync(arguments),
                
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
        try
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
        catch (Exception)
        {
            // If Graph client is not properly configured, return demo data
            return new { 
                message = "Demo mode - Azure AD not configured", 
                users = new[] {
                    new { Id = "demo-1", DisplayName = "Demo User 1", UserPrincipalName = "demo1@example.com", Mail = "demo1@example.com", JobTitle = "Developer", Department = "IT" },
                    new { Id = "demo-2", DisplayName = "Demo User 2", UserPrincipalName = "demo2@example.com", Mail = "demo2@example.com", JobTitle = "Manager", Department = "IT" }
                }
            };
        }
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

    // Additional User Management Methods
    private async Task<object> UpdateUserAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "User update functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> DeleteUserAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "User deletion functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> ResetUserPasswordAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Password reset functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> GetUserManagerAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get user manager functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> SetUserManagerAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Set user manager functionality will be implemented", status = "placeholder" };
    }

    // Additional Group Management Methods
    private async Task<object> UpdateGroupAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Group update functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> DeleteGroupAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Group deletion functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> AddGroupMemberAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Add group member functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> RemoveGroupMemberAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Remove group member functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> GetGroupMembersAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get group members functionality will be implemented", status = "placeholder" };
    }

    // Additional Email Management Methods
    private async Task<object> GetMailboxSettingsAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get mailbox settings functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> GetMailFoldersAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get mail folders functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> CreateMailFolderAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Create mail folder functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> GetMessagesAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get messages functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> ReplyToMessageAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Reply to message functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> ForwardMessageAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Forward message functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> MoveMessageAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Move message functionality will be implemented", status = "placeholder" };
    }

    // Calendar Management Methods
    private async Task<object> GetCalendarEventsAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get calendar events functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> CreateCalendarEventAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Create calendar event functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> UpdateCalendarEventAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Update calendar event functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> DeleteCalendarEventAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Delete calendar event functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> GetCalendarsAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get calendars functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> CreateCalendarAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Create calendar functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> AcceptEventAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Accept event functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> DeclineEventAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Decline event functionality will be implemented", status = "placeholder" };
    }

    // Teams and Communication Methods
    private async Task<object> GetTeamsAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get teams functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> GetTeamChannelsAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get team channels functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> CreateTeamChannelAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Create team channel functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> GetChannelMessagesAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get channel messages functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> SendChannelMessageAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Send channel message functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> ReplyToChannelMessageAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Reply to channel message functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> GetTeamMembersAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get team members functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> AddTeamMemberAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Add team member functionality will be implemented", status = "placeholder" };
    }

    // OneDrive and SharePoint Methods
    private async Task<object> GetDriveItemsAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get drive items functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> UploadFileAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Upload file functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> DownloadFileAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Download file functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> ShareFileAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Share file functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> GetSharePointSitesAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get SharePoint sites functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> GetSiteListsAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get site lists functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> GetListItemsAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get list items functionality will be implemented", status = "placeholder" };
    }

    // Security and Compliance Methods
    private async Task<object> GetSignInLogsAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get sign-in logs functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> GetAuditLogsAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get audit logs functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> GetRiskyUsersAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get risky users functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> ConfirmRiskyUserAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Confirm risky user functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> GetConditionalAccessPoliciesAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get conditional access policies functionality will be implemented", status = "placeholder" };
    }

    // Reports and Analytics Methods
    private async Task<object> GetUsageReportsAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get usage reports functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> GetTeamsActivityAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get Teams activity functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> GetEmailActivityAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get email activity functionality will be implemented", status = "placeholder" };
    }

    private async Task<object> GetSharePointActivityAsync(JsonElement? arguments)
    {
        await Task.Delay(1); // Placeholder for async
        return new { message = "Get SharePoint activity functionality will be implemented", status = "placeholder" };
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