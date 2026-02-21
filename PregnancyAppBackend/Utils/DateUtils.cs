namespace PregnancyAppBackend.Utils
{
    public static class DateUtils
    {
        public static bool Has24HoursPassed(DateTime lastDateUtc)
        {
            return DateTime.UtcNow - lastDateUtc >= TimeSpan.FromHours(24);
        }
        
        public static bool HasWeekPassed(DateTime lastDateUtc)
        {
            return DateTime.UtcNow - lastDateUtc >= TimeSpan.FromHours(24 * 7);
        }
    }
}