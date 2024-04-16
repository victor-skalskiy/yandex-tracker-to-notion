using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YandexTrackerToNotion.Services
{
	public class TelegramService
	{
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public TelegramService(HttpClient httpClient, string botToken)
        {
            _httpClient = httpClient;
            _apiUrl = $"https://api.telegram.org/bot{botToken}/";
        }

        public async Task SendMessageAsync(string chatId, string message)
        {
            var payload = new
            {
                chat_id = chatId,
                text = message
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_apiUrl}sendMessage", content);
            response.EnsureSuccessStatusCode();
        }
    }
}

