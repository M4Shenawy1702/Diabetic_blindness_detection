using CheckEyePro.Core.IServices;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace CheckEyePro.EF.Services
{
    public class PredictionService : IPrediction
    {
        private readonly HttpClient _httpClient;
        public PredictionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string> PredectApi(IFormFile Img)
        {
            try
            {
                MultipartFormDataContent formData = new MultipartFormDataContent();
                formData.Add(new StreamContent(Img.OpenReadStream()), "file", Img.FileName);


                HttpResponseMessage response = await _httpClient.PostAsync("predict", formData);
                response.EnsureSuccessStatusCode();

                dynamic jsonResponse = JsonSerializer.Deserialize<dynamic>(await response.Content.ReadAsStringAsync())!;
                Console.WriteLine(jsonResponse);
                string Diagnosis = response.Content.ReadAsStringAsync().Result;

                return Diagnosis;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"HTTP request failed: {ex.Message}");
            }
        }

    }
}
