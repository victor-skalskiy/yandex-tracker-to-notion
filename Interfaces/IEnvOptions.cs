using System;
namespace YandexTrackerToNotion.Interfaces
{
	public interface IEnvOptions
	{
        string NotionDatabaseId { get; }
        string NotionIntergationToken { get; }
        string NotionAPIVersionTitle { get; }
        string NotionAPIVersionValue { get; }
        string TelegramBotToken { get; }
        string TelegramBotChatId { get; }
    }
}

