using System.Globalization;
using api.ApiServices;
using api.DbModels;
using api.DtoModels;
using Common;
using CsvHelper;
using Database;
using Database.Interfaces;
using Kafka.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.KafkaMessages;
using Newtonsoft.Json;

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
    private readonly IConnection connection;


    public DocumentRecognitionController(FileService fileService, IKafkaProducesService kafkaProducesService, IConnection connection)
    {
        this.fileService = fileService;
        this.kafkaProducesService = kafkaProducesService;
        this.connection = connection;
    }

    [HttpPost]
    public async Task<IActionResult> RecognitionDocument([FromBody] ImageData imageData)
    {
        if (imageData == null || string.IsNullOrEmpty(imageData.Image))
        {
            return BadRequest("Image data is missing.");
        }

        var imageBytes = Convert.FromBase64String(imageData.Image);

        var fileId = await fileService.UploadFileAsync(imageBytes);
        var guid = Guid.NewGuid().ToString();
        var kafkaMessage = new RequestMessage { Guid = guid, FileId = fileId };

        await AddRequestToDb(imageData.UserId, guid, fileId);
        await kafkaProducesService.WriteTraceLogAsync(kafkaMessage);


        return Ok();
    }


    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserDocuments(string userId)
    {
        var listResults = new List<Result>()
        {
            new Result()
            {
                Status = 1,
                UserId = userId,
                Type = "personal_passport",
                Confidence = (decimal)1.0,
                Guid = "1",
                Series = "1213",
                Number = "5664",
                PageNumber = 1,
                FileId = "qweqw1233",
                Data = new Dictionary<string, string>
                {
                    { "address_en", "123" },
                    { "address_ru", "123" },
                    { "birth_date", "123" },
                    { "birth_place_en", "123" },
                    { "birth_place_ru", "123" },
                    { "car_number", "123" },
                    { "car_series", "123" },
                    { "categories", "123" },
                    { "driving_exp", "123" },
                    { "expiration_date", "123" },
                    { "initial_names_en", "123" },
                    { "initial_names_ru", "123" },
                    { "issue_date", "123" },
                    { "issue_place_ru", "123" },
                    { "number_front", "123" },
                    { "series_front", "123" },
                    { "surname_en", "123" },
                    { "surname_ru", "123" },
                }
            },
            new Result()
            {
                Status = 1,
                UserId = userId,
                Type = "vehicle_passport",
                Confidence = (decimal)1.0,
                Guid = "2",
                Series = "1213",
                Number = "5664",
                PageNumber = 1,
                FileId = "qweqw1233",
                Data = new Dictionary<string, string>
                {
                    { "car", "123" },
                    { "car_category", "123" },
                    { "car_color", "123" },
                    { "car_issue_date", "123" },
                    { "car_max_weight", "123" },
                    { "car_raw_weight", "123" },
                    { "car_type", "123" },
                    { "ecology_class", "123" },
                    { "engine_capacity", "123" },
                    { "engine_number", "123" },
                    { "engine_power", "123" },
                    { "engine_type", "123" },
                    { "number", "123" },
                    { "pts-segmentation", "123" },
                    { "series", "123" },
                    { "vin", "123" },
                }
            },
            new Result()
            {
                Status = 1,
                UserId = userId,
                Type = "vehicle_certificate",
                Confidence = (decimal)1.0,
                Guid = "3",
                Series = "1213",
                Number = "5664",
                PageNumber = 1,
                FileId = "qweqw1233",
                Data = new Dictionary<string, string>
                {
                    { "apartment_number", "123" },
                    { "area", "123" },
                    { "car_category", "123" },
                    { "car_color", "123" },
                    { "car_issue_date", "123" },
                    { "car_max_weight", "123" },
                    { "car_model_en", "123" },
                    { "car_model_ru", "123" },
                    { "car_number", "123" },
                    { "car_plate", "123" },
                    { "car_raw_weight", "123" },
                    { "car_series", "123" },
                    { "car_type", "123" },
                    { "chassis_number", "123" },
                    { "country", "123" },
                    { "ecology_class", "123" },
                    { "engine_capacity", "123" },
                    { "engine_model", "123" },
                    { "engine_number", "123" },
                    { "engine_power", "123" },
                    { "fathersname", "123" },
                    { "home_number", "123" },
                    { "issue_day", "123" },
                    { "issue_month", "123" },
                    { "issue_place", "123" },
                    { "issue_year", "123" },
                    { "locality", "123" },
                    { "name_en", "123" },
                    { "name_ru", "123" },
                    { "number", "123" },
                    { "region_en", "123" },
                    { "region_ru", "123" },
                    { "series", "123" },
                    { "special_marks", "123" },
                    { "street", "123" },
                    { "surname_en", "123" },
                    { "temporary_expiration_date", "123" },
                    { "temporary_pts_number", "123" },
                    { "vin", "123" },
                }
            },
            new Result()
            {
                Status = 1,
                UserId = userId,
                Type = "driver_license",
                Confidence = (decimal)1.0,
                Guid = "4",
                Series = "1213",
                Number = "5664",
                PageNumber = 1,
                FileId = "qweqw1233",
                Data = new Dictionary<string, string>
                {
                    { "address_en", "123" },
                    { "address_ru", "123" },
                    { "birth_date", "123" },
                    { "birth_place_en", "123" },
                    { "birth_place_ru", "123" },
                    { "car_number", "123" },
                    { "car_series", "123" },
                    { "categories", "123" },
                    { "driving_exp", "123" },
                    { "expiration_date", "123" },
                    { "initial_names_en", "123" },
                    { "initial_names_ru", "123" },
                    { "issue_date", "123" },
                    { "issue_place_en", "123" },
                    { "issue_place_ru", "123" },
                    { "number_front", "123" },
                    { "series_front", "123" },
                    { "surname_en", "123" },
                    { "surname_ru", "123" },
                }
            },
        };
        return Ok(listResults);
    }


    [HttpGet("test/{userId}")]
    public async Task<IActionResult> GetUserDocumentsTest(string userId)
    {
        var queryObject = new QueryObject(
            "SELECT id as Id, user_id as UserId, guid as Guid, file_id as FileId FROM requests WHERE user_id = @user_id",
            new { user_id = userId });
        var requests = await connection.ListOrEmpty<DbRequest>(queryObject);
        var listResults = new List<DbResult>();
        if (requests.Count != 0)
        {
            var list = requests.Select(x => x.Guid).ToList();
            var result = string.Join(", ", list.Select(e => $"'{e}'"));
            var queryObjectResult = new QueryObject(
                $"SELECT id as Id, guid as Guid, file_id as FileId, type as Type, series as Series, number as Number, page_number as Number, confidence as Confidence, data as Data FROM resolutions WHERE guid in ({result})");
            var t = queryObjectResult.ToJson();
            Console.WriteLine(t);
            listResults.AddRange(await connection.ListOrEmpty<DbResult>(queryObjectResult));
        }


        var resultList = new List<Result>();
        foreach (var request in requests)
        {
            var result = listResults.FirstOrDefault(x => x.Guid == request.Guid);
            if (result != null)
            {
                resultList.Add(new Result()
                {
                    UserId = request.UserId,
                    Guid = result.Guid,
                    Type = result.Type,
                    Series = result.Series,
                    Number = result.Number,
                    PageNumber = result.PageNumber,
                    Confidence = result.Confidence,
                    FileId = result.FileId,
                    Data = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.Data)
                });
            }
            else
            {
                resultList.Add(new Result() { UserId = request.UserId, Guid = request.Guid, Status = 0 });
            }
        }

        return Ok(resultList);
    }

    [HttpGet("download/{guid}")]
    public async Task<IActionResult> GetDocument(string guid)
    {
        var queryObject = new QueryObject(
            "SELECT id as Id, guid as Guid, file_id as FileId, type as Type, series as Series, number as Number, page_number as Number, confidence as Confidence, data as Data FROM resolutions WHERE guid = @guid",
            new { guid = guid });
        var result = await connection.FirstOrDefault<DbResult>(queryObject);
        if (result == null)
        {
            return NotFound();
        }

        var queryObject2 = new QueryObject(
            "SELECT id as Id, user_id as UserId, guid as Guid, file_id as FileId FROM requests WHERE guid = @guid",
            new { guid = guid });
        var request = await connection.FirstOrDefault<DbRequest>(queryObject2);
        if (request == null)
        {
            return NotFound();
        }

        var result1 = new Result()
        {
            UserId = request.UserId,
            Guid = result.Guid,
            Type = result.Type,
            Series = result.Series,
            Number = result.Number,
            PageNumber = result.PageNumber,
            Confidence = result.Confidence,
            FileId = result.FileId,
            Data = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.Data)
        };

        using var memoryStream = new MemoryStream();
        await using var writer = new StreamWriter(memoryStream);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecord(result1);
        await writer.FlushAsync();
        var content = memoryStream.ToArray();
        return File(content, "text/csv", "output.csv");
    }

    private async Task AddRequestToDb(string imageDataUserId, string guid, string fileId)
    {
        var queryObject = new QueryObject(
            "INSERT INTO requests (user_id, guid, file_id) VALUES (@user_id, @guid, @file_id) RETURNING id",
            new { user_id = imageDataUserId, guid = guid, file_id = fileId });
        await connection.CommandWithResponse<int>(queryObject);
    }
}
