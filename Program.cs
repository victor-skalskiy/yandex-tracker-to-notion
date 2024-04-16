﻿using YandexTrackerToNotion.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddHttpClient();

services.AddControllers();
services.AddHttpClient<TelegramService>();
services.AddSingleton(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient();
    var botToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
    return new TelegramService(httpClient, botToken);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

