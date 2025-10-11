// Clippy Assistant powered by Spark LLM API

const SPARK_API_CONFIG = {
    // Note: In production, use environment variables or secure configuration
    endpoint: 'https://spark-api.cn-huabei-1.xf-yun.com/v1/chat/completions',
    // API key should be configured in production
    model: 'spark-3.5'
};

let conversationHistory = [];

// Toggle Clippy chat window
function toggleClippy() {
    const clippyChat = document.getElementById('clippy-chat');
    if (clippyChat.style.display === 'none' || clippyChat.style.display === '') {
        clippyChat.style.display = 'flex';
        document.getElementById('clippy-input').focus();
    } else {
        clippyChat.style.display = 'none';
    }
}

// Send message to Clippy
async function sendClippyMessage() {
    const input = document.getElementById('clippy-input');
    const message = input.value.trim();
    
    if (!message) return;
    
    // Add user message to chat
    addMessage(message, 'user');
    input.value = '';
    
    // Show typing indicator
    showTypingIndicator();
    
    // Get response from knowledge base or AI
    const response = await getClippyResponse(message);
    
    // Remove typing indicator
    removeTypingIndicator();
    
    // Add assistant response
    addMessage(response, 'assistant');
}

// Add message to chat
function addMessage(content, role) {
    const messagesContainer = document.getElementById('clippy-messages');
    const messageDiv = document.createElement('div');
    messageDiv.className = `clippy-message ${role}`;
    
    if (role === 'assistant') {
        messageDiv.innerHTML = `
            <div class="message-avatar">📎</div>
            <div class="message-content">${formatMessage(content)}</div>
        `;
    } else {
        messageDiv.innerHTML = `
            <div class="message-content">${escapeHtml(content)}</div>
        `;
    }
    
    messagesContainer.appendChild(messageDiv);
    messagesContainer.scrollTop = messagesContainer.scrollHeight;
    
    // Add to conversation history
    conversationHistory.push({ role, content });
}

// Show typing indicator
function showTypingIndicator() {
    const messagesContainer = document.getElementById('clippy-messages');
    const typingDiv = document.createElement('div');
    typingDiv.className = 'clippy-message assistant typing-indicator';
    typingDiv.id = 'typing-indicator';
    typingDiv.innerHTML = `
        <div class="message-avatar">📎</div>
        <div class="message-content">
            <span class="typing-dot"></span>
            <span class="typing-dot"></span>
            <span class="typing-dot"></span>
        </div>
    `;
    messagesContainer.appendChild(typingDiv);
    messagesContainer.scrollTop = messagesContainer.scrollHeight;
}

// Remove typing indicator
function removeTypingIndicator() {
    const typingIndicator = document.getElementById('typing-indicator');
    if (typingIndicator) {
        typingIndicator.remove();
    }
}

// Get response from Clippy (knowledge base or AI)
async function getClippyResponse(userMessage) {
    const lowerMessage = userMessage.toLowerCase();
    
    // Knowledge base responses
    const knowledgeBase = {
        'setup': `To set up Darbot Graph MCP:
1. Install via one-click button or NPX
2. Configure Azure AD credentials (or use demo mode)
3. Restart VS Code
4. Start using Microsoft Graph tools!

Check out our <a href="docs.html">detailed setup guide</a> for more info.`,
        
        'azure': `For Azure AD setup:
1. Go to Azure Portal
2. Navigate to Azure Active Directory
3. Register a new application
4. Configure API permissions for Microsoft Graph
5. Create a client secret

See the <a href="docs.html#azure-setup">Azure AD Setup Guide</a> for step-by-step instructions.`,
        
        'tools': `Darbot Graph MCP provides 64+ tools across 10 categories:
- User Management (8 tools)
- Group Management (8 tools)
- Email Management (8 tools)
- Calendar Management (8 tools)
- Teams Management (8 tools)
- File Management (8 tools)
- Security & Compliance (8 tools)
- Reports & Analytics (8 tools)

Check out the <a href="api.html">API Reference</a> for complete documentation.`,
        
        'extensibility': `To add extensibility to Darbot Graph MCP:
1. Identify the Microsoft Graph API you want to use
2. Design tool schema with input parameters
3. Implement service method in GraphServiceEnhanced
4. Add tool definition to appropriate ToolCategories
5. Test and document your new tool

See the <a href="extensibility.html">Extensibility Guide</a> for detailed instructions.`,
        
        'demo': `Demo mode allows you to test Darbot Graph MCP safely without affecting production data. To use demo mode, simply leave Azure AD credentials empty during installation. 

Demo mode provides simulated responses for all tools, perfect for:
- Learning the tool structure
- Testing integrations
- Development and testing`,
        
        'troubleshooting': `Common issues and solutions:
- **Authentication errors**: Verify Azure AD credentials
- **Permission denied**: Check API permissions in Azure AD
- **Tools not showing**: Restart VS Code completely
- **Build errors**: Ensure .NET 8.0 SDK is installed

For more help, see the <a href="https://github.com/dayour/Darbot-Graph-MCP/blob/main/TROUBLESHOOTING.md">Troubleshooting Guide</a>.`,
        
        'security': `Security best practices:
- Use app-only authentication for automation
- Never commit credentials to source control
- Implement least-privilege permissions
- Use managed identities when possible
- Regularly rotate client secrets
- Enable conditional access policies

Read the <a href="https://github.com/dayour/Darbot-Graph-MCP/blob/main/SECURITY.md">Security Guide</a> for more.`,
        
        'examples': `Popular use cases:
- Automated user provisioning
- Email automation and monitoring
- Calendar event management
- Teams collaboration automation
- File and document management
- Security compliance reporting

Check out <a href="examples.html">Examples</a> for code samples!`
    };
    
    // Check knowledge base first
    for (const [key, value] of Object.entries(knowledgeBase)) {
        if (lowerMessage.includes(key)) {
            return value;
        }
    }
    
    // Fallback to AI response (simulated for now)
    // In production, this would call the Spark LLM API
    return await getAIResponse(userMessage);
}

// Get AI response from Spark LLM API
async function getAIResponse(userMessage) {
    // Note: This is a simplified implementation
    // In production, you would make an actual API call to Spark LLM
    
    const systemPrompt = `You are a helpful assistant for Darbot Graph MCP, an extensible MCP server for Microsoft Graph API. 
You help users with:
- Setting up Azure AD authentication
- Understanding the 64+ available tools
- Adding extensibility to the framework
- Troubleshooting common issues
- Best practices for Microsoft Graph integration

Provide concise, helpful responses with links to relevant documentation when appropriate.`;
    
    // Simulated AI response based on common patterns
    const patterns = {
        'how': 'Great question! ',
        'what': 'Let me explain: ',
        'why': 'Here\'s why: ',
        'can': 'Yes, you can! ',
        'install': 'For installation, ',
        'error': 'For errors, '
    };
    
    let response = 'I\'m here to help with Darbot Graph MCP! ';
    
    for (const [pattern, prefix] of Object.entries(patterns)) {
        if (userMessage.toLowerCase().includes(pattern)) {
            response = prefix;
            break;
        }
    }
    
    // Add contextual information
    if (userMessage.toLowerCase().includes('install')) {
        response += 'You can install Darbot Graph MCP using the one-click installation button or via NPX. Check out our <a href="docs.html">documentation</a> for detailed steps.';
    } else if (userMessage.toLowerCase().includes('api') || userMessage.toLowerCase().includes('command')) {
        response += 'Darbot Graph MCP provides 64+ tools for Microsoft Graph operations. See the <a href="api.html">API Reference</a> for complete documentation.';
    } else if (userMessage.toLowerCase().includes('error') || userMessage.toLowerCase().includes('problem')) {
        response += 'Common issues are usually related to Azure AD setup or permissions. Check our <a href="https://github.com/dayour/Darbot-Graph-MCP/blob/main/TROUBLESHOOTING.md">Troubleshooting Guide</a>.';
    } else if (userMessage.toLowerCase().includes('extend') || userMessage.toLowerCase().includes('custom')) {
        response += 'Darbot Graph MCP is designed for extensibility. Check out the <a href="extensibility.html">Extensibility Guide</a> to learn how to add custom Graph API capabilities.';
    } else {
        response += 'Could you be more specific? I can help with setup, tools, extensibility, troubleshooting, or examples. Or try our <a href="docs.html">documentation</a>!';
    }
    
    return response;
}

// Format message with HTML support
function formatMessage(message) {
    // Allow specific HTML tags
    return message.replace(/\n/g, '<br>');
}

// Escape HTML for user input
function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// Handle Enter key in input
document.addEventListener('DOMContentLoaded', () => {
    const clippyInput = document.getElementById('clippy-input');
    if (clippyInput) {
        clippyInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                sendClippyMessage();
            }
        });
    }
});

// Quick action buttons
function askAboutSetup() {
    document.getElementById('clippy-input').value = 'How do I set up Azure AD?';
    sendClippyMessage();
}

function askAboutTools() {
    document.getElementById('clippy-input').value = 'What tools are available?';
    sendClippyMessage();
}

function askAboutExtensibility() {
    document.getElementById('clippy-input').value = 'How do I add extensibility?';
    sendClippyMessage();
}

// Export functions
window.toggleClippy = toggleClippy;
window.sendClippyMessage = sendClippyMessage;
window.askAboutSetup = askAboutSetup;
window.askAboutTools = askAboutTools;
window.askAboutExtensibility = askAboutExtensibility;

// Auto-show Clippy on first visit
if (!localStorage.getItem('clippy-shown')) {
    setTimeout(() => {
        toggleClippy();
        localStorage.setItem('clippy-shown', 'true');
    }, 3000);
}

// Add CSS for typing indicator
const style = document.createElement('style');
style.textContent = `
.typing-indicator .message-content {
    display: flex;
    gap: 4px;
    padding: 12px;
}

.typing-dot {
    width: 8px;
    height: 8px;
    background: #999;
    border-radius: 50%;
    animation: typing 1.4s infinite;
}

.typing-dot:nth-child(2) {
    animation-delay: 0.2s;
}

.typing-dot:nth-child(3) {
    animation-delay: 0.4s;
}

@keyframes typing {
    0%, 60%, 100% {
        transform: translateY(0);
        opacity: 0.7;
    }
    30% {
        transform: translateY(-10px);
        opacity: 1;
    }
}
`;
document.head.appendChild(style);
