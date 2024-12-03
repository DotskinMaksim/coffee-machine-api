using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CoffeeMachineAPI.Services;

public interface IImageUploadService
{
    Task<string> UploadImageAsync(IFormFile image);
}

public class ImgbbImageUploadService : IImageUploadService
{
    private readonly string _apiKey;
    private const string ImgbbUrl = "https://api.imgbb.com/1/upload";
    
    public ImgbbImageUploadService(IConfiguration configuration)
    {
        _apiKey = configuration["Imgbb:ApiKey"];
        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new ArgumentException("Imgbb API Key is not configured.");
        }
    }

    public async Task<string> UploadImageAsync(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            throw new ArgumentException("Image file is null or empty.");
        }

        using var client = new HttpClient();
        var content = new MultipartFormDataContent();

        using (var ms = new MemoryStream())
        {
            await image.CopyToAsync(ms);
            var fileContent = new ByteArrayContent(ms.ToArray())
            {
                Headers = { ContentType = new MediaTypeHeaderValue(image.ContentType) }
            };
            content.Add(fileContent, "image", image.FileName);
        }

        content.Add(new StringContent(_apiKey), "key");

        HttpResponseMessage response;
        try
        {
            response = await client.PostAsync(ImgbbUrl, content);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            throw new Exception("Failed to upload image to Imgbb.", ex);
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ImgbbResponse>(jsonResponse);

        if (result?.Data?.Url == null)
        {
            throw new Exception("Imgbb API response is invalid.");
        }

        return result.Data.Url;
    }

    private class ImgbbResponse
    {
        public ImgbbData Data { get; set; }
    }

    private class ImgbbData
    {
        public string Url { get; set; }
    }
}