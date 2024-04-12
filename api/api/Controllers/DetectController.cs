using api.DtoModels;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;
[ApiController]
[Route("[controller]")]
public class DetectController : ControllerBase
{
    private readonly ILogger<DetectController> logger;

    public DetectController(ILogger<DetectController> logger)
    {
        this.logger = logger;
    }
    
    [HttpPost("detect")]
    public IActionResult Detect([FromBody] ImageData imageData)
    {
        if (imageData == null || string.IsNullOrEmpty(imageData.Image))
        {
            return BadRequest("Image data is missing.");
        }

        // Decode Base64 string to byte array
        var imageBytes = Convert.FromBase64String(imageData.Image);
        // produce kafka message
        // upload to file service
        
        return Ok();
    }
}
