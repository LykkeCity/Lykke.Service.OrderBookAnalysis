using System;

namespace Lykke.Service.OrderBookAnalysis.Services
{
    public static class Ticker
    {
        public static DateTime Round(DateTime dateTime, TimeSpan interval)
        {
            var fullPeriods = (int) Math.Round(dateTime.TimeOfDay / interval);
            return dateTime.Date + fullPeriods * interval;
        }

        public static TimeSpan GetNextTickInterval(DateTime now, TimeSpan interval)
        {
            var fullPeriods = (int) Math.Floor (now.TimeOfDay / interval);
            return interval * (fullPeriods + 1) - now.TimeOfDay;
        }
    }
}
