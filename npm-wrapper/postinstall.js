const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

console.log('📦 Installing Darbot Graph MCP Server...');

// Check if .NET is installed
try {
    const dotnetVersion = execSync('dotnet --version', { encoding: 'utf8' }).trim();
    console.log(`✅ .NET detected: ${dotnetVersion}`);
} catch (error) {
    console.warn('⚠️  .NET SDK not detected. Please install .NET 8.0 SDK or later:');
    console.warn('   https://dotnet.microsoft.com/download');
}

// Check if the repository is available
const repoPath = path.join(process.cwd(), '..', '..');
const serverProjectPath = path.join(repoPath, 'src', 'DarbotGraphMcp.Server', 'DarbotGraphMcp.Server.csproj');

if (fs.existsSync(serverProjectPath)) {
    console.log('✅ Darbot Graph MCP Server project found');
    
    try {
        console.log('🔨 Building Darbot Graph MCP Server...');
        execSync('dotnet build', { 
            cwd: path.join(repoPath, 'src', 'DarbotGraphMcp.Server'),
            stdio: 'inherit' 
        });
        console.log('✅ Build completed successfully');
    } catch (error) {
        console.warn('⚠️  Build failed, but you can still use the MCP server:');
        console.warn('   It will be built automatically when first run');
    }
} else {
    console.log('⚠️  Darbot Graph MCP Server repository not found in expected location');
    console.log('💡 Please ensure you have cloned the repository:');
    console.log('   git clone https://github.com/dayour/Darbot-Graph-MCP.git');
}

console.log('');
console.log('🎉 Installation complete!');
console.log('');
console.log('📚 Next steps:');
console.log('1. Configure Azure AD app registration (see README.md)');
console.log('2. Use the one-click VS Code installation button');
console.log('3. Or manually add to your MCP configuration');
console.log('');
console.log('🔗 Documentation: https://github.com/dayour/Darbot-Graph-MCP');