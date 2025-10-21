using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Azure.Identity;
using OpenAI;
using OpenAI.Chat;

namespace folderchat.Services
{
    // Azure.AI.OpenAI v2.1.0 + OpenAI 2.x 構成
    // - 認証: DefaultAzureCredential（ユーザー提供サンプル方針）
    // - クライアント: AzureOpenAIClient から ChatClient を取得
    // - メッセージ型: OpenAI.Chat の SystemChatMessage / UserChatMessage / AssistantChatMessage
    internal class AzureOpenAIChatService : IChatService
    {
        private readonly ChatClient _chatClient;
        private readonly List<ChatMessage> _conversationHistory = new();

        public AzureOpenAIChatService(string endpoint, string apiKey, string deploymentName, string apiVersion)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("Endpoint cannot be empty", nameof(endpoint));
            if (string.IsNullOrWhiteSpace(deploymentName))
                throw new ArgumentException("Deployment Name cannot be empty", nameof(deploymentName));

            if (!Uri.TryCreate(endpoint.Trim(), UriKind.Absolute, out var endpointUri))
                throw new ArgumentException("Invalid Azure endpoint. Expected format like https://YOUR_RESOURCE_NAME.openai.azure.com/", nameof(endpoint));

            // Entra ID（DefaultAzureCredential）で認証 + API キー / 環境変数フォールバック
            var effectiveApiKey = !string.IsNullOrWhiteSpace(apiKey) ? apiKey
                : Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY")
                ?? Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY")
                ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            AzureOpenAIClient azureClient = !string.IsNullOrWhiteSpace(effectiveApiKey)
                ? new AzureOpenAIClient(endpointUri, new System.ClientModel.ApiKeyCredential(effectiveApiKey))
                : new AzureOpenAIClient(endpointUri, new DefaultAzureCredential());

            // Azure 側はデプロイ名でモデル指定
            _chatClient = azureClient.GetChatClient(deploymentName);

            // 互換目的で受け取るが未使用
            _ = apiKey;
            _ = apiVersion;
        }

        public async Task<SendMessageAsyncResult> SendMessageAsync(string userInput, string? systemMessage = null)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                throw new ArgumentNullException(nameof(userInput), "Message is empty.");

            // 送信用メッセージリストを組み立て
            var messages = new List<ChatMessage>();

            if (!string.IsNullOrEmpty(systemMessage))
            {
                // 今回指定の System メッセージを先頭に据える（既存の System は省く）
                messages.Add(new SystemChatMessage(systemMessage));
                foreach (var msg in _conversationHistory)
                {
                    if (msg is not SystemChatMessage)
                    {
                        messages.Add(msg);
                    }
                }
            }
            else
            {
                // 既存履歴をそのまま使用
                messages.AddRange(_conversationHistory);
            }

            // 今回のユーザ入力
            messages.Add(new UserChatMessage(userInput));

            // ChatCompletion（非ストリーミング）
            ChatCompletion completion = await _chatClient.CompleteChatAsync(messages);

            // 履歴更新
            if (!string.IsNullOrEmpty(systemMessage))
            {
                _conversationHistory.Clear();
                _conversationHistory.Add(new SystemChatMessage(systemMessage));
                // messages: [System] + (旧履歴の非System) + [User]
                foreach (var msg in messages.Skip(1))
                {
                    _conversationHistory.Add(msg);
                }
            }
            else
            {
                // 履歴は既存＋今回追加の User の状態
                // messages の最後が User なので、重複を避けるために履歴へ個別追加
                _conversationHistory.Add(new UserChatMessage(userInput));
            }

            // アシスタント応答を履歴に追加
            var assistantMessage = new AssistantChatMessage(completion);
            _conversationHistory.Add(assistantMessage);

            // 応答テキスト抽出（テキスト以外のパートは無視）
            string assistantText = string.Join("",
                completion.Content?
                    .Where(p => p.Text is not null)
                    .Select(p => p.Text) ?? Array.Empty<string>()
            );

            return new SendMessageAsyncResult
            {
                ActualUserMessage = userInput,
                AssistantResponse = assistantText
            };
        }

        public void ClearHistory()
        {
            _conversationHistory.Clear();
        }
    }
}
