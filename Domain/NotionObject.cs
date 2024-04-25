using Newtonsoft.Json;

namespace YandexTrackerToNotion.Domain
{
    public class NotionObject
    {
        public NotionObject() { }

        public string PageId { get; set; }
        public string DatabaseId { get; set; }
        public string? Key { get; set; }
        public string? Object { get; set; }
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? YTID { get; set; }
        public TimeSpan OriginalEstimation { get; set; }
        public TimeSpan Estimation { get; set; }
        public TimeSpan Spent { get; set; }
        public string? Emoji { get; set; }
        public string? Status { get; set; }
        public string? AssigneeUserId { get; set; }
        public List<string> Project { get; set; }
        public List<string> Components { get; set; }
        public string Comment { get; set; }
        public YandexTrackerIssue Issue { get; set; }

        public dynamic ToJSON()
        {
            //TODO: move it to the service once active development is complete
            return new
            {
                parent = new { database_id = DatabaseId },
                properties = new Dictionary<string, object>
                {
                    ["Name"] = new
                    {
                        title = new[] {
                                new { text = new { content = Title } }
                            }
                    },
                    ["Описание"] = new
                    {
                        rich_text = new[] {
                                new { text = new { content = Description } }
                            }
                    },
                    ["YTID"] = new
                    {
                        rich_text = new[] {
                                new { text = new { content = YTID } }
                            }
                    },
                    ["Отработано минут"] = new
                    {
                        number = Spent.TotalMinutes
                    },
                    ["Оценка (час)"] = new
                    {
                        number = Estimation.TotalMinutes
                    },
                    ["Оценка исходная (час)"] = new
                    {
                        number = OriginalEstimation.TotalMinutes
                    },
                    ["Статус"] = new
                    {
                        select = new { name = Status }
                    },
                    ["Ответственный"] = new
                    {
                        people = new[] { new { id = AssigneeUserId } }
                    },
                    ["Компоненты разработки"] = new
                    {
                        multi_select = Components.Select(c => new { name = c }).ToList()
                    },
                    ["Проект"] = new
                    {
                        multi_select = Project.Select(p => new { name = p }).ToList()
                    }
                },
                icon = new
                {
                    type = "emoji",
                    emoji = Emoji
                }
            };
        }

        public string ToJSONString() => JsonConvert.SerializeObject(ToJSON());

        public dynamic ToSearchJSON()
        {
            return new
            {
                filter = new
                {
                    property = "YTID",
                    rich_text = new
                    {
                        equals = YTID
                    }
                }
            };
        }

        public string ToSearchJSONString() => JsonConvert.SerializeObject(ToSearchJSON());
    }

    /// <summary>
    /// Deserialization wrapper class for search response
    /// </summary>
    public class NotionSearchResponse
    {
        public List<NotionObject>? Results { get; set; }
    }
}