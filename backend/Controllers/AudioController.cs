using PostInterviewAI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;


namespace PostInterviewAI.Controllers
{
    [ApiController]
    [Route("api/audio")]
    public class AudioController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TranscriptionService _transcriptionService;
        private readonly ChatGPTService _chatGPTService;
        private readonly IWebHostEnvironment _env;

        public AudioController(IHttpClientFactory httpClientFactory, IWebHostEnvironment env, TranscriptionService transcriptionService, 
                                ChatGPTService chatGPTService)
        {
            _httpClientFactory = httpClientFactory;
            _env = env;
            _transcriptionService = transcriptionService;
            _chatGPTService = chatGPTService;
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
            var transcript = await _transcriptionService.TranscribeWithWhisper(filePath);

            // Step 2: Call ChatGPT with prompt
            var feedback = await _chatGPTService.GetFeedbackFromChatGPT(transcript);
            //TODO: include when ready
            //feedback.Sentiment = await AnalyzeSentiment(transcript);
            return new JsonResult(feedback);
        }
    }
}
