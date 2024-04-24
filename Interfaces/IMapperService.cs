using YandexTrackerToNotion.Domain;

namespace YandexTrackerToNotion.Interfaces
{
    public interface IMapperService
    {
        string GetNotionObjectJson(NotionObject notionObject);
        YandexTrackerIssue GetYandexTrackerObject(string jsonString);
        NotionObject YandexTrackerConvertToNotion(string json);
        NotionObject YandexTrackerConvertToNotion(YandexTrackerIssue issue);
        string Mapping(string jsonString);
    }
}