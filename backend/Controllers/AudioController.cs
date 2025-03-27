using Microsoft.AspNetCore.Mvc;

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

            var filePath = Path.Combine("uploads", file.FileName);
            Directory.CreateDirectory("uploads");

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            return Ok($"File {file.FileName} uploaded successfully!");
        }
    }
}