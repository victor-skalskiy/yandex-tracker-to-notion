namespace YandexTrackerToNotion.Domain
{
    public class NotionObject
    {
        public NotionObject()
        {
            //Components = new List<NotionMultiSelectProperty>();
        }

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
        //public List<NotionMultiSelectProperty> Components { get; set; }
        public List<string> Project { get; set; }
        public List<string> Components { get; set; }
    }

    /// <summary>
    /// Deserialization wrapper class for search response
    /// </summary>
    public class NotionSearchResponse
    {
        public List<NotionObject>? Results { get; set; }
    }
}