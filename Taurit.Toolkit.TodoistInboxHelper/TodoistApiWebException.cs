using System;

namespace Taurit.Toolkit.TodoistInboxHelper
{
    public class TodoistApiWebException : Exception
    {
        public TodoistApiWebException(String message) : base(message)
        {
        }

        public TodoistApiWebException(String message, Exception innerException) : base(message, innerException)
        {
        }
    }
}