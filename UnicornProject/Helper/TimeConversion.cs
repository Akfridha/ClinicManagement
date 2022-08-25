using System;

namespace UnicornProject.Helper
{
    public static class TimeConversion
    {
        public static int calculateSeconds(DateTime dateTime)

        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);//from 1970/1/1 00:00:00 to now

            TimeSpan result = dateTime.Subtract(dt);

            int seconds = Convert.ToInt32(result.TotalSeconds);

            return seconds;

        }
    }
}
