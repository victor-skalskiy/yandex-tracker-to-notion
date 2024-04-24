using System;
namespace YandexTrackerToNotion.Interfaces
{
    public interface ITelegramService
    {
        Task SendMessageAsync(string message);
        Task SendMessageAsync(string chatId, string message);
        void SendMessage(string message);
        void SendMessage(string chatId, string message);
    }
}