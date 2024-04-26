using YandexTrackerToNotion.Domain;

namespace YandexTrackerToNotion.Interfaces
{
    public interface INotionService
    {
        Task<string> GetPageStructAsync();
        Task CreateOrUpdatePageAsync(NotionObject notionObject);
        Task CheckLinkedItemsAsync(YandexTrackerIssue ytpackage, NotionObject notionObject);
    }
}