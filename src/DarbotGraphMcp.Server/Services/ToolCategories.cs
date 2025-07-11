using System.Text.Json;

namespace DarbotGraphMcp.Server.Services;

public static class ToolCategories
{
    public static List<object> GetUserManagementTools()
    {
        return new List<object>
        {
            new
            {
                name = "darbot-graph-users-list",
                description = "Get a list of users from Microsoft Graph with advanced filtering",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        top = new { type = "integer", description = "Number of users to return (max 999)" },
                        filter = new { type = "string", description = "OData filter expression" },
                        search = new { type = "string", description = "Search query" }
                    }
                }
            },
            new
            {
                name = "darbot-graph-users-get",
                description = "Get detailed information about a specific user by ID or UPN",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" }
                    },
                    required = new[] { "userId" }
                }
            },
            new
            {
                name = "darbot-graph-users-create",
                description = "Create a new user account with comprehensive settings",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        displayName = new { type = "string", description = "Display name of the user" },
                        userPrincipalName = new { type = "string", description = "User principal name (email)" },
                        mailNickname = new { type = "string", description = "Mail nickname" },
                        password = new { type = "string", description = "Temporary password" },
                        jobTitle = new { type = "string", description = "Job title" },
                        department = new { type = "string", description = "Department" }
                    },
                    required = new[] { "displayName", "userPrincipalName", "mailNickname", "password" }
                }
            },
            new
            {
                name = "darbot-graph-users-update",
                description = "Update user properties and settings",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        displayName = new { type = "string", description = "Display name of the user" },
                        jobTitle = new { type = "string", description = "Job title" },
                        department = new { type = "string", description = "Department" },
                        officeLocation = new { type = "string", description = "Office location" }
                    },
                    required = new[] { "userId" }
                }
            },
            new
            {
                name = "darbot-graph-users-delete",
                description = "Remove a user from the directory",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" }
                    },
                    required = new[] { "userId" }
                }
            },
            new
            {
                name = "darbot-graph-users-password-reset",
                description = "Reset user password and force change on next sign-in",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        newPassword = new { type = "string", description = "New password" },
                        forceChange = new { type = "boolean", description = "Force password change on next sign-in" }
                    },
                    required = new[] { "userId", "newPassword" }
                }
            },
            new
            {
                name = "darbot-graph-users-manager-get",
                description = "Get user's manager information",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" }
                    },
                    required = new[] { "userId" }
                }
            },
            new
            {
                name = "darbot-graph-users-manager-set",
                description = "Assign a manager to a user",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        managerId = new { type = "string", description = "Manager's User ID or UPN" }
                    },
                    required = new[] { "userId", "managerId" }
                }
            }
        };
    }

    public static List<object> GetGroupManagementTools()
    {
        return new List<object>
        {
            new
            {
                name = "darbot-graph-groups-list",
                description = "Get a list of groups with advanced filtering",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        top = new { type = "integer", description = "Number of groups to return (max 999)" },
                        filter = new { type = "string", description = "OData filter expression" }
                    }
                }
            },
            new
            {
                name = "darbot-graph-groups-get",
                description = "Get detailed information about a specific group",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        groupId = new { type = "string", description = "Group ID" }
                    },
                    required = new[] { "groupId" }
                }
            },
            new
            {
                name = "darbot-graph-groups-create",
                description = "Create security or Microsoft 365 groups",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        displayName = new { type = "string", description = "Display name of the group" },
                        mailNickname = new { type = "string", description = "Mail nickname" },
                        description = new { type = "string", description = "Group description" },
                        groupType = new { type = "string", description = "Type of group (Security, Microsoft365)" }
                    },
                    required = new[] { "displayName", "mailNickname" }
                }
            },
            new
            {
                name = "darbot-graph-groups-update",
                description = "Update group properties and settings",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        groupId = new { type = "string", description = "Group ID" },
                        displayName = new { type = "string", description = "Display name of the group" },
                        description = new { type = "string", description = "Group description" }
                    },
                    required = new[] { "groupId" }
                }
            },
            new
            {
                name = "darbot-graph-groups-delete",
                description = "Remove a group from the directory",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        groupId = new { type = "string", description = "Group ID" }
                    },
                    required = new[] { "groupId" }
                }
            },
            new
            {
                name = "darbot-graph-groups-members-add",
                description = "Add members to a group",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        groupId = new { type = "string", description = "Group ID" },
                        userId = new { type = "string", description = "User ID to add to the group" }
                    },
                    required = new[] { "groupId", "userId" }
                }
            },
            new
            {
                name = "darbot-graph-groups-members-remove",
                description = "Remove members from a group",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        groupId = new { type = "string", description = "Group ID" },
                        userId = new { type = "string", description = "User ID to remove from the group" }
                    },
                    required = new[] { "groupId", "userId" }
                }
            },
            new
            {
                name = "darbot-graph-groups-members-list",
                description = "List all group members",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        groupId = new { type = "string", description = "Group ID" }
                    },
                    required = new[] { "groupId" }
                }
            }
        };
    }

    public static List<object> GetEmailManagementTools()
    {
        return new List<object>
        {
            new
            {
                name = "darbot-graph-mail-send",
                description = "Send emails with advanced formatting and attachments",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        to = new { type = "array", items = new { type = "string" }, description = "Recipient email addresses" },
                        cc = new { type = "array", items = new { type = "string" }, description = "CC recipient email addresses" },
                        bcc = new { type = "array", items = new { type = "string" }, description = "BCC recipient email addresses" },
                        subject = new { type = "string", description = "Email subject" },
                        body = new { type = "string", description = "Email body content" },
                        bodyType = new { type = "string", description = "Body content type (Text or Html)", @enum = new[] { "Text", "Html" } },
                        importance = new { type = "string", description = "Email importance (Low, Normal, High)", @enum = new[] { "Low", "Normal", "High" } }
                    },
                    required = new[] { "to", "subject", "body" }
                }
            },
            new
            {
                name = "darbot-graph-mail-settings-get",
                description = "Retrieve user mailbox settings",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" }
                    },
                    required = new[] { "userId" }
                }
            },
            new
            {
                name = "darbot-graph-mail-folders-list",
                description = "List mail folders and subfolders",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" }
                    },
                    required = new[] { "userId" }
                }
            },
            new
            {
                name = "darbot-graph-mail-folders-create",
                description = "Create new mail folders",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        displayName = new { type = "string", description = "Folder display name" },
                        parentFolderId = new { type = "string", description = "Parent folder ID (optional)" }
                    },
                    required = new[] { "userId", "displayName" }
                }
            },
            new
            {
                name = "darbot-graph-mail-messages-list",
                description = "Retrieve messages with filtering",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        folderId = new { type = "string", description = "Folder ID (optional)" },
                        top = new { type = "integer", description = "Number of messages to return" },
                        filter = new { type = "string", description = "OData filter expression" }
                    },
                    required = new[] { "userId" }
                }
            },
            new
            {
                name = "darbot-graph-mail-messages-reply",
                description = "Reply to email messages",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        messageId = new { type = "string", description = "Message ID to reply to" },
                        comment = new { type = "string", description = "Reply content" }
                    },
                    required = new[] { "userId", "messageId", "comment" }
                }
            },
            new
            {
                name = "darbot-graph-mail-messages-forward",
                description = "Forward email messages",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        messageId = new { type = "string", description = "Message ID to forward" },
                        to = new { type = "array", items = new { type = "string" }, description = "Recipient email addresses" },
                        comment = new { type = "string", description = "Forward comment" }
                    },
                    required = new[] { "userId", "messageId", "to" }
                }
            },
            new
            {
                name = "darbot-graph-mail-messages-move",
                description = "Move messages between folders",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        messageId = new { type = "string", description = "Message ID to move" },
                        destinationFolderId = new { type = "string", description = "Destination folder ID" }
                    },
                    required = new[] { "userId", "messageId", "destinationFolderId" }
                }
            }
        };
    }

    public static List<object> GetCalendarManagementTools()
    {
        return new List<object>
        {
            new
            {
                name = "darbot-graph-calendar-events-list",
                description = "Retrieve calendar events with advanced filtering",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        startTime = new { type = "string", description = "Start time (ISO 8601)" },
                        endTime = new { type = "string", description = "End time (ISO 8601)" },
                        top = new { type = "integer", description = "Number of events to return" }
                    },
                    required = new[] { "userId" }
                }
            },
            new
            {
                name = "darbot-graph-calendar-events-create",
                description = "Create events with attendees and recurrence",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        subject = new { type = "string", description = "Event subject" },
                        body = new { type = "string", description = "Event body content" },
                        startTime = new { type = "string", description = "Start time (ISO 8601)" },
                        endTime = new { type = "string", description = "End time (ISO 8601)" },
                        attendees = new { type = "array", items = new { type = "string" }, description = "Attendee email addresses" },
                        location = new { type = "string", description = "Event location" }
                    },
                    required = new[] { "userId", "subject", "startTime", "endTime" }
                }
            },
            new
            {
                name = "darbot-graph-calendar-events-update",
                description = "Update existing calendar events",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        eventId = new { type = "string", description = "Event ID" },
                        subject = new { type = "string", description = "Event subject" },
                        body = new { type = "string", description = "Event body content" },
                        startTime = new { type = "string", description = "Start time (ISO 8601)" },
                        endTime = new { type = "string", description = "End time (ISO 8601)" }
                    },
                    required = new[] { "userId", "eventId" }
                }
            },
            new
            {
                name = "darbot-graph-calendar-events-delete",
                description = "Remove calendar events",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        eventId = new { type = "string", description = "Event ID" }
                    },
                    required = new[] { "userId", "eventId" }
                }
            },
            new
            {
                name = "darbot-graph-calendar-list",
                description = "List user calendars",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" }
                    },
                    required = new[] { "userId" }
                }
            },
            new
            {
                name = "darbot-graph-calendar-create",
                description = "Create new calendars",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        name = new { type = "string", description = "Calendar name" },
                        color = new { type = "string", description = "Calendar color" }
                    },
                    required = new[] { "userId", "name" }
                }
            },
            new
            {
                name = "darbot-graph-calendar-events-accept",
                description = "Accept meeting invitations",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        eventId = new { type = "string", description = "Event ID" },
                        comment = new { type = "string", description = "Optional comment" }
                    },
                    required = new[] { "userId", "eventId" }
                }
            },
            new
            {
                name = "darbot-graph-calendar-events-decline",
                description = "Decline meeting invitations",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        eventId = new { type = "string", description = "Event ID" },
                        comment = new { type = "string", description = "Optional comment" }
                    },
                    required = new[] { "userId", "eventId" }
                }
            }
        };
    }

    public static List<object> GetTeamsManagementTools()
    {
        return new List<object>
        {
            new
            {
                name = "darbot-graph-teams-list",
                description = "Get Microsoft Teams user is member of",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" }
                    },
                    required = new[] { "userId" }
                }
            },
            new
            {
                name = "darbot-graph-teams-channels-list",
                description = "List channels in a team",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        teamId = new { type = "string", description = "Team ID" }
                    },
                    required = new[] { "teamId" }
                }
            },
            new
            {
                name = "darbot-graph-teams-channels-create",
                description = "Create new team channels",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        teamId = new { type = "string", description = "Team ID" },
                        displayName = new { type = "string", description = "Channel display name" },
                        description = new { type = "string", description = "Channel description" },
                        channelType = new { type = "string", description = "Channel type (Standard, Private)", @enum = new[] { "Standard", "Private" } }
                    },
                    required = new[] { "teamId", "displayName" }
                }
            },
            new
            {
                name = "darbot-graph-teams-messages-list",
                description = "Retrieve channel messages",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        teamId = new { type = "string", description = "Team ID" },
                        channelId = new { type = "string", description = "Channel ID" },
                        top = new { type = "integer", description = "Number of messages to return" }
                    },
                    required = new[] { "teamId", "channelId" }
                }
            },
            new
            {
                name = "darbot-graph-teams-messages-send",
                description = "Send messages to team channels",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        teamId = new { type = "string", description = "Team ID" },
                        channelId = new { type = "string", description = "Channel ID" },
                        content = new { type = "string", description = "Message content" },
                        contentType = new { type = "string", description = "Content type (Text or Html)", @enum = new[] { "Text", "Html" } }
                    },
                    required = new[] { "teamId", "channelId", "content" }
                }
            },
            new
            {
                name = "darbot-graph-teams-messages-reply",
                description = "Reply to channel messages",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        teamId = new { type = "string", description = "Team ID" },
                        channelId = new { type = "string", description = "Channel ID" },
                        messageId = new { type = "string", description = "Message ID to reply to" },
                        content = new { type = "string", description = "Reply content" }
                    },
                    required = new[] { "teamId", "channelId", "messageId", "content" }
                }
            },
            new
            {
                name = "darbot-graph-teams-members-list",
                description = "List team members",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        teamId = new { type = "string", description = "Team ID" }
                    },
                    required = new[] { "teamId" }
                }
            },
            new
            {
                name = "darbot-graph-teams-members-add",
                description = "Add members to teams",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        teamId = new { type = "string", description = "Team ID" },
                        userId = new { type = "string", description = "User ID to add to the team" },
                        role = new { type = "string", description = "Member role (Owner, Member)", @enum = new[] { "Owner", "Member" } }
                    },
                    required = new[] { "teamId", "userId" }
                }
            }
        };
    }

    public static List<object> GetFilesManagementTools()
    {
        return new List<object>
        {
            new
            {
                name = "darbot-graph-files-list",
                description = "List OneDrive files and folders",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        folderId = new { type = "string", description = "Folder ID (optional, defaults to root)" },
                        top = new { type = "integer", description = "Number of items to return" }
                    },
                    required = new[] { "userId" }
                }
            },
            new
            {
                name = "darbot-graph-files-upload",
                description = "Upload files to OneDrive",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        fileName = new { type = "string", description = "File name" },
                        content = new { type = "string", description = "File content (base64 encoded)" },
                        folderId = new { type = "string", description = "Destination folder ID (optional)" }
                    },
                    required = new[] { "userId", "fileName", "content" }
                }
            },
            new
            {
                name = "darbot-graph-files-download",
                description = "Download files from OneDrive",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        fileId = new { type = "string", description = "File ID" }
                    },
                    required = new[] { "userId", "fileId" }
                }
            },
            new
            {
                name = "darbot-graph-files-share",
                description = "Create sharing links for files",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID or User Principal Name" },
                        fileId = new { type = "string", description = "File ID" },
                        type = new { type = "string", description = "Share type (View, Edit)", @enum = new[] { "View", "Edit" } },
                        scope = new { type = "string", description = "Share scope (Anonymous, Organization)", @enum = new[] { "Anonymous", "Organization" } }
                    },
                    required = new[] { "userId", "fileId", "type" }
                }
            }
        };
    }

    public static List<object> GetSharePointTools()
    {
        return new List<object>
        {
            new
            {
                name = "darbot-graph-sharepoint-sites-list",
                description = "List SharePoint sites",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        search = new { type = "string", description = "Search query for sites" },
                        top = new { type = "integer", description = "Number of sites to return" }
                    }
                }
            },
            new
            {
                name = "darbot-graph-sharepoint-lists-list",
                description = "Get lists from SharePoint sites",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        siteId = new { type = "string", description = "SharePoint site ID" }
                    },
                    required = new[] { "siteId" }
                }
            },
            new
            {
                name = "darbot-graph-sharepoint-items-list",
                description = "Retrieve items from SharePoint lists",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        siteId = new { type = "string", description = "SharePoint site ID" },
                        listId = new { type = "string", description = "List ID" },
                        top = new { type = "integer", description = "Number of items to return" }
                    },
                    required = new[] { "siteId", "listId" }
                }
            }
        };
    }

    public static List<object> GetSecurityTools()
    {
        return new List<object>
        {
            new
            {
                name = "darbot-graph-security-signins-list",
                description = "Retrieve user sign-in logs",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID (optional)" },
                        top = new { type = "integer", description = "Number of logs to return" },
                        filter = new { type = "string", description = "OData filter expression" }
                    }
                }
            },
            new
            {
                name = "darbot-graph-security-audit-list",
                description = "Get directory audit logs",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        top = new { type = "integer", description = "Number of logs to return" },
                        filter = new { type = "string", description = "OData filter expression" }
                    }
                }
            },
            new
            {
                name = "darbot-graph-security-risks-list",
                description = "List users flagged for risk",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        top = new { type = "integer", description = "Number of users to return" },
                        filter = new { type = "string", description = "OData filter expression" }
                    }
                }
            },
            new
            {
                name = "darbot-graph-security-risks-confirm",
                description = "Confirm or dismiss risky users",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", description = "User ID" },
                        action = new { type = "string", description = "Action to take (Confirm, Dismiss)", @enum = new[] { "Confirm", "Dismiss" } }
                    },
                    required = new[] { "userId", "action" }
                }
            },
            new
            {
                name = "darbot-graph-security-policies-list",
                description = "List conditional access policies",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        top = new { type = "integer", description = "Number of policies to return" }
                    }
                }
            }
        };
    }

    public static List<object> GetReportsTools()
    {
        return new List<object>
        {
            new
            {
                name = "darbot-graph-reports-usage",
                description = "Get Microsoft 365 usage reports",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        reportType = new { type = "string", description = "Type of report (Overview, UserActivity, DeviceUsage)", @enum = new[] { "Overview", "UserActivity", "DeviceUsage" } },
                        period = new { type = "string", description = "Report period (D7, D30, D90, D180)", @enum = new[] { "D7", "D30", "D90", "D180" } }
                    },
                    required = new[] { "reportType" }
                }
            },
            new
            {
                name = "darbot-graph-reports-teams",
                description = "Get Teams activity reports",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        period = new { type = "string", description = "Report period (D7, D30, D90, D180)", @enum = new[] { "D7", "D30", "D90", "D180" } }
                    },
                    required = new[] { "period" }
                }
            },
            new
            {
                name = "darbot-graph-reports-email",
                description = "Get email activity reports",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        period = new { type = "string", description = "Report period (D7, D30, D90, D180)", @enum = new[] { "D7", "D30", "D90", "D180" } }
                    },
                    required = new[] { "period" }
                }
            },
            new
            {
                name = "darbot-graph-reports-sharepoint",
                description = "Get SharePoint activity reports",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        period = new { type = "string", description = "Report period (D7, D30, D90, D180)", @enum = new[] { "D7", "D30", "D90", "D180" } }
                    },
                    required = new[] { "period" }
                }
            }
        };
    }

    public static List<object> GetApplicationsTools()
    {
        return new List<object>
        {
            new
            {
                name = "darbot-graph-apps-list",
                description = "List applications in the directory",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        top = new { type = "integer", description = "Number of applications to return" },
                        filter = new { type = "string", description = "OData filter expression" }
                    }
                }
            },
            new
            {
                name = "darbot-graph-apps-get",
                description = "Get details of a specific application",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        appId = new { type = "string", description = "Application ID" }
                    },
                    required = new[] { "appId" }
                }
            },
            new
            {
                name = "darbot-graph-apps-create",
                description = "Create a new application registration",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        displayName = new { type = "string", description = "Application display name" },
                        signInAudience = new { type = "string", description = "Sign-in audience", @enum = new[] { "AzureADMyOrg", "AzureADMultipleOrgs", "AzureADandPersonalMicrosoftAccount" } }
                    },
                    required = new[] { "displayName" }
                }
            },
            new
            {
                name = "darbot-graph-apps-update",
                description = "Update application properties",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        appId = new { type = "string", description = "Application ID" },
                        displayName = new { type = "string", description = "Application display name" },
                        description = new { type = "string", description = "Application description" }
                    },
                    required = new[] { "appId" }
                }
            },
            new
            {
                name = "darbot-graph-apps-delete",
                description = "Delete an application",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        appId = new { type = "string", description = "Application ID" }
                    },
                    required = new[] { "appId" }
                }
            },
            new
            {
                name = "darbot-graph-apps-permissions-list",
                description = "List application permissions",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        appId = new { type = "string", description = "Application ID" }
                    },
                    required = new[] { "appId" }
                }
            },
            new
            {
                name = "darbot-graph-apps-permissions-grant",
                description = "Grant permissions to an application",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        appId = new { type = "string", description = "Application ID" },
                        permissions = new { type = "array", items = new { type = "string" }, description = "Permission scopes to grant" }
                    },
                    required = new[] { "appId", "permissions" }
                }
            },
            new
            {
                name = "darbot-graph-apps-secrets-create",
                description = "Create application client secret",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        appId = new { type = "string", description = "Application ID" },
                        displayName = new { type = "string", description = "Secret display name" },
                        endDateTime = new { type = "string", description = "Expiration date (ISO 8601)" }
                    },
                    required = new[] { "appId", "displayName" }
                }
            }
        };
    }
}