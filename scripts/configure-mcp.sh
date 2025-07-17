#!/bin/bash

# Darbot Graph MCP Configuration Wizard
# This script helps users configure secure authentication for the MCP server

set -e

BLUE='\033[0;34m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${BLUE}🎯 Darbot Graph MCP Configuration Wizard${NC}"
echo "======================================================"
echo
echo "This wizard will help you configure secure authentication for the Darbot Graph MCP Server."
echo "We'll guide you through multiple authentication options for different use cases."
echo

# Function to detect VS Code installation
detect_vscode() {
    if command -v code &> /dev/null; then
        echo "code"
    elif command -v code-insiders &> /dev/null; then
        echo "code-insiders"
    else
        echo ""
    fi
}

# Function to get VS Code config directory
get_vscode_config_dir() {
    if [[ "$OSTYPE" == "darwin"* ]]; then
        # macOS
        echo "$HOME/Library/Application Support/Code/User"
    elif [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "win32" ]]; then
        # Windows
        echo "$APPDATA/Code/User"
    else
        # Linux
        echo "$HOME/.config/Code/User"
    fi
}

# Function to validate Azure AD configuration
validate_azure_config() {
    local tenant_id="$1"
    local client_id="$2"
    local client_secret="$3"
    
    # Basic format validation
    if [[ ! "$tenant_id" =~ ^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$ ]]; then
        echo -e "${RED}❌ Invalid Tenant ID format. Expected GUID format.${NC}"
        return 1
    fi
    
    if [[ ! "$client_id" =~ ^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$ ]]; then
        echo -e "${RED}❌ Invalid Client ID format. Expected GUID format.${NC}"
        return 1
    fi
    
    if [[ ${#client_secret} -lt 8 ]]; then
        echo -e "${RED}❌ Client Secret too short. Expected at least 8 characters.${NC}"
        return 1
    fi
    
    return 0
}

# Main menu
echo -e "${YELLOW}🔐 Authentication Method Selection${NC}"
echo
echo "Choose your preferred authentication method:"
echo "1. 🔑 Client Secret (App Registration) - Most common for production"
echo "2. 🖥️  Azure CLI - Use existing 'az login' session"
echo "3. 💻 VS Code - Use VS Code's Azure authentication"
echo "4. ☁️  Managed Identity - For Azure hosting"
echo "5. 🔄 Default Azure Credential Chain - Try multiple methods"
echo "6. 📚 Environment Variables - Custom setup guidance"
echo "7. 🎭 Demo Mode - Testing without Azure AD"
echo

read -p "Enter your choice (1-7): " choice

case $choice in
    1)
        echo
        echo -e "${BLUE}🔑 Client Secret Authentication Setup${NC}"
        echo "----------------------------------------"
        echo
        echo "You'll need:"
        echo "• Azure AD Tenant ID"
        echo "• App Registration Client ID"
        echo "• App Registration Client Secret"
        echo
        echo "If you don't have these, follow the Azure AD setup guide in the README."
        echo
        
        read -p "Azure AD Tenant ID: " tenant_id
        read -p "App Registration Client ID: " client_id
        read -s -p "App Registration Client Secret: " client_secret
        echo
        
        if validate_azure_config "$tenant_id" "$client_id" "$client_secret"; then
            echo -e "${GREEN}✅ Configuration validated successfully${NC}"
            
            # Offer multiple configuration methods
            echo
            echo "Choose how to store these credentials:"
            echo "1. Environment variables (recommended for security)"
            echo "2. VS Code settings.json (convenient but less secure)"
            echo "3. Show all options"
            echo
            
            read -p "Enter choice (1-3): " storage_choice
            
            case $storage_choice in
                1)
                    echo
                    echo -e "${GREEN}📝 Environment Variables Configuration${NC}"
                    echo "Add these to your environment:"
                    echo
                    echo "export AZURE_AD_TENANT_ID=\"$tenant_id\""
                    echo "export AZURE_AD_CLIENT_ID=\"$client_id\""
                    echo "export AZURE_AD_CLIENT_SECRET=\"$client_secret\""
                    echo
                    echo "For permanent setup, add to your shell profile (.bashrc, .zshrc, etc.)"
                    ;;
                2)
                    vscode_config_dir=$(get_vscode_config_dir)
                    echo
                    echo -e "${GREEN}📝 VS Code Configuration${NC}"
                    echo "Add this to your VS Code settings.json:"
                    echo "File: $vscode_config_dir/settings.json"
                    echo
                    echo '{'
                    echo '  "mcp.servers": {'
                    echo '    "darbot-graph": {'
                    echo '      "command": "npx",'
                    echo '      "args": ["-y", "@darbotlabs/darbot-graph-mcp"],'
                    echo '      "env": {'
                    echo "        \"AzureAd__TenantId\": \"$tenant_id\","
                    echo "        \"AzureAd__ClientId\": \"$client_id\","
                    echo "        \"AzureAd__ClientSecret\": \"$client_secret\""
                    echo '      }'
                    echo '    }'
                    echo '  }'
                    echo '}'
                    ;;
                3)
                    echo
                    echo -e "${GREEN}📝 All Configuration Options${NC}"
                    echo
                    echo "1. Environment Variables (recommended):"
                    echo "   export AZURE_AD_TENANT_ID=\"$tenant_id\""
                    echo "   export AZURE_AD_CLIENT_ID=\"$client_id\""
                    echo "   export AZURE_AD_CLIENT_SECRET=\"$client_secret\""
                    echo
                    echo "2. VS Code settings.json:"
                    echo "   Add env section to mcp.servers.darbot-graph"
                    echo
                    echo "3. Claude Desktop config:"
                    echo "   Use env section in mcpServers configuration"
                    echo
                    echo "4. appsettings.json (not recommended for production):"
                    echo "   Direct configuration in the server settings file"
                    ;;
            esac
        fi
        ;;
        
    2)
        echo
        echo -e "${BLUE}🖥️  Azure CLI Authentication Setup${NC}"
        echo "-----------------------------------"
        echo
        
        # Check if Azure CLI is installed
        if ! command -v az &> /dev/null; then
            echo -e "${RED}❌ Azure CLI not found${NC}"
            echo "Please install Azure CLI: https://docs.microsoft.com/cli/azure/install-azure-cli"
            exit 1
        fi
        
        # Check if logged in
        if ! az account show &> /dev/null; then
            echo -e "${YELLOW}⚠️  Not logged into Azure CLI${NC}"
            echo "Please run: az login"
            exit 1
        fi
        
        # Get current account info
        account_info=$(az account show --query '{tenantId:tenantId, name:name}' -o json 2>/dev/null)
        tenant_id=$(echo "$account_info" | jq -r .tenantId)
        account_name=$(echo "$account_info" | jq -r .name)
        
        echo -e "${GREEN}✅ Azure CLI session detected${NC}"
        echo "Account: $account_name"
        echo "Tenant: $tenant_id"
        echo
        
        echo -e "${GREEN}📝 Azure CLI Configuration${NC}"
        echo "Use this configuration:"
        echo
        echo "Environment variables:"
        echo "export AZURE_AD_TENANT_ID=\"$tenant_id\""
        echo "export AZURE_AD_USE_AZURE_CLI=true"
        echo
        echo "Or in VS Code settings.json:"
        echo '"env": {'
        echo "  \"AzureAd__TenantId\": \"$tenant_id\","
        echo '  "AzureAd__UseAzureCli": true'
        echo '}'
        ;;
        
    3)
        echo
        echo -e "${BLUE}💻 VS Code Authentication Setup${NC}"
        echo "--------------------------------"
        echo
        
        vscode_cmd=$(detect_vscode)
        if [[ -z "$vscode_cmd" ]]; then
            echo -e "${RED}❌ VS Code not found${NC}"
            echo "Please install VS Code or VS Code Insiders"
            exit 1
        fi
        
        echo -e "${GREEN}✅ VS Code detected: $vscode_cmd${NC}"
        echo
        echo "This method uses VS Code's built-in Azure authentication."
        echo "You'll need to sign in to Azure through VS Code's Azure extension."
        echo
        
        read -p "Azure AD Tenant ID: " tenant_id
        
        if [[ "$tenant_id" =~ ^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$ ]]; then
            echo -e "${GREEN}📝 VS Code Authentication Configuration${NC}"
            echo "Use this configuration:"
            echo
            echo "Environment variables:"
            echo "export AZURE_AD_TENANT_ID=\"$tenant_id\""
            echo "export AZURE_AD_USE_VSCODE=true"
            echo
            echo "Or in VS Code settings.json:"
            echo '"env": {'
            echo "  \"AzureAd__TenantId\": \"$tenant_id\","
            echo '  "AzureAd__UseVSCode": true'
            echo '}'
        else
            echo -e "${RED}❌ Invalid Tenant ID format${NC}"
        fi
        ;;
        
    4)
        echo
        echo -e "${BLUE}☁️  Managed Identity Setup${NC}"
        echo "-------------------------"
        echo
        echo "Managed Identity is used when running on Azure infrastructure"
        echo "(Azure VMs, App Service, Functions, etc.)"
        echo
        echo -e "${GREEN}📝 Managed Identity Configuration${NC}"
        echo "Use this configuration:"
        echo
        echo "Environment variables:"
        echo "export AZURE_AD_USE_MANAGED_IDENTITY=true"
        echo
        echo "Optional - specify client ID for user-assigned managed identity:"
        echo "export AZURE_AD_CLIENT_ID=\"your-managed-identity-client-id\""
        echo
        echo "Or in VS Code settings.json:"
        echo '"env": {'
        echo '  "AzureAd__UseManagedIdentity": true'
        echo '}'
        ;;
        
    5)
        echo
        echo -e "${BLUE}🔄 Default Azure Credential Chain${NC}"
        echo "--------------------------------"
        echo
        echo "This method tries multiple authentication methods in order:"
        echo "1. Environment variables"
        echo "2. Managed Identity"
        echo "3. Azure CLI"
        echo "4. VS Code"
        echo "5. Visual Studio"
        echo
        
        read -p "Azure AD Tenant ID (optional): " tenant_id
        
        echo -e "${GREEN}📝 Default Credential Chain Configuration${NC}"
        echo "Use this configuration:"
        echo
        echo "Environment variables:"
        if [[ -n "$tenant_id" ]]; then
            echo "export AZURE_AD_TENANT_ID=\"$tenant_id\""
        fi
        echo "export AZURE_AD_USE_DEFAULT_CHAIN=true"
        echo
        echo "Or in VS Code settings.json:"
        echo '"env": {'
        if [[ -n "$tenant_id" ]]; then
            echo "  \"AzureAd__TenantId\": \"$tenant_id\","
        fi
        echo '  "AzureAd__UseDefaultChain": true'
        echo '}'
        ;;
        
    6)
        echo
        echo -e "${BLUE}📚 Environment Variables Guide${NC}"
        echo "-----------------------------"
        echo
        echo "Supported environment variables:"
        echo
        echo "Standard format:"
        echo "• AZURE_AD_TENANT_ID"
        echo "• AZURE_AD_CLIENT_ID"
        echo "• AZURE_AD_CLIENT_SECRET"
        echo "• AZURE_AD_USE_AZURE_CLI"
        echo "• AZURE_AD_USE_VSCODE"
        echo "• AZURE_AD_USE_MANAGED_IDENTITY"
        echo "• AZURE_AD_USE_DEFAULT_CHAIN"
        echo
        echo ".NET Configuration format:"
        echo "• AzureAd__TenantId"
        echo "• AzureAd__ClientId"
        echo "• AzureAd__ClientSecret"
        echo "• AzureAd__UseAzureCli"
        echo "• AzureAd__UseVSCode"
        echo "• AzureAd__UseManagedIdentity"
        echo "• AzureAd__UseDefaultChain"
        echo
        echo "Priority order:"
        echo "1. Environment variables (highest)"
        echo "2. Configuration files"
        echo "3. Command line arguments"
        ;;
        
    7)
        echo
        echo -e "${BLUE}🎭 Demo Mode${NC}"
        echo "----------"
        echo
        echo "Demo mode allows testing without Azure AD configuration."
        echo "All tools return sample data instead of real Microsoft Graph data."
        echo
        echo -e "${GREEN}📝 Demo Mode Configuration${NC}"
        echo "Simply don't set any authentication configuration."
        echo "The server will automatically detect no credentials and run in demo mode."
        echo
        echo "For explicit demo mode, you can set:"
        echo "export AZURE_AD_DEMO_MODE=true"
        ;;
        
    *)
        echo -e "${RED}❌ Invalid choice${NC}"
        exit 1
        ;;
esac

echo
echo -e "${GREEN}🎉 Configuration complete!${NC}"
echo
echo "Next steps:"
echo "1. Apply the configuration using your chosen method"
echo "2. Restart VS Code or your MCP client"
echo "3. Test the connection using the MCP tools"
echo
echo "For more information, see the project README.md"
echo "GitHub: https://github.com/dayour/Darbot-Graph-MCP"