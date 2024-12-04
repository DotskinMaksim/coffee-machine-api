using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CoffeeMachineAPI.Services;

// Pilti üles laadiva teenuse liides
public interface IImageUploadService
{
    Task<string> UploadImageAsync(IFormFile image); // Meetod pildi üleslaadimiseks
}

// Imgbb teenuse rakendamine piltide üleslaadimiseks
public class ImgbbImageUploadService : IImageUploadService
{
    private readonly string _apiKey; // API võtme muutujad
    private const string ImgbbUrl = "https://api.imgbb.com/1/upload"; // Imgbb API URL
    
    // Konstruktor, mis võtab konfigureerimisest API võtme
    public ImgbbImageUploadService(IConfiguration configuration)
    {
        _apiKey = configuration["Imgbb:ApiKey"]; // API võtme määramine konfigureerimisest
        if (string.IsNullOrEmpty(_apiKey)) // Kui API võti on puudulik, siis visatakse erind
        {
            throw new ArgumentException("Imgbb API Key is not configured.");
        }
    }

    // Meetod pildi üleslaadimiseks Imgbb teenusesse
    public async Task<string> UploadImageAsync(IFormFile image)
    {
        // Kontrollitakse, et pilt ei oleks tühi või null
        if (image == null || image.Length == 0)
        {
            throw new ArgumentException("Image file is null or empty.");
        }

        using var client = new HttpClient(); // Uue HttpClient loomiseks ühenduse tegemiseks
        var content = new MultipartFormDataContent(); // Uus multipart-formdata sisu pildi üleslaadimiseks

        // Laadime pildi mällu ja lisame selle sisu
        using (var ms = new MemoryStream())
        {
            await image.CopyToAsync(ms); // Pilt kopeeritakse mällu
            var fileContent = new ByteArrayContent(ms.ToArray()) // Muudame pildi andmed baidi massiiviks
            {
                Headers = { ContentType = new MediaTypeHeaderValue(image.ContentType) } // Lisame sobiva Content-Type päise
            };
            content.Add(fileContent, "image", image.FileName); // Lisame faili üleslaadimise sisusse
        }

        content.Add(new StringContent(_apiKey), "key"); // Lisame API võtme, mis on vajalik autentsuseks

        HttpResponseMessage response;
        try
        {
            // Saadame POST-päringu API-le, et üles laadida pilt
            response = await client.PostAsync(ImgbbUrl, content);
            response.EnsureSuccessStatusCode(); // Tagame, et vastus on edukas
        }
        catch (HttpRequestException ex) // Kui on viga, siis viskame erindi
        {
            throw new Exception("Failed to upload image to Imgbb.", ex);
        }

        // Võtame vastuse sisu ja deserialiseerime selle JSON-iks
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ImgbbResponse>(jsonResponse); // Muudame JSON-objektiks

        // Kui vastuses pole pilti URL-i, viskame vea
        if (result?.Data?.Url == null)
        {
            throw new Exception("Imgbb API response is invalid.");
        }

        return result.Data.Url; // Tagastame pildi URL-i
    }

    // Imgbb API vastuse struktuur
    private class ImgbbResponse
    {
        public ImgbbData Data { get; set; } // Andmed sisaldavad URL-i
    }

    // Andmed, mis sisaldavad üles laetud pildi URL-i
    private class ImgbbData
    {
        public string Url { get; set; } // Pildi URL
    }
}