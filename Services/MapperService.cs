using Newtonsoft.Json;
using YandexTrackerToNotion.Domain;
using YandexTrackerToNotion.Interfaces;
using YandexTrackerToNotion.Extentions;

namespace YandexTrackerToNotion.Services
{
    public class MapperService : IMapperService
    {
        private readonly string _databaseId;
        private readonly IEnvOptions _options;
        private static Dictionary<string, NotionStatus> statusList;
        private readonly ITelegramService _telegramService;
        private readonly NotionStatus _unknownNotionStatus;

        public MapperService(IEnvOptions options, ITelegramService telegramService)
        {
            _options = options;
            _databaseId = _options.NotionDatabaseId;
            _telegramService = telegramService;
            _unknownNotionStatus = MapStatuses();
        }

        static void CalculateEstimates(YandexTrackerIssue issue, out TimeSpan estimateTime, out TimeSpan originalEstimateTime)
        {
            estimateTime = TimeParser.ParseRussianTimeString(issue.Estimation);
            originalEstimateTime = TimeParser.ParseRussianTimeString(issue.OriginalEstimation);

            if (originalEstimateTime.TotalMinutes > 0 && estimateTime.TotalMinutes == 0)
            {
                estimateTime = originalEstimateTime;
                originalEstimateTime = TimeSpan.Zero;
            }
        }

        NotionUser? GetNotionUser(string yandexTrackerUserName)
        {
            var finded = _options.NotionUsers
                .Where(x => x.Name == yandexTrackerUserName || x.YandexTrackerAlias == yandexTrackerUserName)
                .FirstOrDefault();

            if (finded is null && _options.IsDevMode)
                _telegramService.SendMessage($"YandexTracker user named '{yandexTrackerUserName}' not founded in Notion users DB");
            else if (finded is null)
                SentrySdk.CaptureMessage($"YandexTracker user named '{yandexTrackerUserName}' not founded in Notion users DB", SentryLevel.Error);

            return finded;
        }

        NotionStatus MapStatuses()
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
                { "Решен", new NotionStatus { Status = "Решено", Emoji = "✅" } },
                { "Закрыт", new NotionStatus { Status = "Завершена", Emoji = "🏁" } }
            };

            return new NotionStatus { Status = "Неизвестно", Emoji = "❓" };
        }

        public YandexTrackerIssue GetYandexTrackerObject(string jsonString)
        {
            var result = JsonConvert.DeserializeObject<YandexTrackerIssue>(jsonString);

            if (result is null)
                throw new Exception($"GetYandexTrackerObject: can't deserialize json object");

            return result;
        }

        public NotionObject GetNotionObject(string json)
        {
            return GetNotionObject(GetYandexTrackerObject(json));
        }

        public NotionObject GetNotionObject(YandexTrackerIssue issue)
        {
            CalculateEstimates(issue, out TimeSpan estimate, out TimeSpan originalEstimate);
            var status = statusList.ContainsKey(issue.Status) ? statusList[issue.Status] : _unknownNotionStatus;
            var components = issue.Components.Split(new[] { ',' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
            var searchAssigneeUser = string.IsNullOrWhiteSpace(issue.Assignee) ? issue.Author : issue.Assignee;

            return new NotionObject
            {
                Issue = issue,
                DatabaseId = _databaseId,
                Key = issue.Key,
                Title = $"{issue.Key} : {issue.Summary}",
                Description = issue.Description,
                YTID = issue.Id,
                Estimation = estimate,
                Spent = TimeParser.ParseRussianTimeString(issue.Spent),
                OriginalEstimation = originalEstimate,
                Status = status.Status,
                Emoji = status.Emoji,
                AssigneeUserId = GetNotionUser(searchAssigneeUser)?.Id ?? string.Empty,
                Project = string.IsNullOrWhiteSpace(issue.Project) ? null : new List<string> { issue.Project },
                Components = components.Any() ? components : null
            };
        }

        public NotionComment GetNotionComment(NotionObject notionObject)
        {
            var author = GetNotionUser(notionObject.Issue.CommentAuthor);

            if (author is null)
                throw new Exception($"Can't find User {notionObject.Issue.CommentAuthor} for add comment.");

            return new NotionComment
            {
                AuthorId = notionObject.Issue.CommentAuthor,
                Author = author,
                PageId = notionObject.PageId,
                Text = notionObject.Issue.CommentText,
                Id = notionObject.Issue.CommentId
            };
        }

        public string SerializeNotionObject(NotionObject notionObject)
        {
            var result = new
            {
                parent = new { database_id = notionObject.DatabaseId },
                properties = new Dictionary<string, dynamic>
                {
                    ["Name"] = new
                    {
                        title = new[] { new { text = new { content = notionObject.Title } } }
                    },
                    ["Описание"] = new
                    {
                        rich_text = new[] { new { text = new { content = notionObject.Description } } }
                    },
                    ["YTID"] = new
                    {
                        rich_text = new[] { new { text = new { content = notionObject.YTID } } }
                    },
                    ["Отработано минут"] = new
                    {
                        number = notionObject.Spent.TotalMinutes
                    },
                    ["Оценка (час)"] = new
                    {
                        number = notionObject.Estimation.TotalMinutes
                    },
                    ["Оценка исходная (час)"] = new
                    {
                        number = notionObject.OriginalEstimation.TotalMinutes
                    },
                    ["Статус"] = new
                    {
                        select = new { name = notionObject.Status }
                    },
                    ["Ответственный"] = new
                    {
                        people = new[] { new { id = notionObject.AssigneeUserId } }
                    }
                },
                icon = new
                {
                    type = "emoji",
                    emoji = notionObject.Emoji
                }
            };

            if (notionObject.Components is not null)
            {
                result.properties.Add("Компоненты разработки", new
                {
                    multi_select = notionObject.Components.Select(c => new { name = c }).ToList()
                });
            }

            if (notionObject.Project is not null)
            {
                result.properties.Add("Проект", new
                {
                    multi_select = notionObject.Project.Select(p => new { name = p }).ToList()
                });
            }

            return JsonConvert.SerializeObject(result);
        }

        public string SerializeNotionSearchObject(NotionObject notionObject)
            => JsonConvert.SerializeObject(new
            {
                filter = new
                {
                    property = "YTID",
                    rich_text = new
                    {
                        equals = notionObject.YTID
                    }
                }
            });

        public string SerializeNotionComment(NotionComment notionComment)
            => JsonConvert.SerializeObject(new
            {
                parent = new { page_id = notionComment.PageId },
                rich_text = new[]
                {
                    ConvertNotionUser(notionComment.Author),
                    new
                    {
                        type = "text",
                        text = new
                        {
                            content = $" комментирует:\r\n{notionComment.Text}"
                        }
                    }
                }
            });


        dynamic ConvertNotionUser(NotionUser notionUser)
            => new
            {
                type = "text",
                text = new
                {
                    content = notionUser.Name
                },
                annotations = new
                {
                    bold = true, // Жирный текст
                    color = "yellow_background" // Желтый фон
                }
            };
    }
}