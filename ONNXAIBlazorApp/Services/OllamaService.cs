using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace ONNXAIBlazorApp.Data
{
    public class OllamaService
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public OllamaService(string baseUrl)
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(10);
        }

        public async Task<string> QueryOllamaModelAsync(string query)
        {
            var requestBody = new
            {
                model = "llama2", // Change to your model name
                messages = new[]
                {
                    new { role = "user", content = query }
                }
            };

            var requestContent = new StringContent(
                JsonConvert.SerializeObject(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync($"{_baseUrl}/v1/chat/completions", requestContent);

            if (!response.IsSuccessStatusCode)
            {
                var results = response.Content.ReadAsStringAsync();
                throw new Exception($"Ollama API Error: {response.ReasonPhrase}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(responseBody);

            // Extract the content of the response
            var result = jsonDocument.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            return result;
        }
    }
}
