using System.Text;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
            var content = notionObject.ToJSONString();

            var response =
                await _httpClient.PostAsync(
                    "https://api.notion.com/v1/pages",
                    GetRequestContent(content));

            if (_options.IsDevMode)
                await _telegramService.SendMessageAsync($"{notionObject.Key} CreatePageAsync, response status code is {response.StatusCode}\r\nContent:\r\n{content}");

            response.EnsureSuccessStatusCode();
        }

        async Task UpdatePageAsync(NotionObject notionObject)
        {
            var content = notionObject.ToJSONString();

            var response =
                await _httpClient.PatchAsync(
                    $"https://api.notion.com/v1/pages/{notionObject.PageId}",
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

        async Task<string> FindPageByIDAsync(NotionObject notionObject)
        {
            var content = GetRequestContent(notionObject.ToSearchJSONString());
            var response = await _httpClient.PostAsync($"https://api.notion.com/v1/databases/{_options.NotionDatabaseId}/query", content);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task CreateOrUpdatePageAsync(NotionObject notionObject)
        {
            await _semaphore.WaitAsync();
            try
            {
                var searchResult = await FindPageByIDAsync(notionObject);
                var findedItems = JsonConvert.DeserializeObject<NotionSearchResponse>(searchResult);

                if (IsPageFound(searchResult))
                {
                    if (_options.IsDevMode)
                        await _telegramService.SendMessageAsync($"{notionObject.Key} CreateOrUpdatePageAsync, object found in Notion db");

                    notionObject.PageId = findedItems.Results.First().Id;
                    await UpdatePageAsync(notionObject);

                    // add comment to updated item
                    if (!string.IsNullOrWhiteSpace(notionObject.Issue.CommentText))
                    {
                        if (_options.IsDevMode)
                            await _telegramService.SendMessageAsync($"{notionObject.Key} UpdatePageAsync, going to process comments");

                        var comment = _mapper.GetNotionComment(notionObject);
                        await AddCommentToPageAsync(comment);
                    }
                }
                else
                {
                    if (_options.IsDevMode)
                        await _telegramService.SendMessageAsync($"{notionObject.Key} CreateOrUpdatePageAsync, object not found in Notion db, creating");

                    await CreatePageAsync(notionObject);
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<string> GetPageStructAsync()
        {
            var response = await _httpClient.GetAsync($"https://api.notion.com/v1/databases/{_options.NotionDatabaseId}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private async Task AddCommentToPageAsync(NotionComment notionComment)
        {
            var content = notionComment.ToJSONString();

            var response = await _httpClient.PostAsync(
                                    "https://api.notion.com/v1/comments",
                                    GetRequestContent(content));

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                if (_options.IsDevMode)
                    await _telegramService.SendMessageAsync($"Failed to add comment to Notion page. Status: {response.StatusCode}. Server error: {errorContent}");

                SentrySdk.CaptureException(new Exception($"Failed to add comment: {errorContent}"));
            }
            else if (_options.IsDevMode)
            {
                await _telegramService.SendMessageAsync($"Comment added to Notion page {notionComment.PageId}");
            }

            response.EnsureSuccessStatusCode();
        }
    }
}