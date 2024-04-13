using api.ApiServices;
using api.DtoModels;
using Common;
using Database.Interfaces;
using Kafka.Interfaces;
using Kafka.Services;
using Microsoft.AspNetCore.Mvc;
using Models.KafkaMessages;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class DetectController : ControllerBase
{
    private readonly ILogger<DetectController> logger;
    private readonly ITestRepository testRepository;
    private readonly FileService fileService;
    private readonly IKafkaProducesService kafkaProducesService;
    private readonly KafkaEventHandler  kafkaEventHandler;

    public DetectController(ILogger<DetectController> logger, ITestRepository testRepository, FileService fileService, IKafkaProducesService kafkaProducesService, KafkaEventHandler kafkaEventHandler)
    {
        this.logger = logger;
        this.testRepository = testRepository;
        this.fileService = fileService;
        this.kafkaProducesService = kafkaProducesService;
        this.kafkaEventHandler = kafkaEventHandler;
    }

    [HttpPost("detect")]
    public async Task<IActionResult> Detect([FromBody] ImageData imageData)
    {
        if (imageData == null || string.IsNullOrEmpty(imageData.Image))
        {
            return BadRequest("Image data is missing.");
        }


        var imageBytes = Convert.FromBase64String(imageData.Image);

        var id = await fileService.UploadFileAsync(imageBytes);
        var guid = Guid.NewGuid().ToString();
        var kafkaMessage = new RequestMessage { Guid = guid, FileId = id };
        Console.WriteLine(kafkaMessage.ToJson());
        await kafkaProducesService.WriteTraceLogAsync(kafkaMessage);
        var tcs = new TaskCompletionSource<string>();
        kafkaEventHandler.MessageReceived += (sender, message) =>
        {
            if (message == guid)
            {
                tcs.SetResult(message);
            }
        };
        var receivedMessage = await tcs.Task;

        return Ok(receivedMessage);
    }

    [HttpPost("detect/test")]
    public IActionResult Test()
    {
        testRepository.Execute();
        return Ok();
    }
}
