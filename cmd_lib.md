# Darbot Graph MCP Command Library

This document provides a comprehensive reference for all Microsoft Graph MCP tools available in the Darbot Graph MCP Server. The tools are organized hierarchically to work within Visual Studio Code's 128 tool limit.

## Overview

The Darbot Graph MCP Server provides **64 comprehensive Microsoft Graph tools** organized into 10 categories:

- **User Management**: 8 tools
- **Group Management**: 8 tools  
- **Email Management**: 8 tools
- **Calendar Management**: 8 tools
- **Teams Management**: 8 tools
- **Files Management**: 4 tools
- **SharePoint**: 3 tools
- **Security**: 5 tools
- **Reports**: 4 tools
- **Applications**: 8 tools

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
- Applications tools: `Application.ReadWrite.All`