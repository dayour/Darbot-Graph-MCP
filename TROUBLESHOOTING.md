# 🔧 Troubleshooting Guide

This guide helps resolve common issues when setting up and using the Darbot Graph MCP Server.

## Quick Diagnostics

### Check Server Status
```bash
# Test if server is running
curl http://localhost:5000/health
# Expected: "Darbot Graph MCP Server - Enhanced"

# Check tool count
curl http://localhost:5000/tools | jq length  
# Expected: 64
```

### Check Configuration
```bash
# Verify .NET SDK
dotnet --version
# Expected: 8.0.x or higher

# Test build
cd /path/to/darbot-graph-mcp
dotnet build
# Expected: Build succeeded
```

## Common Issues & Solutions

### 1. Authentication & Authorization Issues

#### ❌ Error: "Tenant not found" or "Invalid tenant"
**Symptoms:**
- Can't authenticate to Azure AD
- "The tenant 'common' not found" error

**Solutions:**
```json
// ❌ WRONG - Don't use generic tenants
"AzureAd__TenantId": "common"
"AzureAd__TenantId": "organizations" 

// ✅ CORRECT - Use your specific tenant
"AzureAd__TenantId": "contoso.onmicrosoft.com"
"AzureAd__TenantId": "12345678-1234-1234-1234-123456789012"
```

**How to find your tenant ID:**
1. Azure Portal → Azure Active Directory → Overview
2. Copy "Tenant ID" value
3. Or use your domain: `yourcompany.onmicrosoft.com`

#### ❌ Error: "Insufficient privileges" or "Access denied"
**Symptoms:**
- Tools return permission errors
- Can't access user/group data

**Solutions:**
1. **Check Admin Consent**
   - Azure Portal → App registrations → Your app → API permissions
   - Ensure all permissions show "Granted for [Organization]"
   - If not granted, click "Grant admin consent"

2. **Verify Required Permissions**
   ```
   ✅ User.ReadWrite.All
   ✅ Group.ReadWrite.All  
   ✅ Mail.ReadWrite
   ✅ Mail.Send
   ✅ Calendars.ReadWrite
   ✅ Directory.ReadWrite.All
   ```

3. **Check Permission Type**
   - Use **Application permissions** (not Delegated)
   - Application permissions work for app-only authentication

#### ❌ Error: "Invalid client secret" or "Authentication failed"
**Symptoms:**
- Server starts but API calls fail
- "401 Unauthorized" responses

**Solutions:**
1. **Check Secret Expiration**
   - Azure Portal → App registrations → Your app → Certificates & secrets
   - Verify secret hasn't expired
   - Generate new secret if needed

2. **Verify Secret Value**
   - Ensure you copied the secret **value** (not the ID)
   - Check for hidden characters or extra spaces
   - Regenerate secret if unsure

### 2. VS Code Integration Issues

#### ❌ Error: "MCP server not found" or "Tools not available"
**Symptoms:**
- No Darbot Graph tools in VS Code
- MCP extension shows no servers

**Solutions:**
1. **Check Configuration File Location**
   ```bash
   # Windows
   %APPDATA%\Claude\claude_desktop_config.json
   
   # macOS  
   ~/Library/Application Support/Claude/claude_desktop_config.json
   
   # Linux
   ~/.config/Claude/claude_desktop_config.json
   ```

2. **Validate JSON Syntax**
   ```bash
   # Test JSON validity
   cat claude_desktop_config.json | jq .
   # Should not show syntax errors
   ```

3. **Complete VS Code Restart**
   - Close VS Code completely (all windows)
   - Wait 5 seconds
   - Restart VS Code
   - Check MCP extension status

#### ❌ Error: "Command not found: npx" or "dotnet not found"
**Symptoms:**
- VS Code can't start the MCP server
- Path-related errors

**Solutions:**
1. **Install Missing Dependencies**
   ```bash
   # Install Node.js (for npx)
   # Download from: https://nodejs.org
   
   # Install .NET 8.0 SDK
   # Download from: https://dotnet.microsoft.com/download
   ```

2. **Use Absolute Paths**
   ```json
   {
     "mcpServers": {
       "darbot-graph": {
         "command": "/usr/local/bin/dotnet",
         "args": ["run", "--project", "/full/path/to/DarbotGraphMcp.Server"]
       }
     }
   }
   ```

### 3. Network & Connectivity Issues

#### ❌ Error: "Connection refused" or "Port in use"
**Symptoms:**
- Server won't start
- Port 5000 conflicts

**Solutions:**
1. **Check Port Usage**
   ```bash
   # Check if port 5000 is in use
   netstat -ano | findstr :5000  # Windows
   lsof -i :5000                 # macOS/Linux
   ```

2. **Use Different Port**
   ```json
   {
     "env": {
       "ASPNETCORE_URLS": "http://localhost:5001"
     }
   }
   ```

#### ❌ Error: "Network timeouts" or "Graph API unreachable"
**Symptoms:**
- Slow responses or timeouts
- Intermittent connectivity issues

**Solutions:**
1. **Check Internet Connection**
   ```bash
   # Test Graph API connectivity
   curl https://graph.microsoft.com/v1.0/me
   ```

2. **Corporate Firewall/Proxy**
   - Contact IT to allow `graph.microsoft.com`
   - Configure proxy settings if needed

### 4. Configuration Issues

#### ❌ Error: "appsettings.json not found" or "Invalid configuration"
**Symptoms:**
- Server fails to start
- Configuration errors in logs

**Solutions:**
1. **Check File Exists**
   ```bash
   ls src/DarbotGraphMcp.Server/appsettings.json
   ```

2. **Validate JSON Format**
   ```json
   {
     "AzureAd": {
       "TenantId": "your-tenant-id",
       "ClientId": "your-client-id", 
       "ClientSecret": "your-client-secret"
     }
   }
   ```

3. **Use Environment Variables Instead**
   ```bash
   export AzureAd__TenantId="your-tenant-id"
   export AzureAd__ClientId="your-client-id"  
   export AzureAd__ClientSecret="your-client-secret"
   ```

#### ❌ Error: "Invalid input parameters" or "Schema validation failed"
**Symptoms:**
- Tool calls fail with validation errors
- Parameter format issues

**Solutions:**
1. **Check Parameter Format**
   ```json
   // ❌ WRONG
   {"startTime": "2024-01-15 09:00:00"}
   
   // ✅ CORRECT  
   {"startTime": "2024-01-15T09:00:00"}
   ```

2. **Use Demo Mode for Testing**
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

### 5. Build & Runtime Issues

#### ❌ Error: "Build failed" or "Compilation errors"
**Symptoms:**
- dotnet build fails
- Missing dependencies

**Solutions:**
1. **Clean and Rebuild**
   ```bash
   dotnet clean
   dotnet restore
   dotnet build
   ```

2. **Check .NET Version**
   ```bash
   dotnet --version
   # Must be 8.0.x or higher
   ```

3. **Update NuGet Packages**
   ```bash
   dotnet list package --outdated
   dotnet update
   ```

#### ❌ Error: "Assembly load errors" or "Runtime failures"  
**Symptoms:**
- Server starts but crashes immediately
- Missing assembly errors

**Solutions:**
1. **Check Dependencies**
   ```bash
   dotnet restore --force
   dotnet clean
   dotnet build
   ```

2. **Run with Detailed Logging**
   ```bash
   dotnet run --project src/DarbotGraphMcp.Server --verbosity detailed
   ```

## Advanced Troubleshooting

### Enable Debug Logging
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

### Test Individual Components

#### Test Azure AD Authentication
```bash
# Test with Azure CLI
az login --tenant your-tenant-id
az account show
```

#### Test Graph API Access
```bash
# Get access token
curl -X POST https://login.microsoftonline.com/your-tenant-id/oauth2/v2.0/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "client_id=your-client-id&client_secret=your-client-secret&scope=https://graph.microsoft.com/.default&grant_type=client_credentials"

# Use token to test Graph API
curl -H "Authorization: Bearer your-access-token" \
  https://graph.microsoft.com/v1.0/users
```

### Performance Troubleshooting

#### Check Response Times
```bash
# Measure health check time
time curl http://localhost:5000/health

# Test tool performance
time curl -X POST http://localhost:5000/call-tool \
  -H "Content-Type: application/json" \
  -d '{"name": "darbot-graph-users-list", "arguments": {"top": 5}}'
```

#### Monitor Resource Usage
```bash
# Check memory usage
dotnet-counters monitor --process-id $(pgrep -f DarbotGraphMcp.Server)
```

## Environment-Specific Issues

### Windows Issues
- **PowerShell Execution Policy**: `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser`
- **Long Path Support**: Enable in Windows settings for deep directory structures
- **Antivirus**: Exclude project directory from real-time scanning

### macOS Issues  
- **Gatekeeper**: Allow dotnet execution if blocked
- **File Permissions**: `chmod +x` for any shell scripts
- **Homebrew**: Use `brew install dotnet` for easier installation

### Linux Issues
- **Package Manager**: Use distribution-specific .NET installation
- **File Permissions**: Ensure proper execute permissions
- **SELinux**: Configure if running on RHEL/CentOS

## Getting Help

### Before Reporting Issues
1. ✅ Check this troubleshooting guide
2. ✅ Test with demo mode
3. ✅ Verify Azure AD configuration
4. ✅ Check server logs
5. ✅ Test with minimal configuration

### Gathering Debug Information
```bash
# System information
dotnet --info
node --version
npx --version

# Project information  
dotnet list package
git status
git log --oneline -5

# Test connectivity
curl -v http://localhost:5000/health
curl -v http://localhost:5000/tools
```

### Logs to Include
- Server console output
- VS Code MCP extension logs
- Azure AD sign-in logs (if applicable)
- Network traces (if connectivity issues)

---

**Related:** [Setup Guide (SETUP.md)](./SETUP.md) | [Security Guide (SECURITY.md)](./SECURITY.md) | [Complete Command Reference (cmd_lib.md)](./cmd_lib.md)