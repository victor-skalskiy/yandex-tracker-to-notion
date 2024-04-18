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
    private readonly NotionService _notionService;
    private readonly Mapper _mapper;

    public WebhookController(HttpClient httpClient, TelegramService telegramService, NotionService notionService, Mapper mapper)
    {
        _telegramService = telegramService;
        _httpClient = httpClient;
        _notionService = notionService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] dynamic data)
    {
        await _telegramService.SendMessageAsync($"catch the input packet: {data}");

        var notionObject = _mapper.YandexTrackerConvertToNotion($"{data}");
        await _notionService.CreatePageIfNotFoundAsync(notionObject);
        
        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetDatabaseProperties()
    {
        var properties = await _notionService.GetPageStruct();
        return Ok(properties);
    }
}