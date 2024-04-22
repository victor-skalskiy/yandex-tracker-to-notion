using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using YandexTrackerToNotion.Interfaces;

namespace YandexTrackerToNotion.Services
{
    public class TelegramService : ITelegramService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly string _chatId;
        private readonly IEnvOptions _options;

        public TelegramService(HttpClient httpClient, IEnvOptions options)
        {
            _options = options;
            _httpClient = httpClient;
            _apiUrl = $"https://api.telegram.org/bot{_options.TelegramBotToken}/";
            _chatId = _options.TelegramBotChatId;
        }

        public async Task SendMessageAsync(string message)
        {
            await SendMessageAsync(_chatId, message);
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

