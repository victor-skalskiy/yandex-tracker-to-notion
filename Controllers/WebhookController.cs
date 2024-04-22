using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using YandexTrackerToNotion.Services;
using YandexTrackerToNotion.Interfaces;
using YandexTrackerToNotion.Domain;

[Route("api/[controller]")]
[ApiController]
public class WebhookController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly ITelegramService _telegramService;
    private readonly INotionService _notionService;
    private readonly IMapperService _mapper;

    public WebhookController(HttpClient httpClient, ITelegramService telegramService, INotionService notionService, IMapperService mapper)
    {
        _telegramService = telegramService;
        _httpClient = httpClient;
        _notionService = notionService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] dynamic data)
    {        
        try
        {
            var notionObject = _mapper.YandexTrackerConvertToNotion($"{data}");
            await _notionService.CreateOrUpdatePageAsync(notionObject);
        }
        catch (Exception ex)
        {
            await _telegramService.SendMessageAsync($"/WebHook.Post exception: {ex.Message}\r\nSource: {ex.Source}\r\nStack trace: {ex.StackTrace}\r\nInput data: {data}");
        }

        return Ok();
    }

    [HttpGet("get-notion-database-properties")]
    public async Task<IActionResult> GetDatabaseProperties()
    {
        var properties = await _notionService.GetPageStructAsync();
        return Ok(properties);
    }

    [HttpGet("check-telegram")]
    public async Task<IActionResult> CheckTelegram()
    {
        await _telegramService.SendMessageAsync("Telegram check pass.");
        return Ok();
    }

}