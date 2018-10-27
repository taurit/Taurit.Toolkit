using System;

namespace Taurit.Toolkit.FileProcessors.Exceptions
{
    public class InvalidConfigurationException : Exception
    {
        public InvalidConfigurationException(String message) : base(message)
        {
        }
    }
}