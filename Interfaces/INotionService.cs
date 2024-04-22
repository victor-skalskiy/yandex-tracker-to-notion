using YandexTrackerToNotion.Domain;
using YandexTrackerToNotion.Services;

namespace YandexTrackerToNotion.Interfaces
{
    public interface INotionService
    {
        Task<string> GetPageStructAsync();
        Task CreateOrUpdatePageAsync(NotionObject notionObject);
    }
}