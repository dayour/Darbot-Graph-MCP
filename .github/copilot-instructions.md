# Copilot Instructions for Darbot-Graph-MCP

## Project Overview
Extensible MCP server with 64+ tools for Microsoft Graph API. Built on official Microsoft Graph SDKs (v1.0 and Beta) in C#/.NET 8.

## Architecture
- `src/DarbotGraphMcp.Server/` - Main MCP server
- `src/DarbotGraphMcp.Core/` - Graph service and tool implementations
- `tests/` - Test projects
- `npm-wrapper/` - NPX installation wrapper

## Code Style
- C# .NET 8.0, solution file DarbotGraphMcp.sln
- Tool naming: darbot-graph-{category}-{action}
- Microsoft.Graph SDK for stable, Microsoft.Graph.Beta for preview
- Demo mode for safe testing without Azure AD
