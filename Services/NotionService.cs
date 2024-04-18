using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using YandexTrackerToNotion.Domain;

namespace YandexTrackerToNotion.Services
{
    public class NotionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _databaseId;
        private readonly Mapper _mapper;

        // Конструктор принимает HttpClient и databaseId
        public NotionService(HttpClient httpClient, string databaseId, Mapper mapper)
        {
            _httpClient = httpClient;
            _databaseId = databaseId;
            _mapper = mapper;
        }

        private StringContent GetRequestContent(string jsonString)
        {
            return new StringContent(jsonString, Encoding.UTF8, "application/json");
        }

        public async Task<string> GetPageStruct()
        {
            var response = await _httpClient.GetAsync($"https://api.notion.com/v1/databases/{_databaseId}");
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

        async Task CreatePageAsync(NotionObject notionObject)
        {
            var response =
                await _httpClient.PostAsync(
                    "https://api.notion.com/v1/pages",
                    GetRequestContent(_mapper.GetNotionObjectJson(notionObject)));

            response.EnsureSuccessStatusCode();
        }

        static bool IsPageFound(string searchResponse)
        {
            var responseObject = JsonConvert.DeserializeObject<NotionSearchResponse>(searchResponse);
            return responseObject.Results != null && responseObject.Results.Any();
        }

        async Task<string> FindPageByTitleAsync(string search)
        {
            var query = new
            {
                filter = new
                {
                    property = "Title",
                    title = new
                    {
                        equals = search
                    }
                }
            };

            var json = JsonConvert.SerializeObject(query);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"https://api.notion.com/v1/databases/{_databaseId}/query", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to search for page by title: {responseContent}");
            }

            return responseContent;
        }

        public async Task CreatePageIfNotFoundAsync(NotionObject notionObject)
        {
            var searchResult = await FindPageByTitleAsync(notionObject.Title);
            if (!IsPageFound(searchResult))
                await CreatePageAsync(notionObject);
        }        
    }
}