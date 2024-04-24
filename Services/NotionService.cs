﻿using System.Text;
using Newtonsoft.Json;
using YandexTrackerToNotion.Domain;
using YandexTrackerToNotion.Interfaces;

namespace YandexTrackerToNotion.Services
{
    public class NotionService : INotionService
    {
        private readonly HttpClient _httpClient;
        private readonly IMapperService _mapper;
        private readonly IEnvOptions _options;
        private readonly ITelegramService _telegramService;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public NotionService(HttpClient httpClient, IMapperService mapper, IEnvOptions options, ITelegramService telegramService)
        {
            _options = options;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.NotionIntergationToken);
            _httpClient.DefaultRequestHeaders.Add(_options.NotionAPIVersionTitle, _options.NotionAPIVersionValue);
            _mapper = mapper;
            _telegramService = telegramService;
        }

        private StringContent GetRequestContent(string jsonString)
        {
            return new StringContent(jsonString, Encoding.UTF8, "application/json");
        }

        async Task CreatePageAsync(NotionObject notionObject)
        {
            var response =
                await _httpClient.PostAsync(
                    "https://api.notion.com/v1/pages",
                    GetRequestContent(_mapper.GetNotionObjectJson(notionObject)));

            if (_options.IsDevMode)
                await _telegramService.SendMessageAsync($"{notionObject.Key} CreatePageAsync, response status code is {response.StatusCode}");

            response.EnsureSuccessStatusCode();
        }

        async Task UpdatePageAsync(string pageId, NotionObject notionObject)
        {
            var content = _mapper.GetNotionObjectJson(notionObject);
            var response =
                await _httpClient.PatchAsync(
                    $"https://api.notion.com/v1/pages/{pageId}",
                    GetRequestContent(content));

            if (_options.IsDevMode)
                await _telegramService.SendMessageAsync($"{notionObject.Key} UpdatePageAsync, response status code is {response.StatusCode}\r\nContent:\r\n{content}");

            response.EnsureSuccessStatusCode();
        }

        static bool IsPageFound(string searchResponse)
        {
            var responseObject = JsonConvert.DeserializeObject<NotionSearchResponse>(searchResponse);
            return responseObject.Results != null && responseObject.Results.Any();
        }

        async Task<string> FindPageByIDAsync(string ytId)
        {
            var query = new
            {
                filter = new
                {
                    property = "YTID",
                    rich_text = new
                    {
                        equals = ytId
                    }
                }
            };

            var content = GetRequestContent(JsonConvert.SerializeObject(query));
            var response = await _httpClient.PostAsync($"https://api.notion.com/v1/databases/{_options.NotionDatabaseId}/query", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to search for page by YTID: {responseContent}");
            }

            return responseContent;
        }

        public async Task CreateOrUpdatePageAsync(NotionObject notionObject)
        {
            await _semaphore.WaitAsync();
            try
            {
                var searchResult = await FindPageByIDAsync(notionObject.YTID);
                var findedItems = JsonConvert.DeserializeObject<NotionSearchResponse>(searchResult);

                if (IsPageFound(searchResult))
                {
                    if (_options.IsDevMode)
                        await _telegramService.SendMessageAsync($"{notionObject.Key} CreateOrUpdatePageAsync, object found in Notion db");

                    var pageId = findedItems.Results.First().Id;
                    await UpdatePageAsync(pageId, notionObject);
                }
                else
                {
                    if (_options.IsDevMode)
                        await _telegramService.SendMessageAsync($"{notionObject.Key} CreateOrUpdatePageAsync, object not found in Notion db, creating");

                    await CreatePageAsync(notionObject);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<string> GetPageStructAsync()
        {
            var response = await _httpClient.GetAsync($"https://api.notion.com/v1/databases/{_options.NotionDatabaseId}");
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
    }
}