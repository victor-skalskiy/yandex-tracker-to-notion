using YandexTrackerToNotion.Domain;

namespace YandexTrackerToNotion.Interfaces
{
    public interface IMapperService
    {
        YandexTrackerIssue GetYandexTrackerObject(string jsonString);
        NotionObject GetNotionObject(string json);
        NotionObject GetNotionObject(YandexTrackerIssue issue);
        NotionComment GetNotionComment(NotionObject notionObject);
        string SerializeNotionObject(NotionObject notionObject);
        string SerializeNotionSearchObject(NotionObject notionObject);
        string SerializeNotionComment(NotionComment NotionComment);
    }
}