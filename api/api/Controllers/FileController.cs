using api.ApiServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace api.Controllers;

/// <summary>
/// Контроллер для скачивания расмеченных документов для мобильного приложения
/// </summary>
[ApiController]
[Route("[controller]")]
public class FileController : ControllerBase
{
    private readonly FileService fileService;

    public FileController(FileService fileService)
    {
        this.fileService = fileService;
    }

    [HttpGet("{fileId}")]
    public async Task<IActionResult> GetFile(string fileId)
    {
        var file = await fileService.DownloadFileAsync(fileId);
        if (file == null)
        {
            return NotFound();
        }

        return Ok(JsonConvert.SerializeObject(file));
    }
}
