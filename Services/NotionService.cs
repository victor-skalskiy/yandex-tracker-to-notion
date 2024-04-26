using System.Text;
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
            var content = _mapper.SerializeNotionObject(notionObject);

            var response =
                await _httpClient.PostAsync(
                    "https://api.notion.com/v1/pages",
                    GetRequestContent(content));

            if (_options.IsDevMode && !response.IsSuccessStatusCode)
                await _telegramService.SendMessageAsync($"{notionObject.Key} CreatePageAsync, response status code is {response.StatusCode}\r\nContent:\r\n{content}");

            response.EnsureSuccessStatusCode();
        }

        async Task UpdatePageAsync(NotionObject notionObject)
        {
            var content = _mapper.SerializeNotionObject(notionObject);

            var response =
                await _httpClient.PatchAsync(
                    $"https://api.notion.com/v1/pages/{notionObject.PageId}",
                    GetRequestContent(content));

            if (_options.IsDevMode && !response.IsSuccessStatusCode)
                await _telegramService.SendMessageAsync($"{notionObject.Key} UpdatePageAsync, response status code is {response.StatusCode}\r\nContent:\r\n{content}");

            response.EnsureSuccessStatusCode();
        }

        async Task<string> FindPageByIDAsync(NotionObject notionObject)
        {
            var content = GetRequestContent(_mapper.SerializeNotionSearchObject(notionObject));
            var response = await _httpClient.PostAsync($"https://api.notion.com/v1/databases/{_options.NotionDatabaseId}/query", content);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task CreateOrUpdatePageAsync(NotionObject notionObject)
        {
            await _semaphore.WaitAsync();
            try
            {
                var findedItems = JsonConvert.DeserializeObject<NotionSearchResponse>(
                                    await FindPageByIDAsync(notionObject))?.Results ?? new List<NotionObject>();

                if (findedItems.Any())
                {
                    notionObject.PageId = findedItems.First().Id;
                    await UpdatePageAsync(notionObject);

                    // add comment to updated item
                    if (!string.IsNullOrWhiteSpace(notionObject.Issue.CommentText))
                    {
                        var comment = _mapper.GetNotionComment(notionObject);
                        await AddCommentToPageAsync(comment);
                    }
                }
                else
                {
                    await CreatePageAsync(notionObject);
                }
            }
            catch (Exception)
            {                
                _semaphore.Release();
                throw;
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
            var content = _mapper.SerializeNotionComment(notionComment);

            var response = await _httpClient.PostAsync(
                                    "https://api.notion.com/v1/comments",
                                    GetRequestContent(content));

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                if (_options.IsDevMode)
                    await _telegramService.SendMessageAsync($"AddCommentToPageAsync: Failed to add comment to Notion page. Status: {response.StatusCode}.\r\nServer error: {errorContent}\r\nContent: {content}");

                SentrySdk.CaptureMessage($"AddCommentToPageAsync: Failed to add comment: {errorContent}, content: {content}", SentryLevel.Error);
            }            

            response.EnsureSuccessStatusCode();
        }
    }
}