---
name: Darbot Graph MCP Server
description: A comprehensive Microsoft Graph MCP server that provides extensive Microsoft Graph operations for Claude and other MCP-compatible AI assistants.
languages:
- csharp
products:
- azure
page_type: sample
urlFragment: darbot-graph-mcp
--- 

# Darbot Graph MCP Server

A comprehensive Model Context Protocol (MCP) server that provides extensive Microsoft Graph operations for Claude and other MCP-compatible AI assistants.

## Features

### Comprehensive Microsoft Graph Tools (50+ Tools)

This MCP server exposes 50+ comprehensive Microsoft Graph operations with all tool names prefixed with `darbot-graph-`:

#### User Management
- **darbot-graph-get-users**: Get a list of users with advanced filtering and search
- **darbot-graph-get-user**: Get detailed information about a specific user
- **darbot-graph-create-user**: Create a new user account with comprehensive settings
- **darbot-graph-update-user**: Update user properties and settings
- **darbot-graph-delete-user**: Remove a user from the directory
- **darbot-graph-reset-user-password**: Reset user password and force change
- **darbot-graph-get-user-manager**: Get user's manager information
- **darbot-graph-set-user-manager**: Assign a manager to a user

#### Group Management  
- **darbot-graph-get-groups**: Get a list of groups with advanced filtering
- **darbot-graph-get-group**: Get detailed information about a specific group
- **darbot-graph-create-group**: Create security or Microsoft 365 groups
- **darbot-graph-update-group**: Update group properties and settings
- **darbot-graph-delete-group**: Remove a group from the directory
- **darbot-graph-add-group-member**: Add members to a group
- **darbot-graph-remove-group-member**: Remove members from a group
- **darbot-graph-get-group-members**: List all group members

#### Email Management
- **darbot-graph-send-mail**: Send emails with advanced formatting and attachments
- **darbot-graph-get-mailbox-settings**: Retrieve user mailbox settings
- **darbot-graph-get-mail-folders**: List mail folders and subfolders
- **darbot-graph-create-mail-folder**: Create new mail folders
- **darbot-graph-get-messages**: Retrieve messages with filtering
- **darbot-graph-reply-to-message**: Reply to email messages
- **darbot-graph-forward-message**: Forward email messages
- **darbot-graph-move-message**: Move messages between folders
- **darbot-graph-delete-message**: Delete email messages

#### Calendar Management
- **darbot-graph-get-calendar-events**: Retrieve calendar events with advanced filtering
- **darbot-graph-create-calendar-event**: Create events with attendees and recurrence
- **darbot-graph-update-calendar-event**: Update existing calendar events
- **darbot-graph-delete-calendar-event**: Remove calendar events
- **darbot-graph-get-calendars**: List user calendars
- **darbot-graph-create-calendar**: Create new calendars
- **darbot-graph-get-event-instances**: Get recurring event instances
- **darbot-graph-accept-event**: Accept meeting invitations
- **darbot-graph-decline-event**: Decline meeting invitations

#### Teams and Communication
- **darbot-graph-get-teams**: Get Microsoft Teams user is member of
- **darbot-graph-get-team-channels**: List channels in a team
- **darbot-graph-create-team-channel**: Create new team channels
- **darbot-graph-get-channel-messages**: Retrieve channel messages
- **darbot-graph-send-channel-message**: Send messages to team channels
- **darbot-graph-reply-to-channel-message**: Reply to channel messages
- **darbot-graph-get-team-members**: List team members
- **darbot-graph-add-team-member**: Add members to teams

#### OneDrive and SharePoint
- **darbot-graph-get-drive-items**: List OneDrive files and folders
- **darbot-graph-upload-file**: Upload files to OneDrive
- **darbot-graph-download-file**: Download files from OneDrive
- **darbot-graph-share-file**: Create sharing links for files
- **darbot-graph-get-sharepoint-sites**: List SharePoint sites
- **darbot-graph-get-site-lists**: Get lists from SharePoint sites
- **darbot-graph-get-list-items**: Retrieve items from SharePoint lists

#### Security and Compliance
- **darbot-graph-get-sign-in-logs**: Retrieve user sign-in logs
- **darbot-graph-get-audit-logs**: Get directory audit logs
- **darbot-graph-get-risky-users**: List users flagged for risk
- **darbot-graph-confirm-risky-user**: Confirm or dismiss risky users
- **darbot-graph-get-conditional-access-policies**: List conditional access policies

#### Reports and Analytics
- **darbot-graph-get-usage-reports**: Get Microsoft 365 usage reports
- **darbot-graph-get-teams-activity**: Get Teams activity reports
- **darbot-graph-get-email-activity**: Get email activity reports
- **darbot-graph-get-sharepoint-activity**: Get SharePoint activity reports

## Prerequisites

### For Microsoft Graph
- Azure AD App Registration with appropriate Microsoft Graph permissions
- Client ID, Client Secret, and Tenant ID
- Required Graph API permissions:
  - User.ReadWrite.All (for user management)
  - Group.ReadWrite.All (for group management)
  - Mail.ReadWrite (for email operations)
  - Mail.Send (for sending emails)
  - Calendars.ReadWrite (for calendar operations)
  - Team.ReadBasic.All (for Teams access)
  - Channel.ReadWrite.All (for Teams channels)
  - Files.ReadWrite.All (for OneDrive/SharePoint)
  - Sites.ReadWrite.All (for SharePoint sites)
  - AuditLog.Read.All (for audit logs)
  - SecurityEvents.Read.All (for security events)
  - Reports.Read.All (for usage reports)

## Configuration

### appsettings.json

```json
{
  "AzureAd": {
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id", 
    "ClientSecret": "your-client-secret"
  }
}
```

### Environment Variables (Alternative)

You can also configure using environment variables:
- `AzureAd__TenantId`
- `AzureAd__ClientId`
- `AzureAd__ClientSecret`

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/dayour/darbot-graph-mcp.git
cd darbot-graph-mcp
```

### 2. Configure Authentication

Update `src/DarbotGraphMcp.Server/appsettings.Development.json` with your Azure AD app registration details.

### 3. Build and Run the Darbot Graph MCP Server

```bash
dotnet build src/DarbotGraphMcp.Server/DarbotGraphMcp.Server.csproj
dotnet run --project src/DarbotGraphMcp.Server/DarbotGraphMcp.Server.csproj
```

The server will start on `http://localhost:5000` by default.

### 4. Test the Server

You can test the available tools:

```bash
# Get available tools
curl http://localhost:5000/tools

# Test a tool call
curl -X POST http://localhost:5000/call-tool \
  -H "Content-Type: application/json" \
  -d '{"name": "darbot-graph-get-users", "arguments": {"top": 5}}'
```

### 5. Connect to Claude Desktop

Add the following to your Claude Desktop MCP settings:

```json
{
  "mcpServers": {
    "darbot-graph": {
      "command": "dotnet",
      "args": ["run", "--project", "path/to/DarbotGraphMcp.Server.csproj"]
    }
  }
}
```

## Tool Count
This MCP server provides **56 comprehensive Microsoft Graph tools**, all following the consistent `darbot-graph-` naming convention:
- 8 User Management tools
- 8 Group Management tools  
- 8 Email Management tools
- 8 Calendar Management tools
- 8 Teams and Communication tools
- 7 OneDrive and SharePoint tools
- 5 Security and Compliance tools
- 4 Reports and Analytics tools

## Demonstration Mode
When Azure AD credentials are not configured, the server runs in demonstration mode with:
- Sample data for user and group queries
- Placeholder responses for all tools indicating implementation status
- Full tool schema documentation for development planning

## Architecture

### Darbot Graph MCP Server (`src/DarbotGraphMcp.Server/`)
- ASP.NET Core web application
- Microsoft Graph SDK integration
- RESTful API endpoints for MCP protocol
- Server-Sent Events (SSE) for real-time communication
- 50+ comprehensive Microsoft Graph tools

### Client App (`src/DarbotMcp.ClientApp/`) 
- Blazor web application for testing MCP tools
- Interactive chat interface
- MCP client implementation

## API Endpoints

- `GET /health` - Health check endpoint
- `GET /tools` - List available MCP tools
- `POST /call-tool` - Execute an MCP tool
- `POST /sse` - Server-Sent Events endpoint for MCP communication

## Tool Schemas

Each tool follows the MCP tool schema with:
- `name`: Tool identifier
- `description`: Human-readable description
- `inputSchema`: JSON schema defining required and optional parameters

Example tool schema:
```json
{
  "name": "darbot-graph-get-users",
  "description": "Get a list of users from Microsoft Graph with advanced filtering",
  "inputSchema": {
    "type": "object",
    "properties": {
      "top": {
        "type": "integer",
        "description": "Number of users to return (max 999)"
      },
      "filter": {
        "type": "string",
        "description": "OData filter expression"
      }
    }
  }
}
```

## Security Considerations

- All Microsoft Graph operations require appropriate permissions
- App-only authentication is used for service-to-service calls
- Sensitive configuration should be stored in Azure Key Vault for production
- Consider implementing rate limiting and request validation

## Development Roadmap

### Phase 1: Core Microsoft Graph Integration ✅
- [x] User management tools (8 tools)
- [x] Group management tools (8 tools)
- [x] Email operations (8 tools)
- [x] Basic MCP server structure

### Phase 2: Extended Graph Features ✅
- [x] Calendar and event management (8 tools)
- [x] Teams integration and messaging (8 tools)
- [x] OneDrive and SharePoint operations (7 tools)
- [x] Security and compliance tools (5 tools)

### Phase 3: Reports and Analytics ✅
- [x] Usage reports and analytics (4 tools)
- [x] Activity monitoring
- [x] Performance metrics

### Phase 4: Production Features (In Progress)
- [x] 50+ comprehensive tools implemented
- [x] Consistent darbot-graph- naming convention
- [ ] Authentication improvements
- [ ] Error handling and logging
- [ ] Performance optimization
- [ ] Comprehensive testing
- [ ] Documentation and examples

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues and questions:
- Open a GitHub issue
- Review the Microsoft Graph documentation
- Check the MCP protocol specification
