using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace PostInterviewAI.Controllers
{
    [ApiController]
    [Route("api/audio")]
    public class AudioController : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            Directory.CreateDirectory(uploadsDir);

            var filePath = Path.Combine(uploadsDir, file.FileName);
            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            var transcription = await TranscribeWithWhisper(filePath);
            var feedback = await GetChatGptFeedbackAsync(transcription);
            return Ok(feedback);
        }

        private async Task<string> TranscribeWithWhisper(string filePath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"whisper/transcribe.py \"{filePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                Console.Error.WriteLine("Whisper error: " + error);
                return $"Transcription failed: {error}";
            }

            return output.Trim();
        }

        private async Task<string> GetChatGptFeedbackAsync(string transcript)
        {
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
                return "OpenAI API key is missing.";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var payload = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "You are an expert interview coach. Analyze the following answer and give constructive feedback on clarity, structure, content, and technical accuracy." },
                    new { role = "user", content = transcript }
                }
            };

            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var responseString = await response.Content.ReadAsStringAsync();

            using var doc = System.Text.Json.JsonDocument.Parse(responseString);
            var reply = doc.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString();

            return reply ?? "No feedback returned.";
        }
    }
}
