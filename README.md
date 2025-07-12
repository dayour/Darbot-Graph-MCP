# ⭐ Darbot Graph MCP Server

Easily install the Darbot Graph MCP Server for VS Code or VS Code Insiders:

[![Install with NPX in VS Code](https://img.shields.io/badge/VS_Code-Install_Darbot_Graph_MCP-0098FF?style=flat-square&logo=visualstudiocode&logoColor=white)](https://insiders.vscode.dev/redirect/mcp/install?name=darbot-graph&config=%7B%22type%22%3A%22stdio%22%2C%22command%22%3A%22npx%22%2C%22args%22%3A%5B%22-y%22%2C%22%40darbotlabs%2Fdarbot-graph-mcp%22%2C%22%24%7Binput%3Aazure_tenant_id%7D%22%2C%22%24%7Binput%3Aazure_client_id%7D%22%2C%22%24%7Binput%3Aazure_client_secret%7D%22%5D%7D&inputs=%5B%7B%22id%22%3A%22azure_tenant_id%22%2C%22type%22%3A%22promptString%22%2C%22description%22%3A%22Azure%20AD%20Tenant%20ID%20(leave%20empty%20for%20demo%20mode)%22%7D%2C%7B%22id%22%3A%22azure_client_id%22%2C%22type%22%3A%22promptString%22%2C%22description%22%3A%22Azure%20AD%20Client%20ID%20(leave%20empty%20for%20demo%20mode)%22%7D%2C%7B%22id%22%3A%22azure_client_secret%22%2C%22type%22%3A%22promptString%22%2C%22description%22%3A%22Azure%20AD%20Client%20Secret%20(leave%20empty%20for%20demo%20mode)%22%7D%5D)
[![Install with NPX in VS Code Insiders](https://img.shields.io/badge/VS_Code_Insiders-Install_Darbot_Graph_MCP-24bfa5?style=flat-square&logo=visualstudiocode&logoColor=white)](https://insiders.vscode.dev/redirect/mcp/install?name=darbot-graph&quality=insiders&config=%7B%22type%22%3A%22stdio%22%2C%22command%22%3A%22npx%22%2C%22args%22%3A%5B%22-y%22%2C%22%40darbotlabs%2Fdarbot-graph-mcp%22%2C%22%24%7Binput%3Aazure_tenant_id%7D%22%2C%22%24%7Binput%3Aazure_client_id%7D%22%2C%22%24%7Binput%3Aazure_client_secret%7D%22%5D%7D&inputs=%5B%7B%22id%22%3A%22azure_tenant_id%22%2C%22type%22%3A%22promptString%22%2C%22description%22%3A%22Azure%20AD%20Tenant%20ID%20(leave%20empty%20for%20demo%20mode)%22%7D%2C%7B%22id%22%3A%22azure_client_id%22%2C%22type%22%3A%22promptString%22%2C%22description%22%3A%22Azure%20AD%20Client%20ID%20(leave%20empty%20for%20demo%20mode)%22%7D%2C%7B%22id%22%3A%22azure_client_secret%22%2C%22type%22%3A%22promptString%22%2C%22description%22%3A%22Azure%20AD%20Client%20Secret%20(leave%20empty%20for%20demo%20mode)%22%7D%5D)

The **ultimate extensible MCP server for any and all Microsoft Graph API needs**. A comprehensive, production-ready Model Context Protocol (MCP) server that provides extensive Microsoft Graph operations for Claude and other MCP-compatible AI assistants. Features hierarchical tool organization optimized for Visual Studio Code's tool limits.

## Overview

The Darbot Graph MCP Server is an enterprise-grade solution that bridges AI assistants with Microsoft 365 services through the Microsoft Graph API. Built on the official Microsoft Graph SDKs (both v1.0 and Beta), it provides 64+ comprehensive tools organized into 10 logical categories, enabling seamless automation and management of your Microsoft 365 environment.

This server is designed to be the definitive Microsoft Graph integration for MCP, covering all major Graph API capabilities and designed for extensibility to support future Graph API enhancements.

### Key Features

- **64+ Comprehensive Tools**: Complete coverage of Microsoft Graph API operations across all major Microsoft 365 services
- **Official SDK Foundation**: Built on [Microsoft.Graph SDK](https://github.com/microsoftgraph/msgraph-sdk-dotnet) and [Microsoft.Graph.Beta SDK](https://github.com/microsoftgraph/msgraph-beta-sdk-dotnet)
- **Hierarchical Organization**: Optimized for Visual Studio Code's 128 tool limit with logical categorization
- **Production Ready**: Enterprise-grade error handling, logging, and security
- **Dual SDK Support**: Microsoft Graph v1.0 and Beta API support for maximum compatibility
- **Demo Mode**: Safe testing without affecting production data
- **Enhanced Authentication**: Support for app-only and delegated permissions with Azure.Identity
- **Ultimate Extensibility**: Designed to be the definitive Microsoft Graph MCP server with modular architecture for easy expansion

## Installation

### 🚀 One-Click Installation (Recommended)

The fastest way to get started with Darbot Graph MCP Server:

1. **Click the install button above** for your VS Code version
2. **Enter your Azure AD credentials** when prompted (or leave empty for demo mode)
3. **Start using Microsoft Graph tools** immediately in VS Code

The one-click installation will:
- ✅ Automatically download and configure the MCP server
- ✅ Build the .NET application if needed
- ✅ Set up the MCP client configuration
- ✅ Handle Azure AD authentication setup

### 📋 Prerequisites

- **VS Code or VS Code Insiders** with MCP support
- **.NET 8.0 SDK** or later ([Download here](https://dotnet.microsoft.com/download))
- **Azure subscription** with Azure Active Directory tenant (optional - for production use)
- **Node.js 16+** (for NPX installation)

### 🔧 Manual Installation

If you prefer manual setup or need custom configuration:

#### Step 1: Clone Repository
```bash
git clone https://github.com/dayour/darbot-graph-mcp.git
cd darbot-graph-mcp
```

#### Step 2: Build Server
```bash
dotnet restore
dotnet build
```

#### Step 3: Configure VS Code MCP
Add to your VS Code MCP configuration:

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

#### Step 4: Restart VS Code
Completely close and restart VS Code to load the new MCP server.

### 🎯 Quick Start

Once installed, the server provides 64+ Microsoft Graph tools. Here are some examples:

**List Users:**
```json
{
  "name": "darbot-graph-users-list",
  "arguments": {"top": 10, "filter": "department eq 'IT'"}
}
```

**Send Email:**
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

**Create Calendar Event:**
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

The server automatically runs on `http://localhost:5000` and provides:
- **64+ Microsoft Graph tools** organized in 10 categories
- **Demo mode** for testing without Azure AD setup
- **Production mode** with full Microsoft 365 integration

## Tool Categories (64+ Tools)

The tools are organized hierarchically using the pattern `darbot-graph-{category}-{action}`:

| Category | Tools | Coverage | Examples |
|----------|--------|----------|-----------|
| **User Management** | 8 | Complete user lifecycle & directory operations | `darbot-graph-users-list`, `darbot-graph-users-create` |
| **Group Management** | 8 | Security & distribution groups, dynamic membership | `darbot-graph-groups-list`, `darbot-graph-groups-members-add` |
| **Email Management** | 8 | Mail sending, folder management, message operations | `darbot-graph-mail-send`, `darbot-graph-mail-messages-list` |
| **Calendar Management** | 8 | Events, calendars, scheduling, responses | `darbot-graph-calendar-events-create`, `darbot-graph-calendar-list` |
| **Teams Management** | 8 | Teams, channels, messages, membership | `darbot-graph-teams-list`, `darbot-graph-teams-messages-send` |
| **Files Management** | 4 | OneDrive, SharePoint files, sharing | `darbot-graph-files-upload`, `darbot-graph-files-share` |
| **SharePoint** | 3 | Sites, lists, content management | `darbot-graph-sharepoint-sites-list`, `darbot-graph-sharepoint-items-list` |
| **Security** | 5 | Risk detection, audit logs, sign-ins | `darbot-graph-security-risks-list`, `darbot-graph-security-audit-list` |
| **Reports** | 4 | Usage analytics, activity reports | `darbot-graph-reports-usage`, `darbot-graph-reports-teams` |
| **Applications** | 8 | App registrations, permissions, service principals | `darbot-graph-apps-list`, `darbot-graph-apps-permissions-grant` |

### Microsoft Graph API Coverage

Based on the [Microsoft Graph SDK](https://github.com/microsoftgraph/msgraph-sdk-dotnet) and [PowerShell Graph modules](https://learn.microsoft.com/en-us/powershell/microsoftgraph/get-started), this server provides comprehensive coverage of:

#### Core Services ✅
- **Users & Groups**: Complete identity management
- **Mail**: Exchange Online integration  
- **Calendar**: Outlook calendar operations
- **Teams**: Microsoft Teams collaboration
- **Files**: OneDrive & SharePoint document management
- **Applications**: Azure AD app management

#### Security & Compliance ✅
- **Identity Protection**: Risk assessment and policies
- **Audit Logs**: Security event monitoring
- **Security Reports**: Threat intelligence

#### Extensible Architecture 🚀
The modular design supports easy addition of:
- **Device Management** (Intune APIs)
- **Compliance** (DLP, retention policies)
- **External Identities** (B2B/B2C)
- **Education** (EDU-specific APIs)
- **Search** (Microsoft Search)
- **Cloud Communications** (Calls, presence)
- **Bookings** (Microsoft Bookings)
- **Planner** (Task management)
- **OneNote** (Note-taking APIs)

## Documentation

- **[Complete Command Reference (cmd_lib.md)](./cmd_lib.md)** - Detailed documentation of all 64+ tools with parameters and examples
- **[Azure AD Setup Guide](#azure-ad-setup)** - Step-by-step configuration instructions
- **[Claude Desktop Integration](#claude-desktop-integration)** - MCP client setup
- **[Microsoft Graph Resources](#microsoft-graph-resources)** - Official SDK and documentation links
- **[Extensibility Guide](#extensibility)** - How to add new Graph API capabilities

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

### Using NPM Package (Recommended)
```json
{
  "mcpServers": {
    "darbot-graph": {
      "command": "npx",
      "args": [
        "-y", 
        "@darbotlabs/darbot-graph-mcp",
        "your-tenant-id",
        "your-client-id", 
        "your-client-secret"
      ]
    }
  }
}
```

### Using Direct .NET Command
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

### Configuration File Locations
- **Windows**: `%APPDATA%\Claude\claude_desktop_config.json`
- **macOS**: `~/Library/Application Support/Claude/claude_desktop_config.json`
- **Linux**: `~/.config/Claude/claude_desktop_config.json`

### Demo Mode
Leave the Azure AD credentials empty to run in demo mode:
```json
{
  "mcpServers": {
    "darbot-graph": {
      "command": "npx",
      "args": ["-y", "@darbotlabs/darbot-graph-mcp"]
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

## Microsoft Graph Resources

This server is built on the official Microsoft Graph SDKs and follows Microsoft Graph best practices:

### Official SDKs
- **[Microsoft Graph .NET SDK](https://github.com/microsoftgraph/msgraph-sdk-dotnet)** - v1.0 API support
- **[Microsoft Graph .NET Beta SDK](https://github.com/microsoftgraph/msgraph-beta-sdk-dotnet)** - Preview API support
- **[Microsoft Graph PowerShell](https://learn.microsoft.com/en-us/powershell/microsoftgraph/get-started)** - Reference for comprehensive API coverage

### Key Components Used
- **Microsoft.Graph**: Production-ready v1.0 APIs
- **Microsoft.Graph.Beta**: Preview APIs for latest features
- **Microsoft.Graph.Applications**: Application management capabilities
- **Microsoft.Graph.Authentication**: Azure Identity integration

### API Reference
- **[Graph Explorer](https://developer.microsoft.com/en-us/graph/graph-explorer)** - Interactive API testing
- **[Graph API Documentation](https://docs.microsoft.com/en-us/graph/api/overview)** - Complete API reference
- **[Graph Permissions Reference](https://docs.microsoft.com/en-us/graph/permissions-reference)** - Required permissions for each API

## Extensibility

The Darbot Graph MCP Server is designed for maximum extensibility to accommodate the full breadth of Microsoft Graph APIs:

### Adding New Tool Categories

1. **Extend ToolCategories.cs**:
```csharp
public static List<object> GetNewCategoryTools()
{
    return new List<object>
    {
        new
        {
            name = "darbot-graph-newcategory-action",
            description = "Description of the new tool",
            inputSchema = new { /* schema definition */ }
        }
    };
}
```

2. **Implement in GraphServiceEnhanced.cs**:
```csharp
"darbot-graph-newcategory-action" => await NewCategoryActionAsync(arguments),
```

3. **Add to GetAvailableTools()**:
```csharp
tools.AddRange(ToolCategories.GetNewCategoryTools());
```

### Supported Graph API Areas for Extension

The current architecture supports adding tools for any Microsoft Graph API endpoint:

#### Ready for Implementation 🔄
- **Device Management**: Intune device operations, compliance policies
- **Identity Protection**: Conditional access, risk policies, named locations
- **Directory Management**: Administrative units, organizational contacts
- **Contacts**: Personal and organization contact management
- **Tasks/Planner**: Microsoft Planner integration for task management
- **OneNote**: Notebook, section, and page operations
- **Licenses**: Subscription and license assignment management
- **External Identities**: B2B collaboration and B2C management
- **Bookings**: Microsoft Bookings appointment and business management
- **Education**: Education-specific classes, assignments, and roster management
- **Compliance**: Data loss prevention, retention policies, eDiscovery
- **Search**: Microsoft Search query and administration
- **Cloud Communications**: Teams calls, meetings, and presence
- **Print**: Universal Print service management

#### Implementation Pattern
Each new category follows the established pattern:
```
darbot-graph-{category}-{action}
```

Examples of potential future tools:
- `darbot-graph-devices-list` - List managed devices
- `darbot-graph-planner-tasks-create` - Create Planner tasks
- `darbot-graph-onenote-pages-create` - Create OneNote pages
- `darbot-graph-compliance-policies-list` - List compliance policies

### Contributing New Tools

1. **Identify Graph API**: Choose from [Microsoft Graph API reference](https://docs.microsoft.com/en-us/graph/api/overview)
2. **Design Tool Schema**: Define input parameters and validation
3. **Implement Service Method**: Add async method in GraphServiceEnhanced
4. **Add Tool Definition**: Include in appropriate ToolCategories method
5. **Test & Document**: Validate functionality and update cmd_lib.md

### SDK Version Management

The server maintains compatibility with both stable and preview APIs:
- **Stable Operations**: Use `_graphClient` (Microsoft.Graph)
- **Preview Features**: Use `_betaGraphClient` (Microsoft.Graph.Beta)

This ensures access to the latest Graph capabilities while maintaining production stability.

## Validation & Quality Assurance

### 🔍 End-to-End Validation Audit

The Darbot Graph MCP Server includes a comprehensive validation audit script that ensures all components are functioning correctly:

```bash
# Run the complete validation audit
./scripts/validate.sh
```

**The audit validates:**
- ✅ **Environment Prerequisites**: .NET 8.0 SDK, Node.js availability
- ✅ **Build Process**: Dependency restoration, compilation success
- ✅ **Server Startup**: Process launch, port binding, health checks
- ✅ **API Endpoints**: Health, tools listing, tool execution
- ✅ **Tool Functionality**: All 64 Microsoft Graph tools available and working
- ✅ **NPM Wrapper**: Installation infrastructure, server detection
- ✅ **Configuration**: JSON validity, required settings
- ✅ **Performance**: Response time validation
- ✅ **Demo Mode**: Safe operation without Azure AD credentials

### 🛡️ Robustness Features

- **Graceful Degradation**: Automatically switches to demo mode when Azure AD is unavailable
- **Error Handling**: Comprehensive error handling and logging throughout
- **Extensibility**: Modular architecture supports easy addition of new Graph API tools
- **Production Ready**: Enterprise-grade configuration management and security practices

### 📊 Audit Results Summary

Recent validation audit confirms:
- **64 Microsoft Graph tools** across 10 categories working correctly
- **Sub-10ms response times** for health checks
- **100% API endpoint availability** in testing
- **Demo mode functionality** verified for safe development
- **NPM wrapper infrastructure** ready for one-click installation

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes with tests
4. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
