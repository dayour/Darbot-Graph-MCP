# 🎯 VS Code MCP Quick Setup

This guide shows how to quickly configure the Darbot Graph MCP Server in VS Code with minimal setup.

## Prerequisites

- ✅ VS Code or VS Code Insiders with MCP extension
- ✅ Node.js 16+ (for NPX method)

## Option 1: One-Click Installation (Easiest)

1. **Click the install button** in the [main README](./README.md)
2. **Follow the prompts** in VS Code
3. **Enter credentials or leave empty** for demo mode
4. **Done!** Tools available immediately

## Option 2: Manual Configuration

### Step 1: Find Configuration File

**Windows:**
```
%APPDATA%\Claude\claude_desktop_config.json
```

**macOS:**
```
~/Library/Application Support/Claude/claude_desktop_config.json
```

**Linux:**
```
~/.config/Claude/claude_desktop_config.json
```

### Step 2: Add Server Configuration

#### Demo Mode (No Azure AD needed)
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

#### Production Mode (With Azure AD)
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

### Step 3: Restart VS Code

- **Close VS Code completely** (all windows)
- **Wait 5 seconds**
- **Restart VS Code**

## Configuration Examples

### Multiple Environments
```json
{
  "mcpServers": {
    "darbot-graph-dev": {
      "command": "npx",
      "args": ["-y", "@darbotlabs/darbot-graph-mcp"]
    },
    "darbot-graph-prod": {
      "command": "npx", 
      "args": [
        "-y",
        "@darbotlabs/darbot-graph-mcp",
        "prod.onmicrosoft.com",
        "prod-client-id",
        "prod-client-secret"
      ]
    }
  }
}
```

### Local Development
```json
{
  "mcpServers": {
    "darbot-graph-local": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/path/to/darbot-graph-mcp/src/DarbotGraphMcp.Server"
      ],
      "env": {
        "AzureAd__TenantId": "",
        "AzureAd__ClientId": "",
        "AzureAd__ClientSecret": ""
      }
    }
  }
}
```

## Quick Validation

### 1. Check MCP Extension
- Open VS Code
- Look for MCP extension in extensions panel
- Verify "darbot-graph" server appears

### 2. Test Tools
Ask Claude: "List the first 3 users in my organization"

Expected tools available:
- `darbot-graph-users-list`
- `darbot-graph-mail-send`
- `darbot-graph-calendar-events-create`
- And 61+ more...

## Common Issues

### ❌ "MCP server not found"
**Solution:** Completely restart VS Code

### ❌ "Command not found: npx"
**Solution:** Install Node.js from [nodejs.org](https://nodejs.org)

### ❌ "Authentication failed"
**Solution:** Check tenant ID format:
- ✅ Use: `company.onmicrosoft.com` or GUID
- ❌ Don't use: `common`, `organizations`

### ❌ "No tools available"
**Solution:** 
1. Check configuration file syntax with JSON validator
2. Verify file is saved in correct location
3. Restart VS Code completely

## Getting Azure AD Credentials

If you need Azure AD credentials:

1. **Quick Setup:** See [SETUP.md](./SETUP.md) for complete guide
2. **Security:** Review [SECURITY.md](./SECURITY.md) for best practices
3. **Troubleshooting:** Check [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) for help

## Demo Mode vs Production

### Demo Mode
- ✅ **Safe for testing** - no real data access
- ✅ **No Azure AD required** - works immediately
- ✅ **All tools available** - test functionality
- ❌ **Sample data only** - not real Microsoft 365 data

### Production Mode
- ✅ **Real Microsoft 365 data** - actual users, emails, calendars
- ✅ **Full functionality** - create, update, delete operations
- ⚠️ **Requires Azure AD setup** - need app registration
- ⚠️ **Security considerations** - follow best practices

## Next Steps

1. **Start with Demo Mode** to explore tools
2. **Set up Azure AD** when ready for production
3. **Review Documentation** for advanced features
4. **Check Security Guide** before production deployment

---

**Related:** [Complete Setup Guide (SETUP.md)](./SETUP.md) | [Security Guide (SECURITY.md)](./SECURITY.md) | [Troubleshooting (TROUBLESHOOTING.md)](./TROUBLESHOOTING.md)