using System;
using Apartment.Common;
using Apartment.Common.Workers;

namespace Apartment.Core
{
    public class ConsoleLogger : ILogger
    {
        private readonly object _locker = new object();

        private string GetUtcNowShortString()
        {
            var now = DateTime.UtcNow;
            return $"{now:yyyy.MM.dd} {now:HH:mm:ss} UTC";
        }

        public void Error(string text, Exception ex = null)
        {
            lock (_locker)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"{GetUtcNowShortString()} [Error]: {text}{(ex == null ? "" : $"\n{ex.Message}: {ex.StackTrace}")}");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        public void Info(string text)
        {
            lock (_locker)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"{GetUtcNowShortString()} [Info]: {text}");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        public void Trace(string text)
        {
            lock (_locker)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{GetUtcNowShortString()} [Trace]: {text}");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        public void Warning(string text)
        {
            lock (_locker)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"{GetUtcNowShortString()} [Warning]: {text}");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    }
}