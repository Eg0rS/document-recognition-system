using api.ApiServices;
using api.DtoModels;
using Common;
using Kafka.Interfaces;
using Kafka.Services;
using Microsoft.AspNetCore.Mvc;
using Models.KafkaMessages;
using Newtonsoft.Json;

namespace api.Controllers;


/// <summary>
/// Контроллер для проверки решения задачи хакатона
/// </summary>
[ApiController]
[Route("[controller]")]
public class DetectController : ControllerBase
{
    private readonly ILogger<DetectController> logger;
    private readonly FileService fileService;
    private readonly IKafkaProducesService kafkaProducesService;
    private readonly KafkaEventHandler kafkaEventHandler;

    public DetectController(ILogger<DetectController> logger, FileService fileService, IKafkaProducesService kafkaProducesService, KafkaEventHandler kafkaEventHandler)
    {
        this.logger = logger;

        this.fileService = fileService;
        this.kafkaProducesService = kafkaProducesService;
        this.kafkaEventHandler = kafkaEventHandler;
    }

    [HttpPost("detect")]
    public async Task<IActionResult> Detect([FromBody] ImageDataForHackTask imageDataForHackTask)
    {
        if (imageDataForHackTask == null || string.IsNullOrEmpty(imageDataForHackTask.Image))
        {
            return BadRequest("Image data is missing.");
        }

        var imageBytes = Convert.FromBase64String(imageDataForHackTask.Image);

        var id = await fileService.UploadFileAsync(imageBytes);
        var guid = Guid.NewGuid().ToString();
        var kafkaMessage = new RequestMessage { Guid = guid, FileId = id };
        Console.WriteLine(kafkaMessage.ToJson());
        await kafkaProducesService.WriteTraceLogAsync(kafkaMessage);
        var tcs = new TaskCompletionSource<string>();

        EventHandler<string> handler = null;
        handler = (sender, message) =>
        {
            var massageJson = JsonConvert.DeserializeObject<ResultMessage>(message);
            if (massageJson?.Guid == guid)
            {
                var response = new ResultForHackTask
                {
                    Type = massageJson.Type,
                    Series = massageJson.Series,
                    Number = massageJson.Number,
                    Confidence = massageJson.Confidence,
                    PageNumber = massageJson.PageNumber
                };
                tcs.SetResult(response.ToJson());
                kafkaEventHandler.MessageReceived -= handler; // Отписываемся от события
            }
        };

        kafkaEventHandler.MessageReceived += handler;


        var receivedMessage = await tcs.Task;

        return Ok(receivedMessage);
    }
}
