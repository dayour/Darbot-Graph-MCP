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

### System Requirements
- .NET 8.0 SDK or later
- Azure subscription with Azure Active Directory tenant
- Global Administrator permissions (for initial app registration)
- Claude Desktop or compatible MCP client

### For Microsoft Graph Integration
- Azure AD App Registration with appropriate Microsoft Graph permissions
- Client ID, Client Secret, and Tenant ID
- Required Graph API permissions (detailed below)

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
   - Record this as your **Client Secret**: `your-secret-value-here`

### Step 3: Configure API Permissions

#### 3.1 Add Microsoft Graph Permissions
1. Go to **API permissions** in your app registration
2. Click **Add a permission** > **Microsoft Graph** > **Application permissions**
3. Add the following permissions (search and select each one):

##### User and Group Management
- `User.ReadWrite.All` - Read and write all users' full profiles
- `Group.ReadWrite.All` - Read and write all groups
- `Directory.ReadWrite.All` - Read and write directory data
- `RoleManagement.ReadWrite.Directory` - Read and write role management data

##### Email and Communication
- `Mail.ReadWrite` - Read and write access to user mail
- `Mail.Send` - Send mail as any user
- `MailboxSettings.ReadWrite` - Read and write user mailbox settings

##### Calendar Management
- `Calendars.ReadWrite` - Read and write calendars
- `Calendars.ReadWrite.Shared` - Read and write shared calendars

##### Teams Integration
- `Team.ReadBasic.All` - Read the basic properties of teams
- `TeamSettings.ReadWrite.All` - Read and write teams' settings
- `Channel.ReadBasic.All` - Read the basic properties of channels
- `ChannelMessage.Read.All` - Read channel messages
- `ChannelMessage.Send` - Send channel messages
- `TeamMember.ReadWrite.All` - Add and remove members from teams

##### Files and SharePoint
- `Files.ReadWrite.All` - Read and write files in all site collections
- `Sites.ReadWrite.All` - Read and write items in all site collections
- `Sites.Manage.All` - Create, edit, and delete SharePoint sites

##### Security and Compliance
- `AuditLog.Read.All` - Read audit log data
- `SecurityEvents.Read.All` - Read security events
- `IdentityRiskyUser.ReadWrite.All` - Read and write risky user information
- `Policy.Read.All` - Read policies

##### Reports and Analytics
- `Reports.Read.All` - Read usage reports
- `Directory.Read.All` - Read directory data

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
For production environments, use environment variables instead:

```bash
# Linux/macOS
export AzureAd__TenantId="your-tenant-id"
export AzureAd__ClientId="your-client-id"
export AzureAd__ClientSecret="your-client-secret"

# Windows PowerShell
$env:AzureAd__TenantId="your-tenant-id"
$env:AzureAd__ClientId="your-client-id"
$env:AzureAd__ClientSecret="your-client-secret"

# Windows Command Prompt
set AzureAd__TenantId=your-tenant-id
set AzureAd__ClientId=your-client-id
set AzureAd__ClientSecret=your-client-secret
```

#### 4.3 Azure Key Vault (Enterprise Production)
For enterprise deployments, store secrets in Azure Key Vault:

```json
{
  "AzureKeyVault": {
    "VaultUri": "https://your-keyvault.vault.azure.net/",
    "ClientId": "your-keyvault-client-id",
    "ClientSecret": "your-keyvault-client-secret",
    "TenantId": "your-tenant-id"
  }
}
```

## Complete Setup and Testing Guide

### Step 1: Clone and Build

```bash
# Clone the repository
git clone https://github.com/dayour/darbot-graph-mcp.git
cd darbot-graph-mcp

# Restore dependencies
dotnet restore

# Build the solution
dotnet build src/DarbotGraphMcp.Server/DarbotGraphMcp.Server.csproj
```

### Step 2: Configure Authentication
1. Update `src/DarbotGraphMcp.Server/appsettings.Development.json` with your Azure AD details
2. Ensure all three values are correctly set:
   - `TenantId`: Your Azure AD tenant ID
   - `ClientId`: Your app registration client ID  
   - `ClientSecret`: Your app registration client secret

### Step 3: Run the Server

```bash
# Run the MCP server
dotnet run --project src/DarbotGraphMcp.Server/DarbotGraphMcp.Server.csproj

# Server will start on http://localhost:5000
# Look for "Now listening on: http://localhost:5000" in the output
```

### Step 4: Validate Configuration

#### 4.1 Test Health Endpoint
```bash
curl http://localhost:5000/health
# Expected response: "Darbot Graph MCP Server - Healthy"
```

#### 4.2 Test Available Tools
```bash
curl http://localhost:5000/tools | jq
# Should return JSON array of 56 available tools
```

#### 4.3 Test Authentication with Simple Tool Call
```bash
curl -X POST http://localhost:5000/call-tool \
  -H "Content-Type: application/json" \
  -d '{"name": "darbot-graph-get-users", "arguments": {"top": 2}}' | jq
```

Expected successful response:
```json
{
  "users": [
    {
      "Id": "user-id-1",
      "DisplayName": "John Doe",
      "UserPrincipalName": "john.doe@yourdomain.com",
      "Mail": "john.doe@yourdomain.com",
      "JobTitle": "Developer",
      "Department": "IT"
    }
  ]
}
```

If you see demo data instead, check your Azure AD configuration.

### Step 5: Claude Desktop Integration

#### 5.1 Locate Claude Desktop Settings
- **Windows**: `%APPDATA%\Claude\claude_desktop_config.json`
- **macOS**: `~/Library/Application Support/Claude/claude_desktop_config.json`
- **Linux**: `~/.config/Claude/claude_desktop_config.json`

#### 5.2 Add MCP Server Configuration
```json
{
  "mcpServers": {
    "darbot-graph": {
      "command": "dotnet",
      "args": [
        "run", 
        "--project", 
        "/absolute/path/to/darbot-graph-mcp/src/DarbotGraphMcp.Server/DarbotGraphMcp.Server.csproj"
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

#### 5.3 Restart Claude Desktop
1. Completely close Claude Desktop
2. Restart the application
3. You should see the Darbot Graph tools available in the Claude interface

## Microsoft Facilitator Agent Integration

### Overview
The Darbot Graph MCP Server is designed to work seamlessly with Microsoft Facilitator Agent, providing a comprehensive Microsoft 365 automation platform.

### Integration Architecture

```
Microsoft Facilitator Agent
         ↓
    Claude Desktop
         ↓
  Darbot Graph MCP Server
         ↓
    Microsoft Graph API
         ↓
   Microsoft 365 Services
```

### Step 1: Microsoft Facilitator Agent Setup

#### 1.1 Install Prerequisites
```bash
# Install Node.js (if not already installed)
# Download from https://nodejs.org/

# Install Microsoft Facilitator Agent
npm install -g microsoft-facilitator-agent
```

#### 1.2 Configure Facilitator Agent
Create `facilitator-config.json`:
```json
{
  "name": "Microsoft365Facilitator",
  "description": "Advanced Microsoft 365 automation and management agent",
  "mcpServers": {
    "darbot-graph": {
      "url": "http://localhost:5000",
      "capabilities": [
        "user-management",
        "group-management", 
        "email-automation",
        "calendar-management",
        "teams-integration",
        "sharepoint-operations",
        "security-monitoring",
        "analytics-reporting"
      ]
    }
  },
  "workflows": {
    "userOnboarding": {
      "steps": [
        "darbot-graph-create-user",
        "darbot-graph-add-group-member", 
        "darbot-graph-send-mail"
      ]
    },
    "securityMonitoring": {
      "steps": [
        "darbot-graph-get-risky-users",
        "darbot-graph-get-sign-in-logs",
        "darbot-graph-get-audit-logs"
      ]
    }
  }
}
```

### Step 2: Advanced Integration Patterns

#### 2.1 User Lifecycle Management
```javascript
// Example workflow: Complete user onboarding
const onboardingWorkflow = async (userDetails) => {
  // 1. Create user account
  const user = await callTool('darbot-graph-create-user', {
    displayName: userDetails.name,
    userPrincipalName: userDetails.email,
    mailNickname: userDetails.alias,
    password: generateSecurePassword(),
    jobTitle: userDetails.role,
    department: userDetails.department
  });
  
  // 2. Add to appropriate groups
  for (const groupId of userDetails.groups) {
    await callTool('darbot-graph-add-group-member', {
      groupId: groupId,
      userId: user.userId
    });
  }
  
  // 3. Send welcome email
  await callTool('darbot-graph-send-mail', {
    to: [userDetails.email],
    subject: 'Welcome to the Organization',
    body: generateWelcomeEmail(userDetails),
    bodyType: 'Html'
  });
  
  // 4. Create calendar for first day
  await callTool('darbot-graph-create-calendar-event', {
    userId: user.userId,
    subject: 'First Day Orientation',
    startTime: userDetails.startDate + 'T09:00:00',
    endTime: userDetails.startDate + 'T17:00:00',
    body: 'Welcome to your first day!'
  });
  
  return user;
};
```

#### 2.2 Security Monitoring Automation
```javascript
// Example workflow: Daily security check
const securityMonitoringWorkflow = async () => {
  // 1. Check for risky users
  const riskyUsers = await callTool('darbot-graph-get-risky-users', {
    top: 50
  });
  
  // 2. Get recent sign-in anomalies
  const signInLogs = await callTool('darbot-graph-get-sign-in-logs', {
    top: 100,
    filter: "createdDateTime ge " + getYesterdayISO()
  });
  
  // 3. Review audit logs for suspicious activity
  const auditLogs = await callTool('darbot-graph-get-audit-logs', {
    top: 100,
    filter: "activityDateTime ge " + getYesterdayISO()
  });
  
  // 4. Generate security report
  const report = generateSecurityReport(riskyUsers, signInLogs, auditLogs);
  
  // 5. Send to security team
  await callTool('darbot-graph-send-mail', {
    to: ['security-team@company.com'],
    subject: 'Daily Security Monitoring Report',
    body: report,
    bodyType: 'Html'
  });
  
  return report;
};
```

#### 2.3 Team Collaboration Setup
```javascript
// Example workflow: Project team setup
const projectTeamSetup = async (projectDetails) => {
  // 1. Create project group
  const group = await callTool('darbot-graph-create-group', {
    displayName: projectDetails.name,
    mailNickname: projectDetails.alias,
    description: projectDetails.description,
    groupType: 'Microsoft365'
  });
  
  // 2. Add team members
  for (const memberId of projectDetails.members) {
    await callTool('darbot-graph-add-group-member', {
      groupId: group.groupId,
      userId: memberId
    });
  }
  
  // 3. Create team channels for collaboration
  const channels = ['General', 'Development', 'Documentation', 'Testing'];
  for (const channelName of channels) {
    await callTool('darbot-graph-create-team-channel', {
      teamId: group.groupId,
      displayName: channelName,
      description: `${channelName} discussions for ${projectDetails.name}`
    });
  }
  
  // 4. Send project kickoff email
  await callTool('darbot-graph-send-mail', {
    to: projectDetails.memberEmails,
    subject: `Project ${projectDetails.name} - Kickoff`,
    body: generateKickoffEmail(projectDetails),
    bodyType: 'Html'
  });
  
  return group;
};
```

### Step 3: Advanced Configuration Options

#### 3.1 Custom Tool Chains
Create custom tool combinations for common workflows:

```json
{
  "customWorkflows": {
    "quarterlyReview": [
      "darbot-graph-get-usage-reports",
      "darbot-graph-get-teams-activity", 
      "darbot-graph-get-email-activity",
      "darbot-graph-get-sharepoint-activity"
    ],
    "incidentResponse": [
      "darbot-graph-get-risky-users",
      "darbot-graph-confirm-risky-user",
      "darbot-graph-reset-user-password",
      "darbot-graph-send-mail"
    ]
  }
}
```

#### 3.2 Real-time Monitoring
Set up continuous monitoring using Server-Sent Events:

```javascript
const eventSource = new EventSource('http://localhost:5000/sse');
eventSource.onmessage = function(event) {
  if (event.data !== 'ping') {
    console.log('MCP Server Event:', event.data);
    // Process real-time events
  }
};
```

## Production Deployment Considerations

### Security Best Practices

#### 1. Credential Management
- **Never** store secrets in source code
- Use Azure Key Vault for production secrets
- Implement credential rotation policies
- Monitor for credential exposure

#### 2. Network Security
```json
{
  "NetworkSecurity": {
    "AllowedHosts": ["specific-domains-only.com"],
    "RequireHttps": true,
    "HstsMaxAge": 31536000,
    "ContentSecurityPolicy": "default-src 'self'"
  }
}
```

#### 3. Application Permissions Review
Regularly audit and minimize permissions:
- Review assigned Graph API permissions quarterly
- Remove unused permissions
- Implement least-privilege access
- Monitor permission usage through audit logs

### Performance Optimization

#### 1. Caching Configuration
```json
{
  "Caching": {
    "UserCache": {
      "ExpirationMinutes": 15,
      "MaxSize": 1000
    },
    "GroupCache": {
      "ExpirationMinutes": 30,
      "MaxSize": 500
    }
  }
}
```

#### 2. Rate Limiting
```json
{
  "RateLimit": {
    "RequestsPerMinute": 60,
    "RequestsPerHour": 1000,
    "EnableThrottling": true
  }
}
```

### Monitoring and Logging

#### 1. Application Insights Integration
```json
{
  "ApplicationInsights": {
    "InstrumentationKey": "your-insights-key",
    "CloudRole": "DarbotGraphMcpServer",
    "EnableDependencyTracking": true
  }
}
```

#### 2. Health Checks
```json
{
  "HealthChecks": {
    "GraphAPI": {
      "Timeout": "00:00:30",
      "FailureStatus": "Degraded"
    },
    "Database": {
      "Timeout": "00:00:15", 
      "FailureStatus": "Unhealthy"
    }
  }
}
```

## Troubleshooting Guide

### Common Issues and Solutions

#### 1. Authentication Failures
**Problem**: "Unable to authenticate to Microsoft Graph"
**Solutions**:
- Verify Azure AD app registration settings
- Check client secret hasn't expired
- Confirm admin consent has been granted
- Validate tenant ID format

#### 2. Permission Denied Errors  
**Problem**: "Insufficient privileges to complete the operation"
**Solutions**:
- Review required API permissions list
- Ensure admin consent is granted
- Check user/group scope limitations
- Verify application permissions vs delegated permissions

#### 3. Tool Discovery Issues
**Problem**: Tools not appearing in Claude Desktop
**Solutions**:
- Restart Claude Desktop completely
- Verify MCP server configuration path
- Check server is running on correct port
- Review environment variable configuration

#### 4. Performance Issues
**Problem**: Slow response times
**Solutions**:
- Implement response caching
- Review Graph API rate limits
- Optimize query filters
- Consider pagination for large datasets

### Diagnostic Commands

```bash
# Check server health
curl -v http://localhost:5000/health

# Verify tool count
curl http://localhost:5000/tools | jq '. | length'

# Test authentication with minimal query
curl -X POST http://localhost:5000/call-tool \
  -H "Content-Type: application/json" \
  -d '{"name": "darbot-graph-get-users", "arguments": {"top": 1}}'

# Monitor server logs
dotnet run --project src/DarbotGraphMcp.Server/DarbotGraphMcp.Server.csproj --verbosity detailed
```

### Getting Help

1. **GitHub Issues**: Report bugs and feature requests
2. **Microsoft Graph Documentation**: [https://docs.microsoft.com/en-us/graph/](https://docs.microsoft.com/en-us/graph/)
3. **MCP Protocol Specification**: [https://modelcontextprotocol.io/](https://modelcontextprotocol.io/)
4. **Azure AD Documentation**: [https://docs.microsoft.com/en-us/azure/active-directory/](https://docs.microsoft.com/en-us/azure/active-directory/)

## Tool Inventory
This MCP server provides **56 comprehensive Microsoft Graph tools**, all following the consistent `darbot-graph-` naming convention:

### User Management (8 tools)
- darbot-graph-get-users
- darbot-graph-get-user
- darbot-graph-create-user
- darbot-graph-update-user
- darbot-graph-delete-user
- darbot-graph-reset-user-password
- darbot-graph-get-user-manager
- darbot-graph-set-user-manager

### Group Management (8 tools)
- darbot-graph-get-groups
- darbot-graph-get-group
- darbot-graph-create-group
- darbot-graph-update-group
- darbot-graph-delete-group
- darbot-graph-add-group-member
- darbot-graph-remove-group-member
- darbot-graph-get-group-members

### Email Management (8 tools)
- darbot-graph-send-mail
- darbot-graph-get-mailbox-settings
- darbot-graph-get-mail-folders
- darbot-graph-create-mail-folder
- darbot-graph-get-messages
- darbot-graph-reply-to-message
- darbot-graph-forward-message
- darbot-graph-move-message

### Calendar Management (8 tools)
- darbot-graph-get-calendar-events
- darbot-graph-create-calendar-event
- darbot-graph-update-calendar-event
- darbot-graph-delete-calendar-event
- darbot-graph-get-calendars
- darbot-graph-create-calendar
- darbot-graph-accept-event
- darbot-graph-decline-event

### Teams and Communication (8 tools)
- darbot-graph-get-teams
- darbot-graph-get-team-channels
- darbot-graph-create-team-channel
- darbot-graph-get-channel-messages
- darbot-graph-send-channel-message
- darbot-graph-reply-to-channel-message
- darbot-graph-get-team-members
- darbot-graph-add-team-member

### OneDrive and SharePoint (7 tools)
- darbot-graph-get-drive-items
- darbot-graph-upload-file
- darbot-graph-download-file
- darbot-graph-share-file
- darbot-graph-get-sharepoint-sites
- darbot-graph-get-site-lists
- darbot-graph-get-list-items

### Security and Compliance (5 tools)
- darbot-graph-get-sign-in-logs
- darbot-graph-get-audit-logs
- darbot-graph-get-risky-users
- darbot-graph-confirm-risky-user
- darbot-graph-get-conditional-access-policies

### Reports and Analytics (4 tools)
- darbot-graph-get-usage-reports
- darbot-graph-get-teams-activity
- darbot-graph-get-email-activity
- darbot-graph-get-sharepoint-activity

## Operating Modes

### Production Mode
When properly configured with Azure AD credentials, the server provides full Microsoft Graph functionality:
- Real-time data from your Microsoft 365 tenant
- Complete CRUD operations for users, groups, and content
- Advanced security and compliance monitoring
- Full integration with Microsoft Facilitator Agent workflows

### Demonstration Mode  
When Azure AD credentials are not configured, the server runs in demonstration mode with:
- Sample data for user and group queries
- Placeholder responses for all tools indicating implementation status
- Full tool schema documentation for development planning
- Safe testing environment without affecting production data

## Architecture

### Darbot Graph MCP Server (`src/DarbotGraphMcp.Server/`)
- **ASP.NET Core web application** - Production-ready server framework
- **Microsoft Graph SDK integration** - Latest Graph SDK with full API coverage
- **RESTful API endpoints** - MCP protocol-compliant endpoints
- **Server-Sent Events (SSE)** - Real-time communication support
- **56 comprehensive Microsoft Graph tools** - Complete Microsoft 365 operations
- **App-only authentication** - Secure service-to-service authentication
- **Comprehensive error handling** - Robust exception management and logging
- **JSON schema validation** - Input validation for all tool calls

### Client App (`src/DarbotMcp.ClientApp/`) 
- **Blazor web application** - Interactive testing interface
- **MCP client implementation** - Full MCP protocol support
- **Real-time chat interface** - WebSocket-based communication
- **Tool testing capabilities** - Interactive tool exploration and testing

### Service Integration Layer
- **Microsoft Facilitator Agent compatibility** - Seamless workflow integration
- **Claude Desktop integration** - Native MCP server support
- **Extensible architecture** - Easy to add new tools and capabilities
- **Multi-tenant support** - Configurable for multiple Azure AD tenants

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
