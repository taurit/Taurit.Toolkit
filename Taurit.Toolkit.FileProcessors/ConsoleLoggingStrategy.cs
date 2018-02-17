using System;

namespace Taurit.Toolkit.FileProcessors
{
    public class ConsoleLoggingStrategy : ILoggingStrategy
    {
        public enum LogLevel
        {
            Actions,
            ActionsAndSuggestions
        }

        private readonly LogLevel _level;

        public ConsoleLoggingStrategy(LogLevel level)
        {
            _level = level;
        }

        /// <inheritdoc />
        public void LogSuggestion(String message)
        {
            if (_level == LogLevel.ActionsAndSuggestions)
                Console.WriteLine(message);
        }

        /// <inheritdoc />
        public void LogAction(String message)
        {
            Console.WriteLine(message);
        }
    }
}