using System;

namespace Taurit.Toolkit.FileProcessors
{
    public interface ILoggingStrategy
    {
        void LogSuggestion(String message);
        void LogAction(String message);
    }
}