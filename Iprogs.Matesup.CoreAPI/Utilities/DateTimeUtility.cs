using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace Iprogs.Matesup.CoreAPI.Utilities
{
    public static class DateTimeUtility
    {
        public static DateOnly DateOnlyFromDateTime(DateTime dt)
        {
            return new DateOnly(dt.Year, dt.Month, dt.Day);
        }

        public static TimeOnly TimeOnlyFromDateTime(DateTime dt)
        {
            return new TimeOnly(dt.TimeOfDay.Ticks);
        }

        public static DateTime ToDateTime(DateOnly dt)
        {
            return new DateTime(dt, new TimeOnly(0));
        }
    }
}
