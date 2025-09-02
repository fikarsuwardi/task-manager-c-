using System.Text.Json;

namespace TaskManagement.Infrastructure.Services
{
    public interface IExternalApiService
    {
        Task<bool> ValidateEmailAsync(string email);
        Task<string> GetRandomQuoteAsync();
    }

    public class ExternalApiService : IExternalApiService
    {
        private readonly HttpClient _httpClient;

        public ExternalApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // public async Task<bool> ValidateEmailAsync(string email)
        // {
        //     try
        //     {
        //         var response = await _httpClient.GetAsync($"https://api.zerobounce.net/v2/validate?api_key=fake&email={email}");
        //         return response.IsSuccessStatusCode;
        //     }
        //     catch
        //     {
        //         return email.Contains("@") && email.Contains(".");
        //     }
        // }

        public async Task<bool> ValidateEmailAsync(string email)
        {
            await Task.Delay(1); // Simulate async call
            return !string.IsNullOrEmpty(email) && email.Contains("@") && email.Contains(".");
        }

        public async Task<string> GetRandomQuoteAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://api.quotable.io/random");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var quote = JsonSerializer.Deserialize<QuoteResponse>(content);
                    return quote?.Content ?? "Keep working hard!";
                }
            }
            catch { }
            return "Keep working hard!";
        }
    }

    public class QuoteResponse
    {
        public string Content { get; set; }
    }
}