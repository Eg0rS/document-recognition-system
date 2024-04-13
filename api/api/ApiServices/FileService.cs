using System.Text;
using api.DtoModels;
using Common;
using Newtonsoft.Json;

namespace api.ApiServices;

public class FileService
{
    private readonly string baseUrl = "http://file-service:10002";
    private readonly ILogger<FileService> logger;

    public FileService(ILogger<FileService> logger)
    {
        this.logger = logger;
    }

    public async Task<string> UploadFileAsync(byte[] file)
    {
        using var client = new HttpClient();
        var image = new ImageData() { Image = Convert.ToBase64String(file) };
        var response = await client.PostAsync($"{baseUrl}/api/files", new StringContent(JsonConvert.SerializeObject(image), Encoding.UTF8, "application/json"));
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to upload file. {content}");
        }

        return content;
    }
}
