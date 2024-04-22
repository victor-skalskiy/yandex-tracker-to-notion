﻿using YandexTrackerToNotion.Interfaces;
using YandexTrackerToNotion.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EnvOptions>(builder.Configuration);

builder.Services
    .AddSingleton<IEnvOptions, EnvOptions>()
    .AddScoped<ITelegramService, TelegramService>()
    .AddScoped<INotionService, NotionService>()
    .AddSingleton<IMapperService, MapperService>();

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