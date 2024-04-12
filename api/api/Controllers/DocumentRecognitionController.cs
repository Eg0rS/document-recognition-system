using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class DocumentRecognitionController : ControllerBase
{
    
    public DocumentRecognitionController()
    {
    }

    [HttpPost]
    public IActionResult RecognitionDocument([FromForm] IFormFile file)
    {
        return Ok();
    }
}
