# Security Guide

## Overview

The Darbot Graph MCP Server implements multiple security layers to protect against common vulnerabilities and ensure safe operation in enterprise environments.

## Security Features

### Tenant Validation
- **Cross-tenant protection**: Validates tenant IDs to prevent accidental resource creation in wrong tenants
- **Placeholder detection**: Blocks common placeholder tenant IDs that could cause security issues
- **GUID format validation**: Ensures tenant IDs follow proper GUID format

### Authentication Security
- **Credential validation**: Validates Azure AD credentials before use
- **Demo mode fallback**: Safe operation when credentials are not provided
- **Environment variable support**: Secure credential management through environment variables

### Input Validation
- **Parameter sanitization**: All tool inputs are validated and sanitized
- **Type checking**: Strong typing prevents injection attacks
- **Length limits**: Prevents buffer overflow attacks

## Security Best Practices

### 1. Credential Management

#### Production Environment
```bash
# Use environment variables (recommended)
export AzureAd__TenantId="your-actual-tenant-id"
export AzureAd__ClientId="your-client-id" 
export AzureAd__ClientSecret="your-client-secret"
```

#### Azure Key Vault (Enterprise)
```json
{
  "AzureAd": {
    "TenantId": "@Microsoft.KeyVault(SecretUri=https://vault.vault.azure.net/secrets/tenant-id/)",
    "ClientId": "@Microsoft.KeyVault(SecretUri=https://vault.vault.azure.net/secrets/client-id/)",
    "ClientSecret": "@Microsoft.KeyVault(SecretUri=https://vault.vault.azure.net/secrets/client-secret/)"
  }
}
```

#### What NOT to do
```json
// ❌ NEVER commit secrets to source control
{
  "AzureAd": {
    "TenantId": "real-tenant-id-here",
    "ClientId": "real-client-id-here", 
    "ClientSecret": "real-secret-here"
  }
}
```

### 2. Network Security

#### Firewall Configuration
```bash
# Allow outbound HTTPS to Microsoft Graph
# Block other unnecessary outbound traffic
=======
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

#### TLS/SSL
- Always use HTTPS in production
- Verify certificate validation is enabled
- Use TLS 1.2 or higher

### 3. Azure AD App Registration Security

#### Required Permissions (Least Privilege)
```
Application Permissions:
- User.ReadWrite.All (only if user management needed)
- Group.ReadWrite.All (only if group management needed)
- Mail.ReadWrite (only if email functionality needed)
- Mail.Send (only if sending emails needed)
- Calendars.ReadWrite (only if calendar functionality needed)
- Team.ReadBasic.All (only if Teams integration needed)
- Files.ReadWrite.All (only if file operations needed)
- Sites.ReadWrite.All (only if SharePoint needed)
- Reports.Read.All (only if reporting needed)
- Application.ReadWrite.All (only if app management needed)
```

#### Security Configuration
1. **Certificate Authentication** (recommended over client secrets):
   ```bash
   # Generate certificate
   openssl req -x509 -newkey rsa:2048 -keyout private.key -out certificate.crt -days 365
   ```

2. **Client Secret Rotation**:
   - Rotate secrets every 90 days maximum
   - Use overlapping secrets during rotation
   - Monitor secret expiration dates

3. **Conditional Access**:
   - Implement conditional access policies
   - Require MFA for admin operations
   - Restrict access by location/device

### 4. Monitoring and Auditing

#### Enable Audit Logs
```csharp
// In appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Graph": "Warning",
      "DarbotGraphMcp.Server": "Information"
    }
  }
}
```

#### Security Monitoring
- Monitor authentication failures
- Track privilege escalation attempts
- Log all administrative actions
- Monitor for unusual API usage patterns

#### Azure AD Sign-in Logs
- Review sign-in logs regularly
- Monitor for suspicious locations
- Check for failed authentication attempts
- Verify application usage patterns

### 5. Data Protection

#### Data Classification
- **Public**: Demo data, documentation
- **Internal**: Non-sensitive organizational data
- **Confidential**: Personal information, business data
- **Restricted**: Highly sensitive information

#### Data Handling
```csharp
// Example: Sanitize sensitive data in logs
_logger.LogInformation("User operation completed for user: {UserId}", 
    userId.Substring(0, 8) + "****");
```

#### Retention Policies
- Implement data retention policies
- Automatically delete old logs
- Archive historical data securely
- Comply with GDPR/privacy requirements

## Security Incident Response

### 1. Credential Compromise
If Azure AD credentials are compromised:

1. **Immediate Actions**:
   ```bash
   # Revoke the compromised secret
   az ad app credential delete --id $CLIENT_ID --key-id $KEY_ID
   
   # Generate new secret
   az ad app credential reset --id $CLIENT_ID
   ```

2. **Investigation**:
   - Review Azure AD audit logs
   - Check for unauthorized API calls
   - Identify affected resources
   - Document the incident

3. **Recovery**:
   - Update application with new credentials
   - Restart MCP server with new configuration
   - Verify normal operation

### 2. Unauthorized Access
If unauthorized access is detected:

1. **Contain**:
   - Disable the application registration
   - Revoke all active sessions
   - Block suspicious IP addresses

2. **Investigate**:
   - Review Graph API usage logs
   - Check for data exfiltration
   - Identify attack vectors
   - Document findings

3. **Recover**:
   - Reset all credentials
   - Review and update permissions
   - Implement additional monitoring
   - Update security policies

### 3. Data Breach
If sensitive data is exposed:

1. **Assess**:
   - Identify what data was accessed
   - Determine the scope of exposure
   - Evaluate potential impact

2. **Notify**:
   - Inform affected users
   - Report to relevant authorities
   - Document the breach

3. **Remediate**:
   - Fix the vulnerability
   - Implement additional controls
   - Monitor for further incidents

## Compliance Considerations

### GDPR Compliance
- Implement data subject rights
- Maintain data processing records
- Ensure lawful basis for processing
- Implement privacy by design

### Industry Standards
- **SOC 2**: Implement security controls
- **ISO 27001**: Follow information security management
- **NIST**: Use cybersecurity framework
- **FedRAMP**: For government deployments

### Microsoft 365 Compliance
- Leverage Microsoft Purview
- Implement data loss prevention
- Use sensitivity labels
- Enable audit logging

## Security Testing

### Regular Security Assessments
1. **Vulnerability Scanning**:
   ```bash
   # Example using npm audit for Node.js dependencies
   npm audit
   
   # Example using dotnet security scan
   dotnet list package --vulnerable
   ```

2. **Penetration Testing**:
   - Test authentication mechanisms
   - Verify input validation
   - Check for privilege escalation
   - Test network security

3. **Code Review**:
   - Review for security vulnerabilities
   - Check credential handling
   - Verify input sanitization
   - Validate error handling

### Automated Security
```yaml
# Example GitHub Actions security workflow
name: Security Scan
on: [push, pull_request]
jobs:
  security:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Run security scan
        run: |
          dotnet list package --vulnerable
          npm audit --audit-level moderate
```

## Contact and Reporting

### Security Issues
Report security vulnerabilities to:
- **Email**: security@example.com (if available)
- **GitHub**: Create a private security advisory
- **Responsible Disclosure**: Follow coordinated disclosure timeline

### Emergency Contacts
- **IT Security Team**: Contact your organization's security team
- **Microsoft Security**: For Microsoft Graph API issues
- **Azure Support**: For Azure AD authentication issues

Remember: Security is an ongoing process, not a one-time setup. Regularly review and update your security posture.

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