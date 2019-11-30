using System;

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Services
{
    public class TimeConverter : ITimeConverter
    {
        public Int32 ConvertToUnitsOfWorkAndCeil(TimeSpan timeSpan, Int32 unitOfWorkDurationInMinutes = 45)
        {
            Double numberOfUnitsOfWorkNecessaryToCoverTheTimeSpan =
                ConvertToUnitsOfWork(timeSpan, unitOfWorkDurationInMinutes);
            var numberOfUnitsOfWorkAsInt = (Int32) Math.Ceiling(numberOfUnitsOfWorkNecessaryToCoverTheTimeSpan);
            return numberOfUnitsOfWorkAsInt;
        }

        public Double ConvertToUnitsOfWork(TimeSpan timeSpan, Int32 unitOfWorkDurationInMinutes = 45)
        {
            Int32 unitOfWorkDurationInSeconds = 60 * unitOfWorkDurationInMinutes;
            Double numberOfUnitsOfWorkNecessaryToCoverTheTimeSpan = timeSpan.TotalSeconds / unitOfWorkDurationInSeconds;

            return numberOfUnitsOfWorkNecessaryToCoverTheTimeSpan;
        }
    }
}