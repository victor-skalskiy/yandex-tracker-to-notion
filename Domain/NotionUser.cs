using Newtonsoft.Json;

namespace YandexTrackerToNotion.Domain
{
    public class NotionUser
    {
        public NotionUser() { }

        public string Id { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public string Email { get; set; }

        [JsonProperty("YTAlias")]
        public string YandexTrackerAlias { get; set; }
        /// <summary>
        /// User type, can be 'person' or 'bot'
        /// </summary>
        public string Type { get; set; }
    }

    public class NotionUsersResponse
    {
        public List<NotionUser> Results { get; set; }
    }
}