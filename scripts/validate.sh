#!/bin/bash
# Darbot Graph MCP Server - End-to-End Validation Audit Script
# This script performs comprehensive validation of all server components

set -e

echo "🔍 Starting Darbot Graph MCP Server Validation Audit"
echo "=================================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print status
print_status() {
    if [ $1 -eq 0 ]; then
        echo -e "${GREEN}✅ $2${NC}"
    else
        echo -e "${RED}❌ $2${NC}"
        exit 1
    fi
}

print_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

# 1. Environment Validation
print_info "Checking prerequisites..."

# Check .NET
if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    print_status 0 ".NET SDK detected: $DOTNET_VERSION"
else
    print_status 1 ".NET SDK not found"
fi

# Check Node.js (for NPM wrapper)
if command -v node &> /dev/null; then
    NODE_VERSION=$(node --version)
    print_status 0 "Node.js detected: $NODE_VERSION"
else
    print_warning "Node.js not found - NPM wrapper won't work"
fi

# 2. Build Validation
print_info "Validating build process..."

cd "$(dirname "$0")"
if [ -f "../src/DarbotGraphMcp.Server/DarbotGraphMcp.Server.csproj" ]; then
    print_status 0 "Server project found"
else
    print_status 1 "Server project not found"
fi

# Restore dependencies
print_info "Restoring .NET dependencies..."
dotnet restore ../DarbotGraphMcp.sln > /dev/null 2>&1
print_status $? "Dependencies restored"

# Build solution
print_info "Building solution..."
dotnet build ../DarbotGraphMcp.sln > /dev/null 2>&1
print_status $? "Solution built successfully"

# 3. Server Startup Validation
print_info "Testing server startup..."

# Start server in background
cd ../src/DarbotGraphMcp.Server
dotnet run --project . &
SERVER_PID=$!

# Wait for server to start
sleep 10

# Check if server is running
if ps -p $SERVER_PID > /dev/null; then
    print_status 0 "Server started successfully (PID: $SERVER_PID)"
else
    print_status 1 "Server failed to start"
fi

# 4. API Endpoint Validation
print_info "Testing API endpoints..."

# Test health endpoint
HEALTH_RESPONSE=$(curl -s http://localhost:5000/health 2>/dev/null || echo "FAILED")
if [ "$HEALTH_RESPONSE" = "Darbot Graph MCP Server - Enhanced" ]; then
    print_status 0 "Health endpoint working"
else
    print_status 1 "Health endpoint failed: $HEALTH_RESPONSE"
fi

# Test tools endpoint
TOOLS_COUNT=$(curl -s http://localhost:5000/tools 2>/dev/null | jq length 2>/dev/null || echo "0")
if [ "$TOOLS_COUNT" = "64" ]; then
    print_status 0 "Tools endpoint returning 64 tools"
else
    print_status 1 "Tools endpoint failed - got $TOOLS_COUNT tools instead of 64"
fi

# 5. Tool Execution Validation
print_info "Testing tool execution..."

# Test a sample tool call
TOOL_RESPONSE=$(curl -s -X POST http://localhost:5000/call-tool \
    -H "Content-Type: application/json" \
    -d '{"name": "darbot-graph-users-list", "arguments": {"top": 2}}' 2>/dev/null)

if echo "$TOOL_RESPONSE" | jq -e '.success' > /dev/null 2>&1; then
    DEMO_MODE=$(echo "$TOOL_RESPONSE" | jq -r '.demo' 2>/dev/null || echo "false")
    if [ "$DEMO_MODE" = "true" ]; then
        print_status 0 "Tool execution working (demo mode)"
    else
        print_status 0 "Tool execution working (production mode)"
    fi
else
    print_status 1 "Tool execution failed"
fi

# 6. NPM Wrapper Validation
if command -v node &> /dev/null; then
    print_info "Testing NPM wrapper..."
    
    cd ../../npm-wrapper
    
    # Check package.json exists
    if [ -f "package.json" ]; then
        print_status 0 "NPM package.json found"
    else
        print_status 1 "NPM package.json missing"
    fi
    
    # Test wrapper can find server
    if node -e "const {findServerPath} = require('./bin/darbot-graph-mcp.js'); console.log(findServerPath());" > /dev/null 2>&1; then
        print_status 0 "NPM wrapper can locate server"
    else
        print_status 1 "NPM wrapper cannot locate server"
    fi
else
    print_warning "Skipping NPM wrapper validation - Node.js not available"
fi

# 7. Configuration Validation
print_info "Validating configuration files..."

# Check appsettings.json
if [ -f "../src/DarbotGraphMcp.Server/appsettings.json" ]; then
    if jq empty ../src/DarbotGraphMcp.Server/appsettings.json > /dev/null 2>&1; then
        print_status 0 "appsettings.json is valid JSON"
    else
        print_status 1 "appsettings.json is invalid JSON"
    fi
else
    print_status 1 "appsettings.json not found"
fi

# 8. Tool Categories Validation
print_info "Validating tool categories..."

TOOL_CATEGORIES=$(curl -s http://localhost:5000/tools 2>/dev/null | jq -r '.[].name' | cut -d'-' -f3 | sort | uniq | wc -l)
if [ "$TOOL_CATEGORIES" -ge "10" ]; then
    print_status 0 "Tool categories validation passed ($TOOL_CATEGORIES categories found)"
else
    print_status 1 "Insufficient tool categories found: $TOOL_CATEGORIES"
fi

# 9. Performance Validation
print_info "Testing performance..."

# Measure response time
START_TIME=$(date +%s%N)
curl -s http://localhost:5000/health > /dev/null
END_TIME=$(date +%s%N)
RESPONSE_TIME=$(( (END_TIME - START_TIME) / 1000000 )) # Convert to milliseconds

if [ "$RESPONSE_TIME" -lt 1000 ]; then
    print_status 0 "Response time acceptable: ${RESPONSE_TIME}ms"
else
    print_warning "Response time high: ${RESPONSE_TIME}ms"
fi

# 10. Cleanup
print_info "Cleaning up..."

# Stop the server
kill $SERVER_PID 2>/dev/null || true
wait $SERVER_PID 2>/dev/null || true
print_status 0 "Server stopped"

# Final Summary
echo ""
echo "=================================================="
echo -e "${GREEN}🎉 Validation Audit Complete!${NC}"
echo ""
echo "✅ All core functions validated successfully"
echo "✅ Server builds and runs without errors"
echo "✅ All 64 Microsoft Graph tools available"
echo "✅ API endpoints responding correctly"
echo "✅ Tool execution working in demo mode"
echo "✅ NPM wrapper infrastructure ready"
echo "✅ Configuration files valid"
echo ""
echo -e "${BLUE}🚀 Darbot Graph MCP Server is ready for production use!${NC}"