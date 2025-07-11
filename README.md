# Darbot Graph MCP Server

A comprehensive, production-ready Model Context Protocol (MCP) server that provides extensive Microsoft Graph operations for Claude and other MCP-compatible AI assistants. Features hierarchical tool organization optimized for Visual Studio Code's tool limits.

## Overview

The Darbot Graph MCP Server is an enterprise-grade solution that bridges AI assistants with Microsoft 365 services through the Microsoft Graph API. It provides 64 comprehensive tools organized into 10 logical categories, enabling seamless automation and management of your Microsoft 365 environment.

### Key Features

- **64 Comprehensive Tools**: Complete coverage of Microsoft Graph API operations
- **Hierarchical Organization**: Optimized for Visual Studio Code's 128 tool limit
- **Production Ready**: Enterprise-grade error handling, logging, and security
- **Dual SDK Support**: Microsoft Graph v1.0 and Beta API support
- **Demo Mode**: Safe testing without affecting production data
- **Enhanced Authentication**: Support for app-only and delegated permissions

## Quick Start

### Prerequisites

- .NET 8.0 SDK or later
- Azure subscription with Azure Active Directory tenant
- Global Administrator permissions (for initial app registration)
- Claude Desktop or compatible MCP client

### Installation

```bash
# Clone the repository
git clone https://github.com/dayour/darbot-graph-mcp.git
cd darbot-graph-mcp

# Restore dependencies and build
dotnet restore
dotnet build

# Run the server
dotnet run --project src/DarbotGraphMcp.Server
```

The server will start on `http://localhost:5000` with 64 Microsoft Graph tools available.

## Tool Categories (64 Total)

The tools are organized hierarchically using the pattern `darbot-graph-{category}-{action}`:

| Category | Tools | Examples |
|----------|--------|----------|
| **User Management** | 8 | `darbot-graph-users-list`, `darbot-graph-users-create` |
| **Group Management** | 8 | `darbot-graph-groups-list`, `darbot-graph-groups-members-add` |
| **Email Management** | 8 | `darbot-graph-mail-send`, `darbot-graph-mail-messages-list` |
| **Calendar Management** | 8 | `darbot-graph-calendar-events-create`, `darbot-graph-calendar-list` |
| **Teams Management** | 8 | `darbot-graph-teams-list`, `darbot-graph-teams-messages-send` |
| **Files Management** | 4 | `darbot-graph-files-upload`, `darbot-graph-files-share` |
| **SharePoint** | 3 | `darbot-graph-sharepoint-sites-list`, `darbot-graph-sharepoint-items-list` |
| **Security** | 5 | `darbot-graph-security-risks-list`, `darbot-graph-security-audit-list` |
| **Reports** | 4 | `darbot-graph-reports-usage`, `darbot-graph-reports-teams` |
| **Applications** | 8 | `darbot-graph-apps-list`, `darbot-graph-apps-permissions-grant` |

## Documentation

- **[Complete Command Reference (cmd_lib.md)](./cmd_lib.md)** - Detailed documentation of all 64 tools with parameters and examples
- **[Azure AD Setup Guide](#azure-ad-setup)** - Step-by-step configuration instructions
- **[Claude Desktop Integration](#claude-desktop-integration)** - MCP client setup

## Architecture

### Enhanced Service Layer

```
Claude Desktop / MCP Client
         ↓
    Darbot Graph MCP Server
         ↓
┌─────────────────────────┐
│   GraphServiceEnhanced  │ ← Enhanced implementation
├─────────────────────────┤
│     ToolCategories      │ ← Hierarchical organization  
├─────────────────────────┤
│  Microsoft.Graph SDK    │ ← v1.0 API support
│  Microsoft.Graph.Beta   │ ← Beta API support
└─────────────────────────┘
         ↓
    Microsoft Graph API
         ↓
   Microsoft 365 Services
```

### Key Components

- **GraphServiceEnhanced**: Production-ready service with comprehensive error handling
- **ToolCategories**: Hierarchical tool organization for VS Code compatibility
- **Dual SDK Support**: Access to both stable and preview Graph APIs
- **Enhanced Authentication**: Robust credential management and demo mode

## Azure AD Setup

### Quick Setup

1. **Register Application** in Azure Portal
2. **Configure Permissions**: Grant required Microsoft Graph permissions
3. **Create Client Secret**: Generate and secure application credentials
4. **Update Configuration**: Add credentials to `appsettings.json`

**Required Permissions:**
```
User.ReadWrite.All, Group.ReadWrite.All, Mail.ReadWrite, Mail.Send,
Calendars.ReadWrite, Team.ReadBasic.All, Files.ReadWrite.All,
Sites.ReadWrite.All, Reports.Read.All, Application.ReadWrite.All
```

**Configuration:**
```json
{
  "AzureAd": {
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret"
  }
}
```

For detailed setup instructions, see the [Azure AD Setup Guide](#detailed-azure-ad-setup-guide) section below.

## Claude Desktop Integration

Add to your Claude Desktop configuration:

```json
{
  "mcpServers": {
    "darbot-graph": {
      "command": "dotnet",
      "args": [
        "run", 
        "--project", 
        "/path/to/darbot-graph-mcp/src/DarbotGraphMcp.Server"
      ],
      "env": {
        "AzureAd__TenantId": "your-tenant-id",
        "AzureAd__ClientId": "your-client-id",
        "AzureAd__ClientSecret": "your-client-secret"
      }
    }
  }
}
```

## Usage Examples

### List Users
```json
{
  "name": "darbot-graph-users-list",
  "arguments": {"top": 10, "filter": "department eq 'IT'"}
}
```

### Send Email
```json
{
  "name": "darbot-graph-mail-send",
  "arguments": {
    "to": ["user@company.com"],
    "subject": "Automated Report",
    "body": "Your report is ready.",
    "bodyType": "Text"
  }
}
```

### Create Calendar Event
```json
{
  "name": "darbot-graph-calendar-events-create",
  "arguments": {
    "userId": "manager@company.com",
    "subject": "Weekly Standup",
    "startTime": "2024-01-15T09:00:00",
    "endTime": "2024-01-15T10:00:00"
  }
}
```

## Operating Modes

### Production Mode
With Azure AD credentials configured:
- Real-time data from your Microsoft 365 tenant
- Complete CRUD operations for users, groups, and content
- Advanced security and compliance monitoring
- Full workflow automation capabilities

### Demo Mode
Without Azure AD credentials (safe testing):
- Sample data responses for all tools
- Schema validation and tool discovery
- Safe development environment
- No production data access

## API Endpoints

- `GET /health` - Server health check
- `GET /tools` - List all 64 available MCP tools
- `POST /call-tool` - Execute a specific tool
- `POST /sse` - Server-Sent Events for MCP communication

---

## Detailed Azure AD Setup Guide

### Step 1: Azure AD App Registration

#### 1.1 Create App Registration
1. Sign in to the [Azure Portal](https://portal.azure.com)
2. Navigate to **Azure Active Directory** > **App registrations**
3. Click **New registration**
4. Configure the application:
   - **Name**: `Darbot Graph MCP Server`
   - **Supported account types**: `Accounts in this organizational directory only (Single tenant)`
   - **Redirect URI**: Leave blank (not needed for app-only authentication)
5. Click **Register**

#### 1.2 Record Application Details
After registration, record these values from the **Overview** page:
- **Application (client) ID**: `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`
- **Directory (tenant) ID**: `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`

### Step 2: Create Client Secret

#### 2.1 Generate Application Secret
1. In your app registration, go to **Certificates & secrets**
2. Under **Client secrets**, click **New client secret**
3. Configure the secret:
   - **Description**: `Darbot MCP Server Secret`
   - **Expires**: Choose appropriate expiration (recommended: 24 months)
4. Click **Add**
5. **IMPORTANT**: Copy the secret **Value** immediately (it won't be shown again)

### Step 3: Configure API Permissions

#### 3.1 Add Microsoft Graph Permissions
1. Go to **API permissions** in your app registration
2. Click **Add a permission** > **Microsoft Graph** > **Application permissions**
3. Add these permissions:

**Core Permissions:**
- `User.ReadWrite.All` - Read and write all users' full profiles
- `Group.ReadWrite.All` - Read and write all groups
- `Directory.ReadWrite.All` - Read and write directory data
- `Mail.ReadWrite` - Read and write access to user mail
- `Mail.Send` - Send mail as any user
- `Calendars.ReadWrite` - Read and write calendars

**Extended Permissions:**
- `Team.ReadBasic.All` - Read the basic properties of teams
- `TeamSettings.ReadWrite.All` - Read and write teams' settings
- `Files.ReadWrite.All` - Read and write files in all site collections
- `Sites.ReadWrite.All` - Read and write items in all site collections
- `Reports.Read.All` - Read usage reports
- `Application.ReadWrite.All` - Read and write applications
- `AuditLog.Read.All` - Read audit log data
- `SecurityEvents.Read.All` - Read security events

#### 3.2 Grant Admin Consent
1. After adding all permissions, click **Grant admin consent for [Your Organization]**
2. Click **Yes** to confirm
3. Verify all permissions show **Granted for [Your Organization]** with green checkmarks

### Step 4: Application Configuration

#### 4.1 Configure appsettings.json
Create or update `src/DarbotGraphMcp.Server/appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "AzureAd": {
    "TenantId": "your-tenant-id-here",
    "ClientId": "your-client-id-here", 
    "ClientSecret": "your-client-secret-here"
  }
}
```

#### 4.2 Environment Variables (Production Alternative)
For production environments, use environment variables:

```bash
# Linux/macOS
export AzureAd__TenantId="your-tenant-id"
export AzureAd__ClientId="your-client-id"
export AzureAd__ClientSecret="your-client-secret"

# Windows PowerShell
$env:AzureAd__TenantId="your-tenant-id"
$env:AzureAd__ClientId="your-client-id"
$env:AzureAd__ClientSecret="your-client-secret"
```

### Step 5: Validation

#### 5.1 Test Health Endpoint
```bash
curl http://localhost:5000/health
# Expected: "Darbot Graph MCP Server - Enhanced"
```

#### 5.2 Test Tool Count
```bash
curl http://localhost:5000/tools | jq length
# Expected: 64
```

#### 5.3 Test Tool Execution
```bash
curl -X POST http://localhost:5000/call-tool \
  -H "Content-Type: application/json" \
  -d '{"name": "darbot-graph-users-list", "arguments": {"top": 2}}' | jq
```

Expected successful response includes real user data from your tenant.

## Claude Desktop Integration

### Configuration File Locations
- **Windows**: `%APPDATA%\Claude\claude_desktop_config.json`
- **macOS**: `~/Library/Application Support/Claude/claude_desktop_config.json`
- **Linux**: `~/.config/Claude/claude_desktop_config.json`

### MCP Server Configuration
```json
{
  "mcpServers": {
    "darbot-graph": {
      "command": "dotnet",
      "args": [
        "run", 
        "--project", 
        "/absolute/path/to/darbot-graph-mcp/src/DarbotGraphMcp.Server"
      ],
      "env": {
        "AzureAd__TenantId": "your-tenant-id",
        "AzureAd__ClientId": "your-client-id",
        "AzureAd__ClientSecret": "your-client-secret"
      }
    }
  }
}
```

### Restart Process
1. Completely close Claude Desktop
2. Restart the application
3. Verify the Darbot Graph tools appear in Claude's tool list

## Production Considerations

### Security Best Practices
- Store secrets in Azure Key Vault for production
- Implement credential rotation policies
- Use least-privilege permissions
- Monitor API usage and audit logs

### Performance Optimization
- Implement response caching for frequently accessed data
- Use pagination for large result sets
- Configure appropriate timeout values
- Monitor Graph API rate limits

### Monitoring and Logging
- Enable Application Insights for production monitoring
- Implement structured logging
- Set up health checks and alerts
- Track tool usage metrics

## Troubleshooting

### Common Issues

**Authentication Failures**
- Verify Azure AD app registration settings
- Check client secret hasn't expired
- Confirm admin consent has been granted

**Permission Denied Errors**
- Review required API permissions
- Ensure admin consent is granted
- Check scope limitations

**Tool Discovery Issues**
- Restart Claude Desktop completely
- Verify MCP server configuration path
- Check server is running on correct port

## Support and Resources

- **Command Reference**: [cmd_lib.md](./cmd_lib.md)
- **Microsoft Graph Documentation**: [docs.microsoft.com/graph](https://docs.microsoft.com/en-us/graph/)
- **MCP Protocol**: [modelcontextprotocol.io](https://modelcontextprotocol.io/)
- **Azure AD Documentation**: [docs.microsoft.com/azure/active-directory](https://docs.microsoft.com/en-us/azure/active-directory/)

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes with tests
4. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
