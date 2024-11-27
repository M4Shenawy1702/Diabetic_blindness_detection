//using Microsoft.AspNetCore.Mvc;
//using System.Text.Json;

//namespace Graduation.Controllers;

//[ApiController]
//[Route("[controller]")]

//public class FlaskConsumer : ControllerBase
//{
//    private readonly HttpClient _httpClient;

//    public FlaskConsumer()
//    {
//        _httpClient = new HttpClient();
//        // Set base URL of Flask API
//        _httpClient.BaseAddress = new Uri("http://localhost:8000/predict");
//        _httpClient.Timeout = TimeSpan.FromSeconds(500);
//    }

//    [HttpPost("Predict")]
//    public async Task<dynamic> Predict([FromForm] IFormFile file)
//    {
//        try
//        {
//            MultipartFormDataContent formData = new MultipartFormDataContent();
//            formData.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);


//            HttpResponseMessage response = await _httpClient.PostAsync("predict", formData);
//            response.EnsureSuccessStatusCode();

//            dynamic jsonResponse = JsonSerializer.Deserialize<dynamic>(await response.Content.ReadAsStringAsync())!;
//            Console.WriteLine(jsonResponse);

//            return response.Content.ReadAsStringAsync().Result;
//        }
//        catch (HttpRequestException ex)
//        {
//            throw new Exception($"HTTP request failed: {ex.Message}");
//        }
//    }

//}
