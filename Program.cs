using Microsoft.Extensions.Configuration;
using YandexTrackerToNotion.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddHttpClient<TelegramService>();
services.AddSingleton(provider =>
{
    var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient();
    var botToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
    var chatId = Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID");
    return new TelegramService(httpClient, botToken, chatId);
});

var notionDatabaseId = Environment.GetEnvironmentVariable("NOTION_DATABASE_ID");

services.AddHttpClient<Mapper>();
services.AddSingleton(provider =>
{
    var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient();
    return new Mapper(httpClient, notionDatabaseId);
});


services.AddHttpClient<NotionService>();
services.AddSingleton(provider =>
{
    var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient();
    var notionIntegrationToken = Environment.GetEnvironmentVariable("NOTION_INTEGRATION_TOKEN");
    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", notionIntegrationToken);
    httpClient.DefaultRequestHeaders.Add("Notion-Version", "2022-02-22");
    return new NotionService(httpClient, notionDatabaseId, new Mapper(httpClient, notionDatabaseId));
});


services.AddControllers();

var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

