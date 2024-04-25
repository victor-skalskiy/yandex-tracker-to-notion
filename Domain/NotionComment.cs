using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace YandexTrackerToNotion.Domain
{
    public class NotionComment
    {
        public NotionComment() { }

        public string PageId { get; set; }
        public string Id { get; set; }
        public string Text { get; set; }
        public NotionUser Author { get; set; }
        public string AuthorId { get; set; }

        public dynamic ToJSON()
        {
            //TODO: move it to the service once active development is complete
            return new
            {
                parent = new { page_id = PageId },
                rich_text = new[]
                {
                    Author.ToCommentJSON(),
                    new
                    {
                        type = "text",
                        text = new
                        {
                            content = $" комментирует:\r\n{Text}"
                        }
                    }
                }
            };
        }

        public string ToJSONString() => JsonConvert.SerializeObject(ToJSON());
    }
}