using System;

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Services
{
    public interface ITimeConverter
    {
        Double ConvertToUnitsOfWork(TimeSpan timeSpan, Int32 unitOfWorkDurationInMinutes = 45);
        Int32 ConvertToUnitsOfWorkAndCeil(TimeSpan timeSpan, Int32 unitOfWorkDurationInMinutes = 45);
    }
}