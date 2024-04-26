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
        public List<string>? Project { get; set; }
        public List<string>? Components { get; set; }
        public string Comment { get; set; }
        public YandexTrackerIssue Issue { get; set; }

        /// <summary>
        /// Deadline
        /// </summary>
        public DateTime DueDate { get; set; }
        public string ParentRelated { get; set; }
    }

    /// <summary>
    /// Deserialization wrapper class for search response
    /// </summary>
    public class NotionSearchResponse
    {
        public NotionSearchResponse() { Results = new List<NotionObject>(); }
        public List<NotionObject> Results { get; set; }
    }
}