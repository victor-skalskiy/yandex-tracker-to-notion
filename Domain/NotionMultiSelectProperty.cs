using Newtonsoft.Json;

namespace YandexTrackerToNotion.Domain
{
    public class NotionMultiSelectProperty
    {
        public NotionMultiSelectProperty() { }

        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("color")]
        public string? Color { get; set; }
    }
}