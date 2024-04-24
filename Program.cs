using YandexTrackerToNotion.Interfaces;
using YandexTrackerToNotion.Services;

var builder = WebApplication.CreateBuilder(args);

SentrySdk.Init(options =>
{
    options.Dsn = builder.Configuration["SENTRY_DSN"];
    options.TracesSampleRate = 0.0;
    options.Debug = true;
    options.AutoSessionTracking = true;
    options.IsGlobalModeEnabled = false;
    options.EnableTracing = true;
});

builder.Services.Configure<EnvOptions>(builder.Configuration);

builder.Services
    .AddSingleton<IEnvOptions, EnvOptions>()
    .AddSingleton<ITelegramService, TelegramService>()
    .AddSingleton<INotionService, NotionService>()
    .AddSingleton<IMapperService, MapperService>()
    .AddSingleton<IHelperService, HelperService>();

builder.Services.AddHttpClient("TelegramClient");
builder.Services.AddHttpClient("MapperClient");
builder.Services.AddHttpClient("NotionClient");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();

app.Run();