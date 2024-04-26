using System.Text;
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

        StringContent GetContent(string chatId, string message)
        {
            var payload = new
            {
                chat_id = chatId,
                text = message
            };

            return new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
        }

        public async Task SendMessageAsync(string message)
        {
            await SendMessageAsync(_chatId, message);
        }

        public async Task SendMessageAsync(string chatId, string message)
        {            
            var response = await _httpClient.PostAsync($"{_apiUrl}sendMessage", GetContent(chatId, message));

            response.EnsureSuccessStatusCode();
        }

        public void SendMessage(string message)
        {
            SendMessage(_options.TelegramBotChatId, message);
        }

        public void SendMessage(string chatId, string message)
        {
            var response = _httpClient.PostAsync($"{_apiUrl}sendMessage", GetContent(chatId, message));

            if (!response.Result.IsSuccessStatusCode)
                throw new Exception($"TelegramService.SendMessage exception: {response.Result}");
        }
    }
}