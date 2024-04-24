using Newtonsoft.Json;
using YandexTrackerToNotion.Domain;
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

            if (!bool.TryParse(configuration["IS_DEV_MODE"], out bool _isDevMode))
                throw new Exception("Environment variable IS_DEV_MODE is unset.");
            IsDevMode = _isDevMode;

            YandexTrackerAuthorizationToken = configuration["YANDEX_TRACKER_AUTHORIZATION_TOKEN"];
            if (string.IsNullOrWhiteSpace(YandexTrackerAuthorizationToken))
                throw new Exception("Environment variable YANDEX_TRACKER_AUTHORIZATION_TOKEN is unset.");

            if (!bool.TryParse(configuration["DISABLE_AUTHORIZATION"], out bool _disableAuth))
                throw new Exception("Environment variable DISABLE_AUTHORIZATION is unset.");
            DisableAuth = _disableAuth; 

            LoadNotionUserDB();
        }

        void LoadNotionUserDB()
        {
            var filePath = "NotionUserDB.json";
            if (!File.Exists(filePath))
                throw new Exception("NotionUserDB.josn not found");

            using (var reader = new StreamReader(filePath))
            {
                string jsonContent = reader.ReadToEnd();
                NotionUsers = JsonConvert.DeserializeObject<List<NotionUser>>(jsonContent);
            }
        }

        public string NotionDatabaseId { get; }
        public string NotionIntergationToken { get; }
        public string NotionAPIVersionTitle { get; }
        public string NotionAPIVersionValue { get; }
        public string TelegramBotToken { get; }
        public string TelegramBotChatId { get; }
        public string YandexTrackerAuthorizationToken { get; }
        public bool IsDevMode { get; }
        public bool DisableAuth { get; }
        public List<NotionUser> NotionUsers { get; set; }
    }
}