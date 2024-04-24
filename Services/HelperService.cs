using System.Text;
using YandexTrackerToNotion.Interfaces;

namespace YandexTrackerToNotion.Services
{
    public class HelperService : IHelperService
    {
        public HelperService() { }

        public string GetRequestFullBody(HttpRequest request, string postData)
        {
            var builder = new StringBuilder();

            builder.AppendLine("Received request:");
            builder.AppendLine($"Method: {request.Method}");
            builder.AppendLine($"Path: {request.Path}");

            // Добавляем заголовки
            builder.AppendLine("Headers:");
            foreach (var header in request.Headers)
            {
                builder.AppendLine($"{header.Key}: {header.Value}");
            }

            builder.AppendLine("PostData:");
            builder.AppendLine(postData);

            return builder.ToString();
        }
    }
}