using folderchat.Forms;
using folderchat.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Forms;

namespace folderchat.Services
{
    public class ApiServerService
    {
        private WebApplication? _apiServer;
        private readonly MainForm _mainForm;

        public ApiServerService(MainForm mainForm)
        {
            _mainForm = mainForm;
        }

        public bool IsRunning => _apiServer != null;

        public async Task StartApiServer()
        {
            try
            {
                if (_apiServer != null)
                {
                    _mainForm.LogSystemMessage("API server is already running");
                    return;
                }

                var port = Properties.Settings.Default.APIServerPort;
                var url = $"http://localhost:{port}";

                _mainForm.LogSystemMessage($"Starting API server on {url}...");
                _mainForm.SetApiStatus("API Server: Starting...");

                var builder = WebApplication.CreateBuilder();
                builder.WebHost.UseUrls(url);
                builder.Services.AddSingleton(_mainForm);

                var app = builder.Build();

                app.MapPost("/v1/chat/completions", async (ChatCompletionRequest request, MainForm mainForm) =>
                {
                    var chatService = mainForm.GetChatServiceForSummarization();
                    var userMessage = request.Messages.LastOrDefault(m => m.Role == "user")?.Content ?? "";

                    if (string.IsNullOrEmpty(userMessage))
                    {
                        return Results.BadRequest("User message cannot be empty.");
                    }

                    var result = await chatService.SendMessageAsync(userMessage);

                    if (mainForm.GetChatComponent() != null)
                    {
                        mainForm.Invoke(new Action(async () =>
                        {
                            mainForm.LogChatMessage("user (API)", result.ActualUserMessage);
                            await mainForm.GetChatComponent().AddMessageToChatAsync(result.ActualUserMessage, true, MessageType.User);

                            mainForm.LogChatMessage("assistant (API)", result.AssistantResponse);
                            await mainForm.GetChatComponent().AddMessageToChatAsync(result.AssistantResponse, false, MessageType.Assistant);
                        }));
                    }

                    var response = new ChatCompletionResponse
                    {
                        Id = $"chatcmpl-integrated-{Guid.NewGuid()}",
                        Object = "chat.completion",
                        Created = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                        Model = request.Model,
                        Choices = new List<ChatCompletionChoice>
                        {
                            new ChatCompletionChoice
                            {
                                Index = 0,
                                Message = new ResponseMessage
                                {
                                    Role = "assistant",
                                    Content = result.AssistantResponse
                                },
                                FinishReason = "stop"
                            }
                        },
                        Usage = new Usage()
                    };

                    return Results.Ok(response);
                });

                _apiServer = app;
                var statusMessage = $"API Server: Running on {url}";
                _mainForm.LogSystemMessage(statusMessage);
                _mainForm.SetApiStatus(statusMessage);

                await app.RunAsync();
            }
            catch (Exception ex)
            {
                var errorMessage = "API Server: Error";
                _mainForm.LogError($"Failed to start internal API server: {ex.Message}");
                _apiServer = null;
                _mainForm.SetApiStatus(errorMessage);
            }
        }

        public async Task StopApiServer()
        {
            try
            {
                if (_apiServer != null)
                {
                    _mainForm.LogSystemMessage("Stopping API server...");
                    _mainForm.SetApiStatus("API Server: Stopping...");
                    await _apiServer.StopAsync();
                    await _apiServer.DisposeAsync();
                    _apiServer = null;
                    _mainForm.SetApiStatus("API Server: Stopped");
                    _mainForm.LogSystemMessage("API server stopped successfully");
                }
            }
            catch (Exception ex)
            {
                _mainForm.LogError($"Error stopping API server: {ex.Message}");
                _mainForm.SetApiStatus("API Server: Error stopping");
            }
        }
    }
}
