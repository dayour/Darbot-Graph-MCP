# 🔒 Security Best Practices

This guide outlines security best practices for deploying and managing the Darbot Graph MCP Server in production environments.

## Overview

The Darbot Graph MCP Server requires careful security configuration to protect your Microsoft 365 environment and ensure compliance with organizational security policies.

## Credential Management

### 1. Azure AD App Registration Security

#### Principle of Least Privilege
**✅ Best Practices:**
- Only grant required permissions for your use case
- Regularly audit and remove unused permissions
- Use application permissions (not delegated) for server applications
- Create separate app registrations for different environments

**❌ Avoid:**
- Granting excessive permissions "just in case"
- Using admin accounts for app registration
- Sharing app registrations across environments

#### App Registration Configuration
```json
{
  "DisplayName": "Darbot Graph MCP Server - Production",
  "SignInAudience": "AzureADMyOrg",
  "RequiredResourceAccess": [
    {
      "ResourceAppId": "00000003-0000-0000-c000-000000000000",
      "ResourceAccess": [
        // Only include permissions you actually need
        {"Id": "df021288-bdef-4463-88db-98f22de89214", "Type": "Role"}, // User.Read.All
        {"Id": "62a82d76-70ea-41e2-9197-370581804d09", "Type": "Role"}  // Group.ReadWrite.All
      ]
    }
  ]
}
```

### 2. Client Secret Management

#### Secret Generation
**✅ Best Practices:**
- Use strong, randomly generated secrets (Azure handles this)
- Set appropriate expiration periods (12-24 months maximum)
- Document secret expiration dates
- Implement automated rotation procedures

**❌ Avoid:**
- Manually creating weak secrets
- Setting secrets to never expire
- Ignoring expiration warnings

#### Secret Storage
**✅ Production Secrets:**
```bash
# Azure Key Vault (Recommended)
az keyvault secret set --vault-name MyKeyVault --name DarbotClientSecret --value "secret-value"

# Environment Variables
export AzureAd__ClientSecret="$(az keyvault secret show --vault-name MyKeyVault --name DarbotClientSecret --query value -o tsv)"

# Kubernetes Secrets
kubectl create secret generic darbot-secrets --from-literal=client-secret="secret-value"
```

**❌ Never Store Secrets In:**
- Source code files
- Configuration files committed to git
- Plain text files on disk
- Chat messages or emails
- Screenshots or documentation

### 3. Environment Separation

#### Development vs Production
```json
// Development Environment
{
  "AzureAd": {
    "TenantId": "dev-tenant.onmicrosoft.com",
    "ClientId": "dev-app-client-id",
    "ClientSecret": "dev-secret-from-keyvault"
  }
}

// Production Environment  
{
  "AzureAd": {
    "TenantId": "prod-tenant.onmicrosoft.com",
    "ClientId": "prod-app-client-id", 
    "ClientSecret": "prod-secret-from-keyvault"
  }
}
```

## Network Security

### 1. Network Isolation

#### Firewall Configuration
```bash
# Allow only necessary traffic
iptables -A INPUT -p tcp --dport 5000 -s 127.0.0.1 -j ACCEPT
iptables -A INPUT -p tcp --dport 5000 -j DROP

# Corporate networks
iptables -A OUTPUT -d graph.microsoft.com -p tcp --dport 443 -j ACCEPT
iptables -A OUTPUT -d login.microsoftonline.com -p tcp --dport 443 -j ACCEPT
```

#### TLS/SSL Configuration
```json
{
  "Kestrel": {
    "Endpoints": {
      "Https": {
        "Url": "https://localhost:5001",
        "Certificate": {
          "Path": "/path/to/certificate.pfx",
          "Password": "certificate-password"
        }
      }
    }
  }
}
```

### 2. Proxy & Corporate Networks

#### Corporate Proxy Configuration
```json
{
  "Proxy": {
    "Url": "http://corporate-proxy:8080",
    "Username": "proxy-user",
    "Password": "proxy-password",
    "BypassOnLocal": true
  }
}
```

## Permission Management

### 1. Required Permissions by Use Case

#### Minimal User Management
```
User.Read.All           - Read user profiles
Group.Read.All          - Read group membership  
Directory.Read.All      - Read directory structure
```

#### Email Automation
```
User.Read.All           - Identify users
Mail.ReadWrite          - Read/write mail folders
Mail.Send               - Send emails on behalf of users
```

#### Full Administration
```
User.ReadWrite.All      - Full user management
Group.ReadWrite.All     - Full group management
Directory.ReadWrite.All - Full directory management
Mail.ReadWrite          - Email management
Mail.Send               - Email sending
Calendars.ReadWrite     - Calendar management
// ... add others as needed
```

### 2. Permission Validation

#### Regular Audit Script
```powershell
# PowerShell script to audit app permissions
$AppId = "your-app-client-id"
$App = Get-AzADApplication -ApplicationId $AppId
$ServicePrincipal = Get-AzADServicePrincipal -ApplicationId $AppId

# List current permissions
$ServicePrincipal.AppRole | ForEach-Object {
    Write-Host "Permission: $($_.Value) - $($_.DisplayName)"
}

# Check last used dates
Get-AzADAppCredential -ApplicationId $AppId | ForEach-Object {
    Write-Host "Secret expires: $($_.EndDate)"
}
```

## Monitoring & Logging

### 1. Application Logging

#### Structured Logging Configuration
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "logs/darbot-graph-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      },
      {
        "Name": "Console"
      }
    ]
  }
}
```

#### Security Event Logging
```csharp
// Log authentication events
logger.LogInformation("Azure AD authentication successful for tenant {TenantId}", tenantId);
logger.LogWarning("Authentication failed for tenant {TenantId}: {Error}", tenantId, error);

// Log permission usage
logger.LogInformation("Graph API call: {Tool} executed by {User}", toolName, userId);
logger.LogWarning("Permission denied for tool: {Tool}, Required: {Permission}", toolName, permission);
```

### 2. Azure AD Audit Logs

#### Monitor App Usage
```kusto
// Azure AD Sign-in logs query
SigninLogs
| where AppId == "your-app-client-id"
| where TimeGenerated > ago(24h)
| project TimeGenerated, UserPrincipalName, ClientAppUsed, IPAddress, LocationDetails
| order by TimeGenerated desc
```

#### Monitor Permission Changes
```kusto
// Azure AD Audit logs query  
AuditLogs
| where Category == "ApplicationManagement"
| where TargetResources has "your-app-client-id"
| project TimeGenerated, ActivityDisplayName, InitiatedBy, TargetResources
| order by TimeGenerated desc
```

## Compliance & Data Protection

### 1. Data Handling

#### Data Classification
**Public Data:** Basic user profiles, group names
**Internal Data:** Email content, calendar details, file metadata  
**Confidential Data:** Personal information, sensitive documents
**Restricted Data:** HR records, financial data, legal documents

#### Data Processing Guidelines
```csharp
// ✅ Secure data handling
public async Task<string> ProcessUserData(string userId)
{
    // Log access
    _logger.LogInformation("Accessing user data for {UserId}", userId);
    
    // Validate authorization
    if (!await IsAuthorizedForUser(userId))
    {
        _logger.LogWarning("Unauthorized access attempt for {UserId}", userId);
        throw new UnauthorizedAccessException();
    }
    
    // Process with minimal data retention
    var userData = await _graphClient.Users[userId].GetAsync();
    
    // Don't log sensitive data
    _logger.LogInformation("User data processed successfully");
    
    return ProcessedResult;
}
```

### 2. GDPR & Privacy Compliance

#### Data Subject Rights
- **Right to Access**: Implement user data export capabilities
- **Right to Rectification**: Ensure data update mechanisms
- **Right to Erasure**: Implement data deletion workflows
- **Right to Portability**: Support data export in standard formats

#### Privacy by Design
```json
{
  "DataRetention": {
    "LogRetentionDays": 30,
    "CacheRetentionMinutes": 15,
    "SessionTimeoutMinutes": 60
  },
  "Privacy": {
    "LogPersonalData": false,
    "AnonymizeUserIds": true,
    "EncryptSensitiveData": true
  }
}
```

## Incident Response

### 1. Security Incident Procedures

#### Detection
- Monitor for unusual API usage patterns
- Alert on authentication failures
- Track permission escalation attempts
- Monitor for data exfiltration patterns

#### Response Plan
1. **Immediate:** Revoke compromised credentials
2. **Short-term:** Review audit logs and assess impact
3. **Long-term:** Update security policies and procedures

#### Tools for Incident Response
```bash
# Revoke app secret immediately
az ad app credential delete --id your-app-id --key-id secret-key-id

# Check recent sign-ins
az ad app show --id your-app-id --query "signInAudience"

# Review permissions
az ad app permission list --id your-app-id
```

### 2. Backup & Recovery

#### Configuration Backup
```bash
#!/bin/bash
# Backup app registration configuration
az ad app show --id your-app-id > backup/app-registration-$(date +%Y%m%d).json

# Backup permissions
az ad app permission list --id your-app-id > backup/app-permissions-$(date +%Y%m%d).json
```

#### Recovery Procedures
```bash
# Restore app registration
az ad app update --id your-app-id --required-resource-accesses @backup/app-permissions.json

# Generate new secret
az ad app credential reset --id your-app-id --credential-description "Emergency Recovery"
```

## Security Checklist

### Pre-Deployment
- [ ] Separate app registrations for each environment
- [ ] Minimal required permissions granted
- [ ] Admin consent obtained for all permissions
- [ ] Client secrets stored securely (Key Vault)
- [ ] Network security configured (firewalls, TLS)
- [ ] Logging and monitoring configured
- [ ] Incident response procedures documented

### Regular Maintenance
- [ ] Review and rotate client secrets (quarterly)
- [ ] Audit app permissions (monthly)
- [ ] Review access logs (weekly)
- [ ] Test backup and recovery procedures (quarterly)
- [ ] Update security documentation (as needed)
- [ ] Security awareness training (annually)

### Monitoring Checklist
- [ ] Authentication success/failure rates
- [ ] API usage patterns and anomalies
- [ ] Permission usage and escalation
- [ ] Network traffic and connectivity
- [ ] Performance and availability metrics
- [ ] Error rates and patterns

## Emergency Contacts

### Security Incidents
- **Internal Security Team**: security@yourcompany.com
- **Microsoft Security Response Center**: secure@microsoft.com
- **Azure Support**: Create support ticket in Azure Portal

### Business Continuity
- **IT Operations**: itops@yourcompany.com
- **Business Stakeholders**: business-continuity@yourcompany.com

---

**Related:** [Setup Guide (SETUP.md)](./SETUP.md) | [Troubleshooting Guide (TROUBLESHOOTING.md)](./TROUBLESHOOTING.md) | [Azure AD Documentation](https://docs.microsoft.com/en-us/azure/active-directory/)