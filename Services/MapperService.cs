using Newtonsoft.Json;
using YandexTrackerToNotion.Domain;
using YandexTrackerToNotion.Interfaces;
using YandexTrackerToNotion.Extentions;

namespace YandexTrackerToNotion.Services
{
    public class MapperService : IMapperService
    {
        private readonly string _databaseId;
        private readonly HttpClient _httpClient;
        private readonly IEnvOptions _options;
        private static Dictionary<string, NotionStatus> statusList;
        private readonly ITelegramService _telegramService;
        private readonly NotionStatus _unknownNotionStatus;

        public MapperService(HttpClient httpClient, IEnvOptions options, ITelegramService telegramService)
        {
            _options = options;
            _databaseId = _options.NotionDatabaseId;
            _httpClient = httpClient;
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

            return finded;
        }

        string GetAssigneeId(YandexTrackerIssue issue)
        {
            var search = string.IsNullOrWhiteSpace(issue.Assignee) ? issue.Author : issue.Assignee;
            return GetNotionUser(search)?.Id ?? string.Empty;
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
            return result is null
                ? throw new Exception($"GetYandexTrackerObject: can't deserialize object by json: {jsonString}")
                : result;
        }

        public NotionObject GetNotionObject(string json)
        {
            return GetNotionObject(GetYandexTrackerObject(json));
        }

        public NotionObject GetNotionObject(YandexTrackerIssue issue)
        {
            CalculateEstimates(issue, out TimeSpan estimate, out TimeSpan originalEstimate);
            var status = statusList.ContainsKey(issue.Status) ? statusList[issue.Status] : _unknownNotionStatus;

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
                AssigneeUserId = GetAssigneeId(issue),
                Project = new List<string> { issue.Project },
                Components = issue.Components.Split(new[] { ',' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList()
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
    }
}