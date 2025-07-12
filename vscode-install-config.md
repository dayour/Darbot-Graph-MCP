# VS Code MCP Installation Configurations

## Configuration Object (before encoding)

```json
{
  "type": "stdio",
  "command": "npx",
  "args": ["-y", "@darbotlabs/darbot-graph-mcp", "${input:azure_tenant_id}", "${input:azure_client_id}", "${input:azure_client_secret}"]
}
```

## Inputs Object (before encoding)

```json
[
  {
    "id": "azure_tenant_id",
    "type": "promptString",
    "description": "Azure AD Tenant ID (leave empty for demo mode)"
  },
  {
    "id": "azure_client_id", 
    "type": "promptString",
    "description": "Azure AD Client ID (leave empty for demo mode)"
  },
  {
    "id": "azure_client_secret",
    "type": "promptString", 
    "description": "Azure AD Client Secret (leave empty for demo mode)"
  }
]
```

## Encoded URLs

After URL encoding, these become the installation links in the README.