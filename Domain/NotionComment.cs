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
    }
}