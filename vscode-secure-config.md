# VS Code MCP Secure Configuration Guide

This guide provides multiple secure configuration options for the Darbot Graph MCP Server in VS Code.

## 🔐 Secure Configuration Options

### Option 1: Environment Variables (Recommended)

The most secure approach using environment variables:

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

### Option 2: Azure CLI Authentication

Use existing Azure CLI session:

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

Prerequisites:
1. Install Azure CLI
2. Run `az login`
3. Ensure you're in the correct tenant

### Option 3: VS Code Authentication

Use VS Code's built-in Azure authentication:

```json
{
  "mcp.servers": {
    "darbot-graph": {
      "command": "npx",
      "args": ["-y", "@darbotlabs/darbot-graph-mcp"],
      "env": {
        "AZURE_TENANT_ID": "your-tenant-id",
        "AZURE_USE_VSCODE": "true"
      }
    }
  }
}
```

Prerequisites:
1. Install Azure Account extension in VS Code
2. Sign in to Azure through VS Code

### Option 4: Default Azure Credential Chain

Try multiple authentication methods automatically:

```json
{
  "mcp.servers": {
    "darbot-graph": {
      "command": "npx",
      "args": ["-y", "@darbotlabs/darbot-graph-mcp"],
      "env": {
        "AZURE_TENANT_ID": "your-tenant-id",
        "AZURE_USE_DEFAULT_CHAIN": "true"
      }
    }
  }
}
```

This tries: Environment → Managed Identity → Azure CLI → VS Code → Visual Studio

### Option 5: Managed Identity (Azure Hosting)

For applications running on Azure infrastructure:

```json
{
  "mcp.servers": {
    "darbot-graph": {
      "command": "npx",
      "args": ["-y", "@darbotlabs/darbot-graph-mcp"],
      "env": {
        "AZURE_USE_MANAGED_IDENTITY": "true"
      }
    }
  }
}
```

## 🎯 One-Click Secure Installation

### Environment Variables Setup
[![Install with Environment Variables](https://img.shields.io/badge/VS_Code-Secure_Install-green?style=flat-square&logo=visualstudiocode)](https://insiders.vscode.dev/redirect/mcp/install?name=darbot-graph&config=%7B%22type%22%3A%22stdio%22%2C%22command%22%3A%22npx%22%2C%22args%22%3A%5B%22-y%22%2C%22%40darbotlabs%2Fdarbot-graph-mcp%22%5D%2C%22env%22%3A%7B%22AZURE_TENANT_ID%22%3A%22%24%7Benv%3AAZURE_TENANT_ID%7D%22%2C%22AZURE_CLIENT_ID%22%3A%22%24%7Benv%3AAZURE_CLIENT_ID%7D%22%2C%22AZURE_CLIENT_SECRET%22%3A%22%24%7Benv%3AAZURE_CLIENT_SECRET%7D%22%7D%7D)

Prerequisites: Set environment variables before clicking

### Azure CLI Setup
[![Install with Azure CLI](https://img.shields.io/badge/VS_Code-Azure_CLI_Install-blue?style=flat-square&logo=visualstudiocode)](https://insiders.vscode.dev/redirect/mcp/install?name=darbot-graph&config=%7B%22type%22%3A%22stdio%22%2C%22command%22%3A%22npx%22%2C%22args%22%3A%5B%22-y%22%2C%22%40darbotlabs%2Fdarbot-graph-mcp%22%5D%2C%22env%22%3A%7B%22AZURE_TENANT_ID%22%3A%22%24%7Binput%3Aazure_tenant_id%7D%22%2C%22AZURE_USE_CLI%22%3A%22true%22%7D%7D&inputs=%5B%7B%22id%22%3A%22azure_tenant_id%22%2C%22type%22%3A%22promptString%22%2C%22description%22%3A%22Azure%20AD%20Tenant%20ID%22%7D%5D)

Prerequisites: Run `az login` first

### Demo Mode
[![Install Demo Mode](https://img.shields.io/badge/VS_Code-Demo_Mode-yellow?style=flat-square&logo=visualstudiocode)](https://insiders.vscode.dev/redirect/mcp/install?name=darbot-graph&config=%7B%22type%22%3A%22stdio%22%2C%22command%22%3A%22npx%22%2C%22args%22%3A%5B%22-y%22%2C%22%40darbotlabs%2Fdarbot-graph-mcp%22%5D%7D)

No prerequisites - safe testing with sample data

## 🛠️ Configuration Wizard

Run the interactive configuration wizard:

```bash
curl -sSL https://raw.githubusercontent.com/dayour/Darbot-Graph-MCP/main/scripts/configure-mcp.sh | bash
```

Or download and run locally:

```bash
git clone https://github.com/dayour/Darbot-Graph-MCP.git
cd Darbot-Graph-MCP
./scripts/configure-mcp.sh
```

## 🔍 Configuration Validation

Test your configuration:

```bash
# Check authentication status
curl http://localhost:5000/auth-info

# Test basic functionality
curl http://localhost:5000/health

# Verify tool count
curl http://localhost:5000/tools | jq length
```

## 🔒 Security Best Practices

### ✅ Do
- Use environment variables for credentials
- Use Azure CLI or VS Code authentication when possible
- Store secrets in secure credential managers
- Use Managed Identity on Azure infrastructure
- Regularly rotate client secrets
- Use least-privilege permissions

### ❌ Don't
- Hardcode secrets in configuration files
- Store secrets in version control
- Use overly broad permissions
- Share client secrets in chat or email
- Use demo mode in production

## 📁 Configuration File Locations

### VS Code Settings
- **Windows**: `%APPDATA%\Code\User\settings.json`
- **macOS**: `~/Library/Application Support/Code/User/settings.json`
- **Linux**: `~/.config/Code/User/settings.json`

### Claude Desktop Config
- **Windows**: `%APPDATA%\Claude\claude_desktop_config.json`
- **macOS**: `~/Library/Application Support/Claude/claude_desktop_config.json`
- **Linux**: `~/.config/Claude/claude_desktop_config.json`

## 🆘 Troubleshooting

### Authentication Issues
1. Check environment variables: `echo $AZURE_AD_TENANT_ID`
2. Verify Azure CLI login: `az account show`
3. Test server auth endpoint: `curl http://localhost:5000/auth-info`

### VS Code Issues
1. Restart VS Code completely
2. Check VS Code output panel for MCP logs
3. Verify file paths in configuration

### Permission Issues
1. Review Azure AD app permissions
2. Ensure admin consent is granted
3. Check scope limitations

## 📞 Support

- **Configuration Issues**: Use the configuration wizard
- **Documentation**: [GitHub Repository](https://github.com/dayour/Darbot-Graph-MCP)
- **Azure AD Setup**: [Microsoft Documentation](https://docs.microsoft.com/azure/active-directory)