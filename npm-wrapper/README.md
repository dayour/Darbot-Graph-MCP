# Darbot Graph MCP NPM Wrapper

This NPM package provides a convenient wrapper for the Darbot Graph MCP Server, enabling easy installation and integration with VS Code's MCP system.

## Installation

```bash
# Global installation
npm install -g @darbotlabs/darbot-graph-mcp

# Or use directly with npx (recommended)
npx @darbotlabs/darbot-graph-mcp
```

## Usage

### Demo Mode (No Azure AD setup required)
```bash
npx @darbotlabs/darbot-graph-mcp
```

### Production Mode (With Azure AD credentials)
```bash
npx @darbotlabs/darbot-graph-mcp <tenant-id> <client-id> <client-secret>
```

### Environment Variables
```bash
export AZURE_TENANT_ID="your-tenant-id"
export AZURE_CLIENT_ID="your-client-id"  
export AZURE_CLIENT_SECRET="your-client-secret"
npx @darbotlabs/darbot-graph-mcp
```

## VS Code Integration

This package is designed to work seamlessly with VS Code's MCP server installation system:

```json
{
  "mcpServers": {
    "darbot-graph": {
      "command": "npx",
      "args": ["-y", "@darbotlabs/darbot-graph-mcp", "tenant-id", "client-id", "client-secret"]
    }
  }
}
```

## Prerequisites

- Node.js 16+ (for NPX)
- .NET 8.0 SDK or later
- Darbot Graph MCP repository (automatically detected)

## How It Works

1. **Detects Installation**: Searches for the Darbot Graph MCP Server in common locations
2. **Builds If Needed**: Automatically builds the .NET project if required
3. **Configures Authentication**: Sets up Azure AD environment variables
4. **Starts Server**: Launches the .NET MCP server on localhost:5000
5. **Handles Lifecycle**: Manages graceful startup/shutdown

## Supported Locations

The wrapper automatically searches for the server in:
- `./src/DarbotGraphMcp.Server` (relative to npm package)
- `./Darbot-Graph-MCP/src/DarbotGraphMcp.Server` (in current directory)
- `~/Darbot-Graph-MCP/src/DarbotGraphMcp.Server` (in home directory)

## Features

- ✅ **One-click Installation**: Compatible with VS Code MCP redirect system
- ✅ **Demo Mode**: Safe testing without Azure AD setup
- ✅ **Flexible Configuration**: Support for arguments and environment variables
- ✅ **Automatic Discovery**: Finds the .NET server automatically
- ✅ **Graceful Handling**: Proper startup/shutdown lifecycle management
- ✅ **Cross-Platform**: Works on Windows, macOS, and Linux

## License

MIT License - see the main repository for details.

## Repository

[https://github.com/dayour/Darbot-Graph-MCP](https://github.com/dayour/Darbot-Graph-MCP)