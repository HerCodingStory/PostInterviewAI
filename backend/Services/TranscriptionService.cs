using System.Diagnostics;

namespace PostInterviewAI.Services
{
    public class TranscriptionService
    {
        public async Task<string> TranscribeWithWhisper(string filePath)
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
    }
}