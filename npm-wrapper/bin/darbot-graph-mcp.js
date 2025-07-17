#!/usr/bin/env node

const fs = require('fs');
const path = require('path');
const { spawn } = require('child_process');

/**
 * Darbot Graph MCP Server - NPM Wrapper
 * Provides a Node.js wrapper for the .NET-based MCP server
 */

// Parse command line arguments
const args = process.argv.slice(2);
const tenantId = args[0] || process.env.AzureAd__TenantId || '';
const clientId = args[1] || process.env.AzureAd__ClientId || '';
const clientSecret = args[2] || process.env.AzureAd__ClientSecret || '';

console.log('🚀 Starting Darbot Graph MCP Server...');

// Validate tenant ID to prevent cross-tenant issues
function validateTenantId(tenantId) {
    if (!tenantId) {
        console.log('⚠️  No tenant ID provided - running in demo mode');
        return true;
    }
    
    // Basic GUID validation - more permissive pattern
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
    if (!guidRegex.test(tenantId)) {
        console.error('❌ Invalid tenant ID format. Must be a valid GUID.');
        console.error('   Expected format: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx');
        process.exit(1);
    }
    
    // Additional security check - warn about common/default tenant IDs
    const commonTenantIds = [
        '00000000-0000-0000-0000-000000000000',
        '11111111-1111-1111-1111-111111111111',
        'ffffffff-ffff-ffff-ffff-ffffffffffff'
    ];
    
    if (commonTenantIds.includes(tenantId.toLowerCase())) {
        console.error('❌ Security warning: Using placeholder or common tenant ID');
        console.error('   This could lead to cross-tenant security issues');
        console.error('   Please use your organization\'s actual tenant ID');
        process.exit(1);
    }
    
    return true;
}

// Find the server project path
function findServerPath() {
    const possiblePaths = [
        // When installed via NPM from repository
        path.join(__dirname, '..', '..', 'src', 'DarbotGraphMcp.Server'),
        // When running from cloned repository
        path.join(process.cwd(), 'src', 'DarbotGraphMcp.Server'),
        // When npm wrapper is in subdirectory
        path.join(__dirname, '..', '..', '..', 'src', 'DarbotGraphMcp.Server'),
        // Global installation path
        path.join(__dirname, '..', '..', '..', '..', 'src', 'DarbotGraphMcp.Server')
    ];
    
    for (const serverPath of possiblePaths) {
        const projectFile = path.join(serverPath, 'DarbotGraphMcp.Server.csproj');
        if (fs.existsSync(projectFile)) {
            console.log(`✅ Found server project at: ${serverPath}`);
            return serverPath;
        }
    }
    
    return null;
}

// Start the MCP server
function startServer() {
    // Validate tenant ID for security
    validateTenantId(tenantId);
    
    const serverPath = findServerPath();
    if (!serverPath) {
        console.error('❌ Darbot Graph MCP Server project not found');
        console.error('💡 Please ensure the repository is properly cloned:');
        console.error('   git clone https://github.com/dayour/Darbot-Graph-MCP.git');
        process.exit(1);
    }
    
    // Prepare environment variables
    const env = { ...process.env };
    if (tenantId) env.AzureAd__TenantId = tenantId;
    if (clientId) env.AzureAd__ClientId = clientId;
    if (clientSecret) env.AzureAd__ClientSecret = clientSecret;
    
    // Log configuration (without secrets)
    console.log('🔧 Configuration:');
    console.log(`   Tenant ID: ${tenantId || '(demo mode)'}`)
    console.log(`   Client ID: ${clientId || '(demo mode)'}`)
    console.log(`   Client Secret: ${clientSecret ? '***' : '(demo mode)'}`)
    console.log('');
    
    if (!tenantId || !clientId || !clientSecret) {
        console.log('ℹ️  Running in demo mode - sample data will be returned');
        console.log('💡 For production use, provide Azure AD credentials');
        console.log('');
    }
    
    // Start the .NET server
    console.log('🔨 Building and starting MCP server...');
    const server = spawn('dotnet', ['run'], {
        cwd: serverPath,
        env: env,
        stdio: 'inherit'
    });
    
    server.on('error', (error) => {
        console.error('❌ Failed to start server:', error.message);
        console.error('💡 Please ensure .NET 8.0 SDK is installed:');
        console.error('   https://dotnet.microsoft.com/download');
        process.exit(1);
    });
    
    server.on('close', (code) => {
        console.log(`Server exited with code ${code}`);
        process.exit(code);
    });
    
    // Handle graceful shutdown
    process.on('SIGINT', () => {
        console.log('\n🛑 Shutting down MCP server...');
        server.kill('SIGINT');
    });
    
    process.on('SIGTERM', () => {
        console.log('\n🛑 Shutting down MCP server...');
        server.kill('SIGTERM');
    });
}

// Export functions for module use
module.exports = {
    startServer,
    findServerPath,
    validateTenantId
};

// Run if called directly
if (require.main === module) {
    startServer();
}