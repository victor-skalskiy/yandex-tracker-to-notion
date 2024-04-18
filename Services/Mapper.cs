using System;
using Newtonsoft.Json;
using System.Text;
using YandexTrackerToNotion.Domain;

namespace YandexTrackerToNotion.Services
{
    public class Mapper
    {
        private readonly string _databaseId;
        private readonly HttpClient _httpClient;

        public Mapper(HttpClient httpClient, string notionDatabaseId)
        {
            _databaseId = notionDatabaseId;
            _httpClient = httpClient;
        }

        public string GetNotionObjectJson(NotionObject notionObject)
        {
            var json = JsonConvert.SerializeObject(
                new
                {
                    parent = new { database_id = _databaseId },
                    properties = new
                    {
                        Title = new
                        {
                            title = new[]
                        {
                        new { text = new { content = notionObject.Title } }
                    }
                        },
                        Description = new
                        {
                            rich_text = new[]
                        {
                        new { text = new { content = notionObject.Description } }
                    }
                        }
                    }
                });

            return json;
        }

        public YandexTrackerIssue GetYandexTrackerObject(string jsonString)
        {
            var result = JsonConvert.DeserializeObject<YandexTrackerIssue>(jsonString);
            return result;
        }

        public NotionObject YandexTrackerConvertToNotion(string json)
        {
            return YandexTrackerConvertToNotion(GetYandexTrackerObject(json));
        }

        public NotionObject YandexTrackerConvertToNotion(YandexTrackerIssue issue)
        {
            return new NotionObject
            {
                Title = $"{issue.Key} : {issue.Summary}",
                Description = issue.Description
            };
        }

        public string Mapping(string jsonString)
        {
            var yTObject = GetYandexTrackerObject(jsonString);
            var notionObject = YandexTrackerConvertToNotion(yTObject);
            return GetNotionObjectJson(notionObject);
        }
    }
}