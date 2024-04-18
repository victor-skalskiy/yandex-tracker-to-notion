using System;
namespace YandexTrackerToNotion.Domain
{
	public class NotionObject
    {
        public NotionObject() { Description = ""; }
        public string Object { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class NotionSearchResponse
    {
        public List<NotionObject> Results { get; set; }
    }
}

