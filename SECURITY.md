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