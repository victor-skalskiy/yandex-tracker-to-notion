using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using YandexTrackerToNotion.Services;

[Route("api/[controller]")]
[ApiController]
public class WebhookController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly TelegramService _telegramService;

    public WebhookController(HttpClient httpClient, TelegramService telegramService)
    {
        _telegramService = telegramService;    
        _httpClient = httpClient;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] dynamic data)
    {
        await _telegramService.SendMessageAsync(Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID"), $"catch the input packet: {data}");

        // Логика обработки данных из Yandex Tracker
        var notionPageId = await CreateOrUpdateNotionPage(data);
        return Ok(notionPageId);
    }

    private async Task<string> CreateOrUpdateNotionPage(dynamic trackerData)
    {
        // Построение запроса к Notion API
        var url = "https://api.notion.com/v1/pages";
        var notionRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(JsonConvert.SerializeObject(new
            {
                parent = new { database_id = "your-database-id" },
                properties = new
                {
                    Name = new { title = new[] { new { text = new { content = trackerData.title } } } },
                    Description = new { rich_text = new[] { new { text = new { content = trackerData.description } } } }
                }
            }), System.Text.Encoding.UTF8, "application/json")
        };
        notionRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "your-notion-token");

        var response = await _httpClient.SendAsync(notionRequest);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var page = JsonConvert.DeserializeObject<dynamic>(responseBody);
        return page.id;
    }
}