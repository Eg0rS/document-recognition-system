using api.DtoModels;
using Database.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;
[ApiController]
[Route("[controller]")]
public class DetectController : ControllerBase
{
    private readonly ILogger<DetectController> logger;
    private readonly ITestRepository testRepository;

    public DetectController(ILogger<DetectController> logger, ITestRepository testRepository)
    {
        this.logger = logger;
        this.testRepository = testRepository;
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

    [HttpPost("detect/test")]
    public IActionResult Test()
    {
        testRepository.Execute();
        return Ok();
    }
}
