using System;
using System.Text.RegularExpressions;

namespace YandexTrackerToNotion.Extentions
{
    public static class TimeParser
    {
        public static TimeSpan ParseRussianTimeString(string input)
        {
            var regex = new Regex(@"(\d+)\s(минут|час|дня|дней)");
            var match = regex.Match(input);

            if (!match.Success)
            {
                throw new ArgumentException("Некорректный ввод", nameof(input));
            }

            int quantity = int.Parse(match.Groups[1].Value);
            string type = match.Groups[2].Value;
            TimeSpan result;

            switch (type)
            {
                case "минут":
                    result = TimeSpan.FromMinutes(quantity);
                    break;
                case "час":
                    result = TimeSpan.FromHours(quantity);
                    break;
                case "дня":
                case "дней":
                    result = TimeSpan.FromDays(quantity);
                    break;
                default:
                    throw new NotImplementedException("Неизвестный тип интервала времени.");
            }
            return result;
        }
    }
}