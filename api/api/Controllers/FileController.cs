using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class FileController : ControllerBase
{
    private readonly ILogger<FileController> logger;

    public FileController(ILogger<FileController> logger)
    {
        this.logger = logger;
    }


    [HttpGet("{fileId}")]
    public IActionResult DownloadFile(string fileId)
    {
        return Ok();
    }
}
