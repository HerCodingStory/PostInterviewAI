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
            return Ok(transcription);
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
    }
}
