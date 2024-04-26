using Microsoft.AspNetCore.Mvc;
using YandexTrackerToNotion.Interfaces;
using YandexTrackerToNotion.Domain;
using YandexTrackerToNotion.Extentions;

[Route("api/[controller]")]
[ApiController]
public class WebhookController : ControllerBase
{
    private readonly ITelegramService _telegramService;
    private readonly INotionService _notionService;
    private readonly IMapperService _mapper;
    private readonly IEnvOptions _options;
    private readonly IHelperService _helperService;

    public WebhookController(ITelegramService telegramService, INotionService notionService, IMapperService mapper,
        IEnvOptions options, IHelperService helperService)
    {
        _telegramService = telegramService;
        _notionService = notionService;
        _mapper = mapper;
        _options = options;
        _helperService = helperService;
    }

    [TokenAuthorization]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] dynamic data)
    {
        try
        {
            YandexTrackerIssue ytpackage = _mapper.GetYandexTrackerObject($"{data}");
            if (_options.IsDevMode)
                await _telegramService.SendMessageAsync($"{ytpackage.Key} started, packet type: {ytpackage?.PacketType}\r\npacket data: {data}");

            var notionObject = _mapper.GetNotionObject(ytpackage);
            await _notionService.CreateOrUpdatePageAsync(notionObject);
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            await _telegramService.SendMessageAsync($"/WebHook.Post exception: {ex.Message}\r\nSource: {ex.Source}\r\nStack trace: {ex.StackTrace}\r\nRequest: {_helperService.GetRequestFullBody(Request, $"{data}")}");
        }

        return Ok();
    }

    [HttpGet("get-notion-database-properties")]
    public async Task<IActionResult> GetDatabaseProperties()
    {
        var result = "";
        try
        {
            result = await _notionService.GetPageStructAsync();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }

        return Ok(result);
    }

    [HttpGet("check-telegram")]
    public async Task<IActionResult> CheckTelegram()
    {
        try
        {
            await _telegramService.SendMessageAsync("Telegram check pass.");
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }

        return Ok();
    }
}