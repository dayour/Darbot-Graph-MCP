# Darbot Graph MCP Command Library

This document provides a comprehensive reference for all Microsoft Graph MCP tools available in the Darbot Graph MCP Server. The tools are organized hierarchically to work within Visual Studio Code's 128 tool limit and provide complete coverage of Microsoft Graph API operations.

## Overview

The Darbot Graph MCP Server provides **64+ comprehensive Microsoft Graph tools** organized into 10 categories, built on the official Microsoft Graph SDKs:

- **Foundation**: [Microsoft Graph .NET SDK](https://github.com/microsoftgraph/msgraph-sdk-dotnet) v5.58.0+
- **Preview APIs**: [Microsoft Graph .NET Beta SDK](https://github.com/microsoftgraph/msgraph-beta-sdk-dotnet) v5.59.0+
- **Authentication**: [Azure.Identity](https://docs.microsoft.com/en-us/dotnet/api/azure.identity) for secure credential management
- **Reference**: [Microsoft Graph PowerShell](https://learn.microsoft.com/en-us/powershell/microsoftgraph/get-started) for comprehensive API coverage patterns

### Tool Distribution by Category

- **User Management**: 8 tools - Complete user lifecycle management
- **Group Management**: 8 tools - Security and distribution group operations  
- **Email Management**: 8 tools - Exchange Online mail operations
- **Calendar Management**: 8 tools - Outlook calendar and scheduling
- **Teams Management**: 8 tools - Microsoft Teams collaboration
- **Files Management**: 4 tools - OneDrive and SharePoint file operations
- **SharePoint**: 3 tools - Sites and content management
- **Security**: 5 tools - Identity protection and audit logging
- **Reports**: 4 tools - Usage analytics and activity monitoring
- **Applications**: 8 tools - Azure AD application management

**Total: 64 tools** providing comprehensive Microsoft Graph API coverage

## Naming Convention

All tools follow the hierarchical naming pattern: `darbot-graph-{category}-{action}`

Examples:
- `darbot-graph-users-list` - List users
- `darbot-graph-groups-create` - Create a group
- `darbot-graph-mail-send` - Send an email

## Tool Categories

### 1. User Management (`darbot-graph-users-*`)

#### `darbot-graph-users-list`
Get a list of users from Microsoft Graph with advanced filtering.

**Parameters:**
- `top` (integer): Number of users to return (max 999)
- `filter` (string): OData filter expression
- `search` (string): Search query

**Example:**
```json
{
  "name": "darbot-graph-users-list",
  "arguments": {
    "top": 10,
    "filter": "department eq 'IT'"
  }
}
```

#### `darbot-graph-users-get`
Get detailed information about a specific user by ID or UPN.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name

#### `darbot-graph-users-create`
Create a new user account with comprehensive settings.

**Parameters:**
- `displayName` (string, required): Display name of the user
- `userPrincipalName` (string, required): User principal name (email)
- `mailNickname` (string, required): Mail nickname
- `password` (string, required): Temporary password
- `jobTitle` (string): Job title
- `department` (string): Department

#### `darbot-graph-users-update`
Update user properties and settings.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name
- `displayName` (string): Display name of the user
- `jobTitle` (string): Job title
- `department` (string): Department
- `officeLocation` (string): Office location

#### `darbot-graph-users-delete`
Remove a user from the directory.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name

#### `darbot-graph-users-password-reset`
Reset user password and force change on next sign-in.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name
- `newPassword` (string, required): New password
- `forceChange` (boolean): Force password change on next sign-in

#### `darbot-graph-users-manager-get`
Get user's manager information.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name

#### `darbot-graph-users-manager-set`
Assign a manager to a user.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name
- `managerId` (string, required): Manager's User ID or UPN

### 2. Group Management (`darbot-graph-groups-*`)

#### `darbot-graph-groups-list`
Get a list of groups with advanced filtering.

**Parameters:**
- `top` (integer): Number of groups to return (max 999)
- `filter` (string): OData filter expression

#### `darbot-graph-groups-get`
Get detailed information about a specific group.

**Parameters:**
- `groupId` (string, required): Group ID

#### `darbot-graph-groups-create`
Create security or Microsoft 365 groups.

**Parameters:**
- `displayName` (string, required): Display name of the group
- `mailNickname` (string, required): Mail nickname
- `description` (string): Group description
- `groupType` (string): Type of group (Security, Microsoft365)

#### `darbot-graph-groups-update`
Update group properties and settings.

**Parameters:**
- `groupId` (string, required): Group ID
- `displayName` (string): Display name of the group
- `description` (string): Group description

#### `darbot-graph-groups-delete`
Remove a group from the directory.

**Parameters:**
- `groupId` (string, required): Group ID

#### `darbot-graph-groups-members-add`
Add members to a group.

**Parameters:**
- `groupId` (string, required): Group ID
- `userId` (string, required): User ID to add to the group

#### `darbot-graph-groups-members-remove`
Remove members from a group.

**Parameters:**
- `groupId` (string, required): Group ID
- `userId` (string, required): User ID to remove from the group

#### `darbot-graph-groups-members-list`
List all group members.

**Parameters:**
- `groupId` (string, required): Group ID

### 3. Email Management (`darbot-graph-mail-*`)

#### `darbot-graph-mail-send`
Send emails with advanced formatting and attachments.

**Parameters:**
- `to` (array of strings, required): Recipient email addresses
- `cc` (array of strings): CC recipient email addresses
- `bcc` (array of strings): BCC recipient email addresses
- `subject` (string, required): Email subject
- `body` (string, required): Email body content
- `bodyType` (string): Body content type (Text or Html)
- `importance` (string): Email importance (Low, Normal, High)

#### `darbot-graph-mail-settings-get`
Retrieve user mailbox settings.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name

#### `darbot-graph-mail-folders-list`
List mail folders and subfolders.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name

#### `darbot-graph-mail-folders-create`
Create new mail folders.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name
- `displayName` (string, required): Folder display name
- `parentFolderId` (string): Parent folder ID (optional)

#### `darbot-graph-mail-messages-list`
Retrieve messages with filtering.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name
- `folderId` (string): Folder ID (optional)
- `top` (integer): Number of messages to return
- `filter` (string): OData filter expression

#### `darbot-graph-mail-messages-reply`
Reply to email messages.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name
- `messageId` (string, required): Message ID to reply to
- `comment` (string, required): Reply content

#### `darbot-graph-mail-messages-forward`
Forward email messages.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name
- `messageId` (string, required): Message ID to forward
- `to` (array of strings, required): Recipient email addresses
- `comment` (string): Forward comment

#### `darbot-graph-mail-messages-move`
Move messages between folders.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name
- `messageId` (string, required): Message ID to move
- `destinationFolderId` (string, required): Destination folder ID

### 4. Calendar Management (`darbot-graph-calendar-*`)

#### `darbot-graph-calendar-events-list`
Retrieve calendar events with advanced filtering.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name
- `startTime` (string): Start time (ISO 8601)
- `endTime` (string): End time (ISO 8601)
- `top` (integer): Number of events to return

#### `darbot-graph-calendar-events-create`
Create events with attendees and recurrence.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name
- `subject` (string, required): Event subject
- `body` (string): Event body content
- `startTime` (string, required): Start time (ISO 8601)
- `endTime` (string, required): End time (ISO 8601)
- `attendees` (array of strings): Attendee email addresses
- `location` (string): Event location

#### `darbot-graph-calendar-events-update`
Update existing calendar events.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name
- `eventId` (string, required): Event ID
- `subject` (string): Event subject
- `body` (string): Event body content
- `startTime` (string): Start time (ISO 8601)
- `endTime` (string): End time (ISO 8601)

#### `darbot-graph-calendar-events-delete`
Remove calendar events.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name
- `eventId` (string, required): Event ID

#### `darbot-graph-calendar-list`
List user calendars.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name

#### `darbot-graph-calendar-create`
Create new calendars.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name
- `name` (string, required): Calendar name
- `color` (string): Calendar color

#### `darbot-graph-calendar-events-accept`
Accept meeting invitations.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name
- `eventId` (string, required): Event ID
- `comment` (string): Optional comment

#### `darbot-graph-calendar-events-decline`
Decline meeting invitations.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name
- `eventId` (string, required): Event ID
- `comment` (string): Optional comment

### 5. Teams Management (`darbot-graph-teams-*`)

#### `darbot-graph-teams-list`
Get Microsoft Teams user is member of.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name

#### `darbot-graph-teams-channels-list`
List channels in a team.

**Parameters:**
- `teamId` (string, required): Team ID

#### `darbot-graph-teams-channels-create`
Create new team channels.

**Parameters:**
- `teamId` (string, required): Team ID
- `displayName` (string, required): Channel display name
- `description` (string): Channel description
- `channelType` (string): Channel type (Standard, Private)

#### `darbot-graph-teams-messages-list`
Retrieve channel messages.

**Parameters:**
- `teamId` (string, required): Team ID
- `channelId` (string, required): Channel ID
- `top` (integer): Number of messages to return

#### `darbot-graph-teams-messages-send`
Send messages to team channels.

**Parameters:**
- `teamId` (string, required): Team ID
- `channelId` (string, required): Channel ID
- `content` (string, required): Message content
- `contentType` (string): Content type (Text or Html)

#### `darbot-graph-teams-messages-reply`
Reply to channel messages.

**Parameters:**
- `teamId` (string, required): Team ID
- `channelId` (string, required): Channel ID
- `messageId` (string, required): Message ID to reply to
- `content` (string, required): Reply content

#### `darbot-graph-teams-members-list`
List team members.

**Parameters:**
- `teamId` (string, required): Team ID

#### `darbot-graph-teams-members-add`
Add members to teams.

**Parameters:**
- `teamId` (string, required): Team ID
- `userId` (string, required): User ID to add to the team
- `role` (string): Member role (Owner, Member)

### 6. Files Management (`darbot-graph-files-*`)

#### `darbot-graph-files-list`
List OneDrive files and folders.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name
- `folderId` (string): Folder ID (optional, defaults to root)
- `top` (integer): Number of items to return

#### `darbot-graph-files-upload`
Upload files to OneDrive.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name
- `fileName` (string, required): File name
- `content` (string, required): File content (base64 encoded)
- `folderId` (string): Destination folder ID (optional)

#### `darbot-graph-files-download`
Download files from OneDrive.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name
- `fileId` (string, required): File ID

#### `darbot-graph-files-share`
Create sharing links for files.

**Parameters:**
- `userId` (string, required): User ID or User Principal Name
- `fileId` (string, required): File ID
- `type` (string, required): Share type (View, Edit)
- `scope` (string): Share scope (Anonymous, Organization)

### 7. SharePoint (`darbot-graph-sharepoint-*`)

#### `darbot-graph-sharepoint-sites-list`
List SharePoint sites.

**Parameters:**
- `search` (string): Search query for sites
- `top` (integer): Number of sites to return

#### `darbot-graph-sharepoint-lists-list`
Get lists from SharePoint sites.

**Parameters:**
- `siteId` (string, required): SharePoint site ID

#### `darbot-graph-sharepoint-items-list`
Retrieve items from SharePoint lists.

**Parameters:**
- `siteId` (string, required): SharePoint site ID
- `listId` (string, required): List ID
- `top` (integer): Number of items to return

### 8. Security (`darbot-graph-security-*`)

#### `darbot-graph-security-signins-list`
Retrieve user sign-in logs.

**Parameters:**
- `userId` (string): User ID (optional)
- `top` (integer): Number of logs to return
- `filter` (string): OData filter expression

#### `darbot-graph-security-audit-list`
Get directory audit logs.

**Parameters:**
- `top` (integer): Number of logs to return
- `filter` (string): OData filter expression

#### `darbot-graph-security-risks-list`
List users flagged for risk.

**Parameters:**
- `top` (integer): Number of users to return
- `filter` (string): OData filter expression

#### `darbot-graph-security-risks-confirm`
Confirm or dismiss risky users.

**Parameters:**
- `userId` (string, required): User ID
- `action` (string, required): Action to take (Confirm, Dismiss)

#### `darbot-graph-security-policies-list`
List conditional access policies.

**Parameters:**
- `top` (integer): Number of policies to return

### 9. Reports (`darbot-graph-reports-*`)

#### `darbot-graph-reports-usage`
Get Microsoft 365 usage reports.

**Parameters:**
- `reportType` (string, required): Type of report (Overview, UserActivity, DeviceUsage)
- `period` (string): Report period (D7, D30, D90, D180)

#### `darbot-graph-reports-teams`
Get Teams activity reports.

**Parameters:**
- `period` (string, required): Report period (D7, D30, D90, D180)

#### `darbot-graph-reports-email`
Get email activity reports.

**Parameters:**
- `period` (string, required): Report period (D7, D30, D90, D180)

#### `darbot-graph-reports-sharepoint`
Get SharePoint activity reports.

**Parameters:**
- `period` (string, required): Report period (D7, D30, D90, D180)

### 10. Applications (`darbot-graph-apps-*`)

#### `darbot-graph-apps-list`
List applications in the directory.

**Parameters:**
- `top` (integer): Number of applications to return
- `filter` (string): OData filter expression

#### `darbot-graph-apps-get`
Get details of a specific application.

**Parameters:**
- `appId` (string, required): Application ID

#### `darbot-graph-apps-create`
Create a new application registration.

**Parameters:**
- `displayName` (string, required): Application display name
- `signInAudience` (string): Sign-in audience

#### `darbot-graph-apps-update`
Update application properties.

**Parameters:**
- `appId` (string, required): Application ID
- `displayName` (string): Application display name
- `description` (string): Application description

#### `darbot-graph-apps-delete`
Delete an application.

**Parameters:**
- `appId` (string, required): Application ID

#### `darbot-graph-apps-permissions-list`
List application permissions.

**Parameters:**
- `appId` (string, required): Application ID

#### `darbot-graph-apps-permissions-grant`
Grant permissions to an application.

**Parameters:**
- `appId` (string, required): Application ID
- `permissions` (array of strings, required): Permission scopes to grant

#### `darbot-graph-apps-secrets-create`
Create application client secret.

**Parameters:**
- `appId` (string, required): Application ID
- `displayName` (string, required): Secret display name
- `endDateTime` (string): Expiration date (ISO 8601)

## Usage Examples

### Basic User Listing
```json
{
  "name": "darbot-graph-users-list",
  "arguments": {
    "top": 5
  }
}
```

### Filtered Group Search
```json
{
  "name": "darbot-graph-groups-list",
  "arguments": {
    "filter": "startswith(displayName,'IT')",
    "top": 10
  }
}
```

### Sending Email
```json
{
  "name": "darbot-graph-mail-send",
  "arguments": {
    "to": ["user@example.com"],
    "subject": "Test Email",
    "body": "<h1>Hello World</h1>",
    "bodyType": "Html"
  }
}
```

### Creating Calendar Event
```json
{
  "name": "darbot-graph-calendar-events-create",
  "arguments": {
    "userId": "user@example.com",
    "subject": "Team Meeting",
    "startTime": "2024-01-15T14:00:00",
    "endTime": "2024-01-15T15:00:00",
    "attendees": ["colleague@example.com"],
    "location": "Conference Room A"
  }
}
```

## Error Handling

All tools return structured responses with error information when applicable:

```json
{
  "success": false,
  "error": "Error message",
  "details": "Detailed error information"
}
```

For demo mode (when Azure AD is not configured):

```json
{
  "success": true,
  "demo": true,
  "message": "Demo mode - Azure AD not configured",
  "data": "Sample data"
}
```

## Authentication Requirements

All tools require proper Azure AD authentication with appropriate Microsoft Graph permissions. See the main README.md for detailed setup instructions.

Required permissions vary by tool category but generally include:
- User tools: `User.ReadWrite.All`
- Group tools: `Group.ReadWrite.All`
- Mail tools: `Mail.ReadWrite`, `Mail.Send`
- Calendar tools: `Calendars.ReadWrite`
- Teams tools: `Team.ReadBasic.All`, `TeamSettings.ReadWrite.All`
- Files tools: `Files.ReadWrite.All`
- SharePoint tools: `Sites.ReadWrite.All`
- Security tools: `AuditLog.Read.All`, `SecurityEvents.Read.All`
- Reports tools: `Reports.Read.All`

## Microsoft Graph API Coverage

This MCP server is designed to be the **ultimate extensible MCP server for any and all Microsoft Graph API needs**, based on comprehensive analysis of Microsoft Graph SDK capabilities.

### Current Implementation Status ✅

**Core Microsoft 365 Services (64 Tools)**
- **Identity & Access Management**: Complete user and group lifecycle operations
- **Exchange Online**: Full email management, folders, and messaging
- **Outlook Calendar**: Event creation, management, and scheduling
- **Microsoft Teams**: Team collaboration, channels, and messaging
- **OneDrive & SharePoint**: File operations and content management
- **Azure AD Applications**: App registration and permission management
- **Security & Compliance**: Audit logs, risk detection, and monitoring
- **Usage Analytics**: Comprehensive reporting across all services

### Microsoft Graph SDK Foundation

Built on official Microsoft SDKs following best practices:

#### Primary Dependencies
```xml
<PackageReference Include="Microsoft.Graph" Version="5.58.0" />
<PackageReference Include="Microsoft.Graph.Beta" Version="5.59.0-preview" />
<PackageReference Include="Azure.Identity" Version="1.12.0" />
```

#### SDK Compatibility
- **[Microsoft.Graph SDK](https://github.com/microsoftgraph/msgraph-sdk-dotnet)**: Production v1.0 APIs
- **[Microsoft.Graph.Beta SDK](https://github.com/microsoftgraph/msgraph-beta-sdk-dotnet)**: Preview APIs
- **[Microsoft Graph PowerShell](https://learn.microsoft.com/en-us/powershell/microsoftgraph/get-started)**: Reference for comprehensive coverage

### Extensible Architecture for Future Growth 🚀

The server architecture supports expansion to cover **all Microsoft Graph APIs**:

#### Ready for Implementation Categories

**Device & Endpoint Management**
```
darbot-graph-devices-list          # List managed devices
darbot-graph-devices-compliance    # Check compliance status
darbot-graph-intune-policies-list  # List Intune policies
darbot-graph-devices-wipe          # Remote device wipe
```

**Identity Protection & Conditional Access**
```
darbot-graph-identity-policies-list    # List conditional access policies
darbot-graph-identity-risks-assess     # Assess identity risks
darbot-graph-identity-locations-list   # List named locations
darbot-graph-identity-mfa-enforce      # Enforce MFA policies
```

**Directory Management**
```
darbot-graph-directory-units-list      # List administrative units
darbot-graph-directory-contacts-list   # List organizational contacts
darbot-graph-directory-domains-list    # List verified domains
darbot-graph-directory-roles-list      # List directory roles
```

**Personal Information Management**
```
darbot-graph-contacts-list             # List personal contacts
darbot-graph-contacts-create          # Create contacts
darbot-graph-tasks-list               # List Outlook tasks
darbot-graph-notes-list               # List personal notes
```

**Microsoft Planner & Project**
```
darbot-graph-planner-plans-list       # List Planner plans
darbot-graph-planner-tasks-create     # Create Planner tasks
darbot-graph-planner-buckets-list     # List plan buckets
darbot-graph-project-tasks-list       # List Project tasks
```

**OneNote Integration**
```
darbot-graph-onenote-notebooks-list   # List OneNote notebooks
darbot-graph-onenote-sections-create  # Create notebook sections
darbot-graph-onenote-pages-create     # Create OneNote pages
darbot-graph-onenote-search           # Search OneNote content
```

**Advanced SharePoint & Content**
```
darbot-graph-sharepoint-permissions-list  # List site permissions
darbot-graph-sharepoint-columns-create    # Create site columns
darbot-graph-sharepoint-workflows-list    # List site workflows
darbot-graph-sharepoint-search            # Search SharePoint content
```

**License & Subscription Management**
```
darbot-graph-licenses-list            # List available licenses
darbot-graph-licenses-assign          # Assign user licenses
darbot-graph-subscriptions-list       # List organization subscriptions
darbot-graph-usage-quotas-check       # Check service quotas
```

**External Identities & B2B**
```
darbot-graph-b2b-invitations-send     # Send B2B invitations
darbot-graph-b2b-users-list           # List external users
darbot-graph-b2c-policies-list        # List B2C policies
darbot-graph-partners-list            # List partner organizations
```

**Microsoft Bookings**
```
darbot-graph-bookings-businesses-list # List booking businesses
darbot-graph-bookings-appointments-create # Create appointments
darbot-graph-bookings-services-list   # List booking services
darbot-graph-bookings-staff-list      # List booking staff
```

**Education APIs**
```
darbot-graph-education-classes-list   # List education classes
darbot-graph-education-assignments-create # Create assignments
darbot-graph-education-users-list     # List education users
darbot-graph-education-schools-list   # List schools
```

**Compliance & eDiscovery**
```
darbot-graph-compliance-policies-list # List DLP policies
darbot-graph-retention-policies-list  # List retention policies
darbot-graph-ediscovery-cases-list    # List eDiscovery cases
darbot-graph-labels-list              # List sensitivity labels
```

**Microsoft Search**
```
darbot-graph-search-query             # Perform Microsoft Search
darbot-graph-search-bookmarks-list    # List search bookmarks
darbot-graph-search-acronyms-list     # List search acronyms
darbot-graph-search-qnas-list         # List Q&A results
```

**Cloud Communications**
```
darbot-graph-calls-list               # List active calls
darbot-graph-meetings-create          # Create online meetings
darbot-graph-presence-get             # Get user presence
darbot-graph-voicemail-list           # List voicemail messages
```

**Universal Print**
```
darbot-graph-print-printers-list      # List universal printers
darbot-graph-print-jobs-list          # List print jobs
darbot-graph-print-shares-list        # List printer shares
darbot-graph-print-connectors-list    # List print connectors
```

### Implementation Guidelines

#### Adding New Categories
1. **Define Tool Schema** in `ToolCategories.cs`:
```csharp
public static List<object> GetNewCategoryTools()
{
    return new List<object>
    {
        new
        {
            name = "darbot-graph-category-action",
            description = "Tool description",
            inputSchema = new { /* JSON schema */ }
        }
    };
}
```

2. **Implement Service Methods** in `GraphServiceEnhanced.cs`:
```csharp
private async Task<object> NewCategoryActionAsync(JsonElement? arguments)
{
    try
    {
        // Use _graphClient for v1.0 APIs
        // Use _betaGraphClient for Beta APIs
        return new { success = true, data = result };
    }
    catch (Exception ex)
    {
        return new { success = false, error = ex.Message };
    }
}
```

3. **Add Tool Routing**:
```csharp
"darbot-graph-category-action" => await NewCategoryActionAsync(arguments),
```

4. **Register Tool Category**:
```csharp
tools.AddRange(ToolCategories.GetNewCategoryTools());
```

### Microsoft Graph API Reference

#### Official Documentation
- **[Graph API Reference](https://docs.microsoft.com/en-us/graph/api/overview)**: Complete API documentation
- **[Graph Explorer](https://developer.microsoft.com/graph/graph-explorer)**: Interactive API testing
- **[Graph Permissions](https://docs.microsoft.com/en-us/graph/permissions-reference)**: Permission requirements
- **[Graph SDKs](https://docs.microsoft.com/en-us/graph/sdks/sdks-overview)**: Official SDK documentation

#### PowerShell Graph Module Reference
The [Microsoft Graph PowerShell module](https://learn.microsoft.com/en-us/powershell/microsoftgraph/get-started) provides comprehensive coverage patterns that can be implemented in this MCP server:

```powershell
# Examples of Graph PowerShell coverage this server can implement
Get-MgUser                    # darbot-graph-users-list
Get-MgGroup                   # darbot-graph-groups-list  
Get-MgDevice                  # darbot-graph-devices-list (future)
Get-MgApplication             # darbot-graph-apps-list
Get-MgDirectoryRole           # darbot-graph-directory-roles-list (future)
Get-MgPlannerPlan            # darbot-graph-planner-plans-list (future)
```

### Contribution & Extension

This server is designed as the **definitive Microsoft Graph MCP integration**. To contribute:

1. **Research**: Use [Graph Explorer](https://developer.microsoft.com/graph/graph-explorer) to test APIs
2. **Design**: Follow the hierarchical naming convention
3. **Implement**: Use the established patterns in existing tools
4. **Test**: Validate against both v1.0 and Beta APIs where applicable
5. **Document**: Update this reference with new tool descriptions

The goal is comprehensive coverage of all Microsoft Graph capabilities, making this the ultimate extensible MCP server for any and all Graph API needs.
- Applications tools: `Application.ReadWrite.All`