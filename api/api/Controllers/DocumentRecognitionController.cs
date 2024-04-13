using api.ApiServices;
using api.DtoModels;
using Kafka.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.KafkaMessages;

namespace api.Controllers;

/// <summary>
/// Контроллер для клиентских методов распознаванию документов мобильного приложения 
/// </summary>
[ApiController]
[Route("[controller]")]
public class DocumentRecognitionController : ControllerBase
{
    private readonly FileService fileService;
    private readonly IKafkaProducesService kafkaProducesService;

    public DocumentRecognitionController(FileService fileService, IKafkaProducesService kafkaProducesService)
    {
        this.fileService = fileService;
        this.kafkaProducesService = kafkaProducesService;
    }

    [HttpPost]
    public async Task<IActionResult> RecognitionDocument([FromBody] ImageData imageData)
    {
        if (imageData == null || string.IsNullOrEmpty(imageData.Image))
        {
            return BadRequest("Image data is missing.");
        }

        var imageBytes = Convert.FromBase64String(imageData.Image);

        var id = await fileService.UploadFileAsync(imageBytes);
        var guid = Guid.NewGuid().ToString();
        var kafkaMessage = new RequestMessage { Guid = guid, FileId = id };


        await kafkaProducesService.WriteTraceLogAsync(kafkaMessage);


        return Ok();
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserDocuments(string userId)
    {
        var listResults = new List<Result>();
        return Ok(listResults);
    }
}
