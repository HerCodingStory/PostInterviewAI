using PostInterviewAI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;

namespace PostInterviewAI.Services
{
    public class ChatGPTService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ChatGPTService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<InterviewFeedback> GetFeedbackFromChatGPT(string transcript)
        {
            var prompt = @$"
                Analyze the following interview transcript.
                Return your response as a JSON object with the following structure:

                {{
                ""clarity"": ""..."",
                ""structure"": ""..."",
                ""content"": ""..."",
                ""technical_accuracy"": ""...""
                }}

                Transcript:
                {transcript}";

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var responseJson = await response.Content.ReadAsStringAsync();

            Console.WriteLine("ChatGPT raw response:");
            Console.WriteLine(responseJson);

            try
            {
                using var doc = JsonDocument.Parse(responseJson);

                if (!doc.RootElement.TryGetProperty("choices", out var choices) ||
                    choices.GetArrayLength() == 0 ||
                    !choices[0].TryGetProperty("message", out var message) ||
                    !message.TryGetProperty("content", out var contentProp))
                {
                    throw new Exception("Unexpected ChatGPT response structure.");
                }

                var messageContent = contentProp.GetString();

                return JsonSerializer.Deserialize<InterviewFeedback>(messageContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Failed to parse ChatGPT response:");
                Console.WriteLine(ex.Message);
                return new InterviewFeedback
                {
                    Clarity = null,
                    Structure = null,
                    Content = null,
                    TechnicalAccuracy = null
                };
            }
        }
    }
}