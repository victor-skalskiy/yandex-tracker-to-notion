using System;
using YandexTrackerToNotion.Interfaces;

namespace YandexTrackerToNotion.Services
{
    public class EnvOptions : IEnvOptions
    {        
        public EnvOptions(IConfiguration configuration)
        {
            NotionDatabaseId = configuration["NOTION_DATABASE_ID"];
            if (string.IsNullOrWhiteSpace(NotionDatabaseId))
                throw new Exception("Environment variable NOTION_DATABASE_ID is unset.");

            NotionIntergationToken = configuration["NOTION_INTEGRATION_TOKEN"];
            if (string.IsNullOrWhiteSpace(NotionIntergationToken))
                throw new Exception("Environment variable NOTION_INTEGRATION_TOKEN is unset.");

            NotionAPIVersionTitle = "Notion-Version";
            NotionAPIVersionValue = "2022-06-28";

            TelegramBotToken = configuration["TELEGRAM_BOT_TOKEN"];
            if (string.IsNullOrWhiteSpace(TelegramBotToken))
                throw new Exception("Environment variable TELEGRAM_BOT_TOKEN is unset.");

            TelegramBotChatId = configuration["TELEGRAM_CHAT_ID"];
            if (string.IsNullOrWhiteSpace(TelegramBotChatId))
                throw new Exception("Environment variable TELEGRAM_CHAT_ID is unset.");
        }

        public string NotionDatabaseId { get; }
        public string NotionIntergationToken { get; }
        public string NotionAPIVersionTitle { get; }
        public string NotionAPIVersionValue { get; }
        public string TelegramBotToken { get; }
        public string TelegramBotChatId { get; }
    }
}

