using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SchedulerService.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime RoundToNearestMinutes(this DateTime dt, int minutes, bool roundUp = true)
        {
            int remainder = dt.Minute % minutes;
            if (remainder == 0)
            {
                return dt;
            }

            int addMinutes = roundUp ? minutes - remainder : -remainder;
            var rounded =  dt.Add(new TimeSpan(0, addMinutes, 0));

            // Truncate DateTime to nearest minute
            return new DateTime(rounded.Ticks - (rounded.Ticks % TimeSpan.TicksPerMinute), rounded.Kind);
        }


        /// <summary>
        /// Returns all time slots falling between the range at quarters
        /// </summary>
        /// <param name="date">Start date of the range</param>
        /// <param name="fromTime">Start time on the given start date</param>
        /// <param name="toTime">End time of the </param>
        /// <returns>List of time slots within range</returns>
        public static IEnumerable<DateTime> GetSlotsForTimeRange(this DateTime fromDateTime, DateTime toDateTime)
        {
            var startDateTime = fromDateTime.RoundToNearestMinutes(15);
            var endTime = toDateTime.RoundToNearestMinutes(15, false);

            // Appointment slots are every 15 minutes (:00, :15, :30, :45)
            // Only slots within range are reserved (:59 => 00, :01 => :15)
            var numSlots = (int)((endTime.Subtract(startDateTime).TotalMinutes) / 15);

            return Enumerable.Range(0, numSlots)
                .Select(offset => startDateTime.AddMinutes(offset * 15));
        }
    }
}