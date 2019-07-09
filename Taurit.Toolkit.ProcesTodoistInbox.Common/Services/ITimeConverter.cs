using System;

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Services
{
    public interface ITimeConverter
    {
        Int32 ConvertToUnitsOfWork(TimeSpan timeSpan, Int32 unitOfWorkDurationInMinutes = 45);
    }
}