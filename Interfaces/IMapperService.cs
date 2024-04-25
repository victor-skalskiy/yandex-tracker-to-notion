using YandexTrackerToNotion.Domain;

namespace YandexTrackerToNotion.Interfaces
{
    public interface IMapperService
    {
        YandexTrackerIssue GetYandexTrackerObject(string jsonString);
        NotionObject GetNotionObject(string json);
        NotionObject GetNotionObject(YandexTrackerIssue issue);
        NotionComment GetNotionComment(NotionObject notionObject);
    }
}