using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace PostInterviewAI.Controllers
{
    public class InterviewFeedback
    {
        public string Clarity { get; set; }
        public string Structure { get; set; }
        public string Content { get; set; }
        public string TechnicalAccuracy { get; set; }
    }

    [ApiController]
    [Route("api/audio")]
    public class AudioController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;

        public AudioController(IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
        {
            _httpClientFactory = httpClientFactory;
            _env = env;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file uploaded" });

            var uploadsPath = Path.Combine(_env.ContentRootPath, "uploads");
            Directory.CreateDirectory(uploadsPath);
            var filePath = Path.Combine(uploadsPath, file.FileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            // Step 1: Run Whisper script
            var transcript = await TranscribeWithWhisper(filePath);

            // Step 2: Call ChatGPT with prompt
            var feedback = await GetFeedbackFromChatGPT(transcript);

            return new JsonResult(feedback);
        }

        private async Task<string> TranscribeWithWhisper(string filePath)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"whisper/transcribe.py \"{filePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            var process = Process.Start(psi);
            string output = await process.StandardOutput.ReadToEndAsync();
            return output.Trim();
        }

        private async Task<InterviewFeedback> GetFeedbackFromChatGPT(string transcript)
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
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] {
                    new { role = "user", content = prompt }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var responseJson = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseJson);
            var messageContent = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            return JsonSerializer.Deserialize<InterviewFeedback>(messageContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
