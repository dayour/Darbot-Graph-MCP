# 🚀 Darbot Graph MCP Server Setup Guide

This guide provides step-by-step instructions for setting up the Darbot Graph MCP Server with Azure Active Directory authentication.

## Prerequisites

Before starting, ensure you have:

- ✅ **VS Code or VS Code Insiders** with MCP extension
- ✅ **.NET 8.0 SDK** ([Download](https://dotnet.microsoft.com/download))
- ✅ **Node.js 16+** for NPX installation
- ✅ **Azure subscription** with admin access to Azure Active Directory
- ✅ **Microsoft 365 tenant** (for production use)

## Quick Start Options

### Option 1: One-Click Installation (Recommended)

The fastest way to get started:

1. **Click the install button** in the [README.md](./README.md) for your VS Code version
2. **Enter your Azure AD credentials** when prompted (or leave empty for demo mode)
3. **Start using Microsoft Graph tools** immediately

### Option 2: Manual Setup

Follow this guide for custom configuration or troubleshooting.

## Step 1: Azure Active Directory Setup

### 1.1 Understanding Azure AD Concepts

**Key Terms:**
- **Tenant ID**: Your organization's unique Azure AD identifier
- **Client ID**: Your app registration's unique identifier (also called Application ID)
- **Client Secret**: A password for your app registration (secure credential)

**Tenant Selection:**
- ❌ **Don't use**: `common`, `organizations`, or Microsoft's tenant
- ✅ **Use**: Your organization's specific tenant ID (e.g., `contoso.onmicrosoft.com` or GUID)

### 1.2 Create App Registration

1. **Access Azure Portal**
   - Go to [portal.azure.com](https://portal.azure.com)
   - Sign in with admin account

2. **Navigate to App Registrations**
   - Azure Active Directory → App registrations → New registration

3. **Configure Application**
   ```
   Name: Darbot Graph MCP Server
   Supported account types: Single tenant (your organization only)
   Redirect URI: (leave blank)
   ```

4. **Record Values**
   - Copy **Application (client) ID**: `12345678-1234-1234-1234-123456789012`
   - Copy **Directory (tenant) ID**: `87654321-4321-4321-4321-210987654321`

### 1.3 Generate Client Secret

1. **Go to Certificates & secrets**
   - In your app registration → Certificates & secrets

2. **Create New Secret**
   ```
   Description: Darbot MCP Server Secret
   Expires: 24 months (recommended)
   ```

3. **Copy Secret Value**
   - ⚠️ **CRITICAL**: Copy the secret value immediately - it won't be shown again
   - Store securely (e.g., password manager)

### 1.4 Configure API Permissions

1. **Add Permissions**
   - API permissions → Add a permission → Microsoft Graph → Application permissions

2. **Required Permissions**
   Add these permissions one by one:
   ```
   User.ReadWrite.All          - Manage users
   Group.ReadWrite.All         - Manage groups  
   Directory.ReadWrite.All     - Directory operations
   Mail.ReadWrite             - Read/write mail
   Mail.Send                  - Send emails
   Calendars.ReadWrite        - Manage calendars
   Team.ReadBasic.All         - Read Teams info
   TeamSettings.ReadWrite.All - Manage Teams settings
   Files.ReadWrite.All        - Manage files
   Sites.ReadWrite.All        - Manage SharePoint sites
   Reports.Read.All           - Read usage reports
   Application.ReadWrite.All  - Manage applications
   AuditLog.Read.All         - Read audit logs
   SecurityEvents.Read.All    - Read security events
   ```

3. **Grant Admin Consent**
   - Click **Grant admin consent for [Your Organization]**
   - ✅ Verify all permissions show "Granted" with green checkmarks

## Step 2: VS Code Configuration

### 2.1 Locate Configuration File

**Windows**: `%APPDATA%\Claude\claude_desktop_config.json`
**macOS**: `~/Library/Application Support/Claude/claude_desktop_config.json`  
**Linux**: `~/.config/Claude/claude_desktop_config.json`

### 2.2 Configuration Options

#### Option A: NPM Package (Recommended)
```json
{
  "mcpServers": {
    "darbot-graph": {
      "command": "npx",
      "args": [
        "-y",
        "@darbotlabs/darbot-graph-mcp",
        "your-tenant-id-here",
        "your-client-id-here",
        "your-client-secret-here"
      ]
    }
  }
}
```

#### Option B: Direct .NET Command
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
        "AzureAd__TenantId": "your-tenant-id-here",
        "AzureAd__ClientId": "your-client-id-here",
        "AzureAd__ClientSecret": "your-client-secret-here"
      }
    }
  }
}
```

#### Option C: Demo Mode (No Azure AD)
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

### 2.3 Configuration Examples

#### Production Environment
```json
{
  "mcpServers": {
    "darbot-graph": {
      "command": "npx",
      "args": [
        "-y",
        "@darbotlabs/darbot-graph-mcp",
        "contoso.onmicrosoft.com",
        "12345678-1234-1234-1234-123456789012",
        "abc123XYZ~secretvalue.here-456"
      ]
    }
  }
}
```

#### Development/Testing
```json
{
  "mcpServers": {
    "darbot-graph-dev": {
      "command": "npx", 
      "args": ["-y", "@darbotlabs/darbot-graph-mcp"]
    }
  }
}
```

## Step 3: Validation

### 3.1 Restart VS Code
- **Completely close** VS Code
- **Restart** to load the new MCP server configuration

### 3.2 Test Connection
1. Open VS Code
2. Access MCP tools (check extension)
3. Try a simple command: "List the first 5 users in my organization"

### 3.3 Verify Tool Availability
You should see 64+ Microsoft Graph tools available:
- User management tools
- Group management tools  
- Email tools
- Calendar tools
- Teams tools
- File management tools
- SharePoint tools
- Security tools
- Reports tools
- Application tools

## Step 4: Security Configuration

### 4.1 Credential Storage

**❌ Don't:**
- Store secrets in plain text files
- Share credentials in chat/email
- Use personal Microsoft accounts for production

**✅ Do:**
- Use organization-specific tenant ID
- Store secrets in password managers
- Implement credential rotation
- Use environment variables for production

### 4.2 Permission Validation

Verify your permissions work:
```bash
# Test health endpoint
curl http://localhost:5000/health

# Test tools list
curl http://localhost:5000/tools | jq length
# Should return: 64

# Test simple tool
curl -X POST http://localhost:5000/call-tool \
  -H "Content-Type: application/json" \
  -d '{"name": "darbot-graph-users-list", "arguments": {"top": 2}}'
```

## Common Setup Scenarios

### Corporate Environment
- Use your organization's tenant ID (not `common`)
- Ensure admin consent is granted
- Consider conditional access policies

### Development Environment  
- Use demo mode for safe testing
- Create separate dev app registration
- Use minimal permissions for testing

### Multi-Tenant Environment
- Each tenant needs separate app registration
- Use tenant-specific configuration
- Avoid cross-tenant operations

## Next Steps

After successful setup:

1. **Explore Tools**: Review [cmd_lib.md](./cmd_lib.md) for all available commands
2. **Check Security**: Review [SECURITY.md](./SECURITY.md) for best practices
3. **Troubleshooting**: See [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) for common issues
4. **Start Automating**: Begin using Microsoft Graph tools in your workflows

## Support

If you encounter issues:
1. Check [TROUBLESHOOTING.md](./TROUBLESHOOTING.md)
2. Verify Azure AD configuration
3. Test with demo mode first
4. Review server logs for errors

---

**Next:** [Security Best Practices (SECURITY.md)](./SECURITY.md) | [Troubleshooting Guide (TROUBLESHOOTING.md)](./TROUBLESHOOTING.md)