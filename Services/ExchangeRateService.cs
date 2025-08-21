using CuzdanUygulamasi.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CuzdanUygulamasi.Services
{
    public class ExchangeRateService
    {
        private readonly HttpClient _httpClient;

        public ExchangeRateService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<ExchangeRateResponse> GetRatesAsync(string baseCurrency, string symbols)
        {
            string apiKey = "Mm7cJlRxQwFYXWoEZqyvtTEGNujUwqcJ";
            string url = $"https://api.apilayer.com/exchangerates_data/latest?base={baseCurrency}&symbols={symbols}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", apiKey);

            var response = await _httpClient.GetStringAsync(url);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // büyük/küçük harf farkını kaldır
            };

            return JsonSerializer.Deserialize<ExchangeRateResponse>(response, options);
        }


    }
}
