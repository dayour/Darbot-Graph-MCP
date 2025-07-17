# Authentication Configuration Guide

This guide provides comprehensive information about the authentication methods supported by the Darbot Graph MCP Server and how to configure them securely.

## 🔐 Authentication Methods

The Darbot Graph MCP Server supports multiple authentication methods, automatically detecting the best available option:

### 1. Client Secret Authentication
Traditional app registration with client secret.

**Environment Variables:**
```bash
export AZURE_TENANT_ID="your-tenant-id"
export AZURE_CLIENT_ID="your-client-id"
export AZURE_CLIENT_SECRET="your-client-secret"
```

**VS Code Configuration:**
```json
{
  "mcp.servers": {
    "darbot-graph": {
      "command": "npx",
      "args": ["-y", "@darbotlabs/darbot-graph-mcp"],
      "env": {
        "AZURE_TENANT_ID": "your-tenant-id",
        "AZURE_CLIENT_ID": "your-client-id",
        "AZURE_CLIENT_SECRET": "your-client-secret"
      }
    }
  }
}
```

### 2. Azure CLI Authentication
Uses existing `az login` session.

**Prerequisites:**
1. Install Azure CLI
2. Run `az login`
3. Verify: `az account show`

**Environment Variables:**
```bash
export AZURE_TENANT_ID="your-tenant-id"
export AZURE_USE_CLI=true
```

**VS Code Configuration:**
```json
{
  "mcp.servers": {
    "darbot-graph": {
      "command": "npx",
      "args": ["-y", "@darbotlabs/darbot-graph-mcp"],
      "env": {
        "AZURE_TENANT_ID": "your-tenant-id",
        "AZURE_USE_CLI": "true"
      }
    }
  }
}
```

### 3. VS Code Authentication
Uses VS Code's built-in Azure authentication.

**Prerequisites:**
1. Install Azure Account extension
2. Sign in to Azure through VS Code

**Environment Variables:**
```bash
export AZURE_TENANT_ID="your-tenant-id"
export AZURE_USE_VSCODE=true
```

### 4. Managed Identity Authentication
For applications running on Azure infrastructure.

**Environment Variables:**
```bash
export AZURE_USE_MANAGED_IDENTITY=true
# Optional: specify client ID for user-assigned managed identity
export AZURE_CLIENT_ID="your-managed-identity-client-id"
```

### 5. Default Azure Credential Chain
Tries multiple authentication methods automatically:
1. Environment variables
2. Managed Identity
3. Azure CLI
4. VS Code
5. Visual Studio

**Environment Variables:**
```bash
export AZURE_TENANT_ID="your-tenant-id"  # optional
export AZURE_USE_DEFAULT_CHAIN=true
```

### 6. Demo Mode
Automatically enabled when no authentication is configured. Returns sample data for testing.

## 🛠️ Configuration Priority

The authentication service checks methods in this order:

1. **Client Secret** (if all three values are present)
2. **Azure CLI** (if UseAzureCli=true and TenantId is present)
3. **Managed Identity** (if UseManagedIdentity=true)
4. **VS Code** (if UseVSCode=true and TenantId is present)
5. **Default Chain** (if UseDefaultChain=true)
6. **Demo Mode** (fallback)

## 📝 Environment Variable Formats

### User-Friendly Format (Recommended)
```bash
AZURE_TENANT_ID
AZURE_CLIENT_ID
AZURE_CLIENT_SECRET
AZURE_USE_CLI
AZURE_USE_VSCODE
AZURE_USE_MANAGED_IDENTITY
AZURE_USE_DEFAULT_CHAIN
```

### Legacy Format (Still Supported)
```bash
AZURE_AD_TENANT_ID
AZURE_AD_CLIENT_ID
AZURE_AD_CLIENT_SECRET
AZURE_AD_USE_AZURE_CLI
AZURE_AD_USE_VSCODE
AZURE_AD_USE_MANAGED_IDENTITY
AZURE_AD_USE_DEFAULT_CHAIN
```

### .NET Configuration Format
```bash
AzureAd__TenantId
AzureAd__ClientId
AzureAd__ClientSecret
AzureAd__UseAzureCli
AzureAd__UseVSCode
AzureAd__UseManagedIdentity
AzureAd__UseDefaultChain
```

## 🔍 Configuration Validation

### Check Authentication Status
```bash
curl http://localhost:5000/auth-info
```

Response example:
```json
{
  "isConfigured": true,
  "authenticationMethod": "AzureCLI",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Common Authentication Methods
- `"ClientSecret"` - App registration with secret
- `"AzureCLI"` - Azure CLI session
- `"VSCodeCredential"` - VS Code authentication
- `"ManagedIdentity"` - Azure managed identity
- `"DefaultAzure"` - Default credential chain
- `"Demo"` - No authentication configured

## 🎯 Configuration Wizard

Use the interactive configuration wizard for guided setup:

```bash
curl -sSL https://raw.githubusercontent.com/dayour/Darbot-Graph-MCP/main/scripts/configure-mcp.sh | bash
```

Or run locally:
```bash
git clone https://github.com/dayour/Darbot-Graph-MCP.git
cd Darbot-Graph-MCP
./scripts/configure-mcp.sh
```

## 🛡️ Security Best Practices

### ✅ Recommended Practices
- **Use Azure CLI or VS Code authentication** for development
- **Use Managed Identity** for Azure hosting
- **Store secrets in environment variables**, not config files
- **Use Azure Key Vault** for production secrets
- **Rotate client secrets regularly**
- **Use least-privilege permissions**

### ❌ Security Anti-Patterns
- Hardcoding secrets in configuration files
- Storing secrets in version control
- Using demo mode in production
- Sharing client secrets in chat/email
- Using overly broad permissions

## 🚨 Troubleshooting

### Authentication Issues

**Check Environment Variables:**
```bash
echo $AZURE_TENANT_ID
echo $AZURE_USE_CLI
```

**Verify Azure CLI Login:**
```bash
az account show
az account list
```

**Test Server Authentication:**
```bash
curl http://localhost:5000/auth-info
curl http://localhost:5000/health
```

**Enable Debug Logging:**
```bash
LOGGING__LOGLEVEL__DEFAULT=Debug dotnet run
```

### Common Error Messages

**"No valid authentication configuration found"**
- Solution: Set appropriate environment variables or run configuration wizard

**"Graph client authentication failed"**
- Solution: Check Azure AD permissions and verify credentials

**"Resource temporarily unavailable"**
- Solution: Check network connectivity to login.microsoftonline.com

## 📊 Authentication Flow

```
1. Server Startup
   ↓
2. Authentication Service Initialization
   ↓
3. Configuration Detection (priority order)
   ↓
4. Credential Creation
   ↓
5. Graph Client Initialization
   ↓
6. Ready for MCP Requests
```

## 🔄 Switching Authentication Methods

To change authentication methods:

1. **Stop the server**
2. **Update environment variables**
3. **Restart the server**
4. **Verify new method**: `curl http://localhost:5000/auth-info`

Example switching from Client Secret to Azure CLI:
```bash
# Remove client secret variables
unset AZURE_CLIENT_SECRET

# Enable Azure CLI
export AZURE_USE_CLI=true

# Restart server and verify
curl http://localhost:5000/auth-info
```

## 📞 Support

- **Configuration Issues**: Use the configuration wizard
- **Authentication Problems**: Check the troubleshooting section
- **Azure AD Setup**: See the main README.md
- **Environment Variables**: Review the supported formats above