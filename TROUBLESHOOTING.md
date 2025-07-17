# Troubleshooting Guide

## Common Issues and Solutions

### Installation Issues

#### NPM Package Not Found
**Error**: `npm ERR! 404 Not Found - GET https://registry.npmjs.org/@darbotlabs/darbot-graph-mcp`

**Solution**: The package may not be published yet. Use manual installation:
```bash
git clone https://github.com/dayour/Darbot-Graph-MCP.git
cd Darbot-Graph-MCP
dotnet build
```

#### .NET SDK Missing
**Error**: `dotnet: command not found`

**Solution**: Install .NET 8.0 SDK:
- **Windows**: Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
- **macOS**: `brew install --cask dotnet`
- **Linux**: Follow [Microsoft's guide](https://docs.microsoft.com/en-us/dotnet/core/install/linux)

### Authentication Issues

#### Invalid Tenant ID
**Error**: `Invalid tenant ID format. Must be a valid GUID.`

**Solution**: Ensure your tenant ID is a valid GUID format:
```
xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
```

To find your tenant ID:
1. Go to [Azure Portal](https://portal.azure.com)
2. Navigate to **Azure Active Directory** > **Properties**
3. Copy the **Tenant ID**

#### Placeholder Tenant ID Security Warning
**Error**: `Security warning: Using placeholder or common tenant ID`

**Solution**: This is a security feature. Use your organization's actual tenant ID, not placeholder values like:
- `00000000-0000-0000-0000-000000000000`
- `11111111-1111-1111-1111-111111111111`

#### Permission Denied Errors
**Error**: `Insufficient privileges to complete the operation`

**Solutions**:
1. **Check Admin Consent**: Ensure admin consent is granted for all required permissions
2. **Verify App Registration**: Confirm your app registration has the correct permissions:
   - `User.ReadWrite.All`
   - `Group.ReadWrite.All`
   - `Mail.ReadWrite`
   - `Mail.Send`
   - `Calendars.ReadWrite`
   - `Team.ReadBasic.All`
   - And others as listed in the main README

3. **Check Token Scope**: Verify your client secret hasn't expired

### Configuration Issues

#### VS Code MCP Not Working
**Issue**: MCP server not appearing in VS Code

**Solutions**:
1. **Restart VS Code Completely**: Close all windows and restart
2. **Check Configuration Path**: Ensure the path in `claude_desktop_config.json` is correct
3. **Verify File Permissions**: Ensure the config file is readable
4. **Check Logs**: Look for MCP-related errors in VS Code developer console

#### Server Won't Start
**Error**: `Failed to start server`

**Solutions**:
1. **Check Port Availability**: Ensure port 5000 is not in use
2. **Verify Dependencies**: Run `dotnet restore` in the server directory
3. **Check Environment Variables**: Verify Azure AD configuration is correct
4. **Review Logs**: Check server output for specific error messages

### Demo Mode Issues

#### Demo Mode Not Working
**Issue**: Server fails to start even in demo mode

**Solutions**:
1. **Clear Configuration**: Ensure no invalid Azure AD credentials are set
2. **Check Build**: Verify the .NET project builds successfully with `dotnet build`
3. **Port Conflicts**: Try a different port by modifying the server configuration

#### Unexpected Demo Data
**Issue**: Getting demo data when expecting real data

**Solutions**:
1. **Verify Credentials**: Ensure all three values are provided: TenantId, ClientId, ClientSecret
2. **Check Environment Variables**: Verify environment variables are correctly set
3. **Restart Server**: After changing credentials, restart the MCP server

### Performance Issues

#### Slow Response Times
**Issue**: Tools taking a long time to respond

**Solutions**:
1. **Check Network**: Verify internet connectivity to Microsoft Graph
2. **Graph API Limits**: You may be hitting rate limits; reduce request frequency
3. **Optimize Queries**: Use filters and pagination for large data sets

#### Memory Usage
**Issue**: High memory consumption

**Solutions**:
1. **Reduce Concurrent Requests**: Limit simultaneous Graph API calls
2. **Use Pagination**: For large result sets, use top/skip parameters
3. **Monitor Logs**: Check for memory leaks in application logs

### Security Considerations

#### Cross-Tenant Security
**Risk**: Creating resources in wrong tenant

**Prevention**:
- Always verify tenant ID before configuration
- Use the built-in tenant validation features
- Test with demo mode first
- Implement tenant-specific naming conventions

#### Credential Management
**Best Practices**:
- Store secrets in Azure Key Vault for production
- Rotate client secrets regularly
- Use environment variables instead of config files
- Monitor authentication logs

#### Network Security
**Recommendations**:
- Use HTTPS in production environments
- Implement firewall rules for outbound Graph API calls
- Monitor network traffic for anomalies
- Use Azure Private Endpoints when possible

### Development Issues

#### Build Failures
**Error**: Build errors in Visual Studio/VS Code

**Solutions**:
1. **Clean Solution**: `dotnet clean && dotnet restore && dotnet build`
2. **Update Dependencies**: Check for NuGet package updates
3. **Check Target Framework**: Ensure .NET 8.0 is installed
4. **IDE Restart**: Restart your development environment

#### Testing Issues
**Issue**: Unable to test Graph API calls

**Solutions**:
1. **Use Graph Explorer**: Test API calls at [developer.microsoft.com/graph/graph-explorer](https://developer.microsoft.com/graph/graph-explorer)
2. **Check Permissions**: Verify your app has required permissions
3. **Use Demo Mode**: Test server functionality without real credentials
4. **Monitor Logs**: Enable detailed logging to trace issues

## Getting Help

### Log Collection
When reporting issues, include:
1. **Server logs**: Output from the MCP server startup
2. **Configuration**: Your MCP configuration (without secrets)
3. **Environment**: OS, .NET version, Node.js version
4. **Error messages**: Complete error text and stack traces

### Support Channels
1. **GitHub Issues**: [github.com/dayour/Darbot-Graph-MCP/issues](https://github.com/dayour/Darbot-Graph-MCP/issues)
2. **Documentation**: [Main README](../README.md)
3. **Microsoft Graph**: [Microsoft Graph documentation](https://docs.microsoft.com/graph/)

### Emergency Recovery
If the server becomes completely unresponsive:
1. **Kill Process**: Stop the dotnet process
2. **Clear State**: Delete any temporary files
3. **Reset Configuration**: Revert to known-good configuration
4. **Restart Fresh**: Start with demo mode to verify basic functionality