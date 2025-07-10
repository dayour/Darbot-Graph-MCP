--- 
name: .NET OpenAI MCP Agent
description: This is an MCP agent app written in .NET, using OpenAI, with a remote MCP server written in TypeScript.
languages:
- csharp
- bicep
- azdeveloper
products:
- azure-openai
- azure-container-apps
- azure
page_type: sample
urlFragment: openai-mcp-agent-dotnet
--- 

# Microsoft Graph MCP Server

A Model Context Protocol (MCP) server that provides Microsoft Graph and Power Platform Administration CLI (PAC CLI) tools for Claude and other MCP-compatible AI assistants.

## Features

### Microsoft Graph Tools

This MCP server exposes the following Microsoft Graph operations:

#### User Management
- **get_users**: Get a list of users with optional filtering and search
- **get_user**: Get detailed information about a specific user
- **create_user**: Create a new user account

#### Group Management  
- **get_groups**: Get a list of groups with optional filtering
- **get_group**: Get detailed information about a specific group
- **create_group**: Create a new security or Microsoft 365 group

#### Email Operations
- **send_mail**: Send emails to one or more recipients

#### Calendar Management
- **get_calendar_events**: Retrieve calendar events for a date range
- **create_calendar_event**: Create new calendar events with attendees

#### Teams Integration
- **get_teams**: Get Microsoft Teams the user is a member of

### Power Platform CLI (PAC CLI) Tools (Planned)

The following PAC CLI tools will be integrated:

#### Environment Management
- **pac_list_environments**: List Power Platform environments
- **pac_create_environment**: Create new environments
- **pac_select_environment**: Switch active environment

#### Solution Management
- **pac_list_solutions**: List solutions in an environment
- **pac_export_solution**: Export solutions
- **pac_import_solution**: Import solutions

#### App Management
- **pac_list_apps**: List Power Apps
- **pac_publish_app**: Publish Power Apps

#### Flow Management
- **pac_list_flows**: List Power Automate flows
- **pac_enable_flow**: Enable/disable flows

## Prerequisites

### For Microsoft Graph
- Azure AD App Registration with appropriate Microsoft Graph permissions
- Client ID, Client Secret, and Tenant ID
- Required Graph API permissions:
  - User.Read.All (for reading users)
  - User.ReadWrite.All (for creating users)
  - Group.Read.All (for reading groups)
  - Group.ReadWrite.All (for creating groups)
  - Mail.Send (for sending emails)
  - Calendars.ReadWrite (for calendar operations)
  - Team.ReadBasic.All (for Teams access)

### For Power Platform CLI (Planned)
- Power Platform CLI installed
- Appropriate Power Platform licenses and permissions
- Authentication configured for target environments

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
git clone https://github.com/dayour/dotnet-graph-mcp.git
cd dotnet-graph-mcp
```

### 2. Configure Authentication

Update `src/GraphMcp.Server/appsettings.Development.json` with your Azure AD app registration details.

### 3. Build and Run the Graph MCP Server

```bash
dotnet build src/GraphMcp.Server/GraphMcp.Server.csproj
dotnet run --project src/GraphMcp.Server/GraphMcp.Server.csproj
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
  -d '{"name": "get_users", "arguments": {"top": 5}}'
```

### 5. Connect to Claude Desktop

Add the following to your Claude Desktop MCP settings:

```json
{
  "mcpServers": {
    "microsoft-graph": {
      "command": "dotnet",
      "args": ["run", "--project", "path/to/GraphMcp.Server.csproj"]
    }
  }
}
```

## Architecture

### Graph MCP Server (`src/GraphMcp.Server/`)
- ASP.NET Core web application
- Microsoft Graph SDK integration
- RESTful API endpoints for MCP protocol
- Server-Sent Events (SSE) for real-time communication

### Client App (`src/McpTodo.ClientApp/`) 
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
  "name": "get_users",
  "description": "Get a list of users from Microsoft Graph",
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
- [x] User management tools
- [x] Group management tools  
- [x] Email operations
- [x] Basic MCP server structure

### Phase 2: Extended Graph Features (In Progress)
- [ ] Calendar and event management
- [ ] Teams integration
- [ ] SharePoint operations
- [ ] Advanced filtering and search

### Phase 3: Power Platform CLI Integration (Planned)
- [ ] Environment management
- [ ] Solution lifecycle operations
- [ ] App and flow management
- [ ] Security and compliance tools

### Phase 4: Production Features (Planned)
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
