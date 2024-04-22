using Newtonsoft.Json;
using YandexTrackerToNotion.Domain;
using YandexTrackerToNotion.Interfaces;
using YandexTrackerToNotion.Extentions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;

namespace YandexTrackerToNotion.Services
{
    public class MapperService : IMapperService
    {
        private readonly string _databaseId;
        private readonly HttpClient _httpClient;
        private readonly IEnvOptions _options;
        private static Dictionary<string, NotionStatus> statusList;

        public MapperService(HttpClient httpClient, IEnvOptions options)
        {
            _options = options;
            _databaseId = _options.NotionDatabaseId;
            _httpClient = httpClient;
            MapStatuses();
        }

        private static void CalculateEstimates(YandexTrackerIssue issue, out TimeSpan estimateTime, out TimeSpan originalEstimateTime)
        {
            estimateTime = TimeParser.ParseRussianTimeString(issue.Estimation);
            originalEstimateTime = TimeParser.ParseRussianTimeString(issue.OriginalEstimation);

            if (originalEstimateTime.TotalMinutes > 0 && estimateTime.TotalMinutes == 0)
            {
                estimateTime = originalEstimateTime;
                originalEstimateTime = TimeSpan.Zero;
            }
        }

        private static void MapStatuses()
        {
            statusList = new Dictionary<string, NotionStatus>
            {
                { "Открыт", new NotionStatus { Status = "Новая", Emoji = "🆕" } },
                { "В работе", new NotionStatus { Status = "В процессе", Emoji = "🏗️" } },
                { "Ревью", new NotionStatus { Status = "Ревью", Emoji = "🫵🏻" } },
                { "Готов к сборке", new NotionStatus { Status = "Готов к сборке", Emoji = "🤞🏻" } },
                { "Тестируется", new NotionStatus { Status = "Тестируется", Emoji = "🧑🏻‍🔬" } },
                { "Протестировано", new NotionStatus { Status = "Протестировано", Emoji = "🤪" } },
                { "Зарелизено", new NotionStatus { Status = "Зарелизено", Emoji = "🚀" } },
                { "Решено", new NotionStatus { Status = "Решено", Emoji = "✅" } },
                { "Закрыт", new NotionStatus { Status = "Решено", Emoji = "🏁" } }
            };
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
                            title = new[] {
                                new { text = new { content = notionObject.Title } }
                            }
                        },
                        Description = new
                        {
                            rich_text = new[] {
                                new { text = new { content = notionObject.Description } }
                            }
                        },
                        YTID = new
                        {
                            rich_text = new[] {
                                new { text = new { content = notionObject.YTID } }
                            }
                        },
                        SpendMinutes = new
                        {
                            number = notionObject.Spent.TotalMinutes
                        },
                        EstimateHour = new
                        {
                            number = notionObject.Estimation.TotalMinutes
                        },
                        OriginalEstimateHour = new
                        {
                            number = notionObject.OriginalEstimation.TotalMinutes
                        },
                        Status = new
                        {
                            select = new { name = notionObject.Status }
                        }
                    },
                    icon = new
                    {
                        type = "emoji",
                        emoji = notionObject.Emoji
                    }
                });

            return json;
        }

        public YandexTrackerIssue GetYandexTrackerObject(string jsonString)
        {
            var result = JsonConvert.DeserializeObject<YandexTrackerIssue>(jsonString);
            return result is null
                ? throw new Exception($"GetYandexTrackerObject: can't deserialize object by json: {jsonString}")
                : result;
        }

        public NotionObject YandexTrackerConvertToNotion(string json)
        {
            return YandexTrackerConvertToNotion(GetYandexTrackerObject(json));
        }

        public NotionObject YandexTrackerConvertToNotion(YandexTrackerIssue issue)
        {
            CalculateEstimates(issue, out TimeSpan estimate, out TimeSpan originalEstimate);
            var status = statusList.ContainsKey(issue.Status) ? statusList[issue.Status] : new NotionStatus() { Status = "Неизвестно" };

            //TODO: add a null check for issue.id etc
            return new NotionObject
            {
                Title = $"{issue.Key} : {issue.Summary}",
                Description = issue.Description,
                YTID = issue.Id,
                Estimation = estimate,
                Spent = TimeParser.ParseRussianTimeString(issue.Spent),
                OriginalEstimation = originalEstimate,
                Status = status.Status,
                Emoji =  status.Emoji
            };
        }

        public string Mapping(string jsonString)
        {
            return GetNotionObjectJson(
                        YandexTrackerConvertToNotion(
                            GetYandexTrackerObject(jsonString)));
        }
    }
}