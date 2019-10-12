using System;

namespace Taurit.Toolkit.FileProcessors.Exceptions
{
    [Serializable]
    public class InvalidConfigurationException : Exception
    {
        public InvalidConfigurationException(String message) : base(message)
        {
        }
    }
}