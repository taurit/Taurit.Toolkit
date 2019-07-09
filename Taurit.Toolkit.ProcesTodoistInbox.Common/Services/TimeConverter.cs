using System;

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Services
{
    public class TimeConverter : ITimeConverter
    {
        public Int32 ConvertToUnitsOfWork(TimeSpan timeSpan, Int32 unitOfWorkDurationInMinutes = 45)
        {
            Int32 unitOfWorkDurationInSeconds = 60 * unitOfWorkDurationInMinutes;
            Double numberOfUnitsOfWorkNecessaryToCoverTheTimeSpan = timeSpan.TotalSeconds / unitOfWorkDurationInSeconds;
            var numberOfUnitsOfWorkAsInt = (Int32) Math.Ceiling(numberOfUnitsOfWorkNecessaryToCoverTheTimeSpan);
            return numberOfUnitsOfWorkAsInt;
        }
    }
}