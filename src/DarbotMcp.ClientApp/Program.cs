using System.ClientModel;
using Azure;

using Azure.AI.OpenAI;

using DarbotMcp.ClientApp.Components;
using DarbotMcp.ClientApp.Extensions;

using Microsoft.Extensions.AI;

using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

using OpenAI;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var connectionstring = config.GetConnectionString("openai") ?? throw new InvalidOperationException("Missing connection string: openai.");
var endpoint = connectionstring.Split(';').FirstOrDefault(x => x.StartsWith("Endpoint=", StringComparison.InvariantCultureIgnoreCase))?.Split('=')[1]
                   ?? throw new InvalidOperationException("Missing endpoint.");
var apiKey = connectionstring.Split(';').FirstOrDefault(x => x.StartsWith("Key=", StringComparison.InvariantCultureIgnoreCase))?.Split('=')[1]
                 ?? throw new InvalidOperationException("Missing API key.");

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

var credential = new AzureKeyCredential(apiKey);

// TODO: Fix OpenAI client configuration for .NET 8 compatibility
// IChatClient chatClient;
// if (endpoint.TrimEnd('/').Equals("https://models.inference.ai.azure.com"))
// {
//     var openAIClient = new OpenAI.OpenAIClient(apiKey);
//     chatClient = openAIClient.GetChatClient(config["OpenAI:DeploymentName"]).AsIChatClient();
// }
// else
// {
//     var openAIClient = new Azure.AI.OpenAI.OpenAIClient(new Uri(endpoint), credential);
//     chatClient = openAIClient.GetChatClient(config["OpenAI:DeploymentName"]).AsIChatClient();
// }

// builder.Services.AddChatClient(chatClient)
//                 .UseFunctionInvocation()
//                 .UseLogging();

// TODO: Configure MCP client for Graph server
// builder.Services.AddSingleton<IMcpClient>(sp =>
// {
//     var config = sp.GetRequiredService<IConfiguration>();
//     var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
// 
//     var uri = new Uri(config["McpServers:GraphServer"]!);
// 
//     var clientTransportOptions = new SseClientTransportOptions()
//     {
//         Endpoint = new Uri($"{uri.AbsoluteUri.TrimEnd('/')}/sse")
//     };
//     var clientTransport = new SseClientTransport(clientTransportOptions, loggerFactory);
// 
//     var clientOptions = new McpClientOptions()
//     {
//         ClientInfo = new Implementation()
//         {
//             Name = "Microsoft Graph MCP Client",
//             Version = "1.0.0",
//         }
//     };
// 
//     return McpClientFactory.CreateAsync(clientTransport, clientOptions, loggerFactory).GetAwaiter().GetResult();
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseStaticFiles();
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
