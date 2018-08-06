using System;
using System.Globalization;
using Lykke.Service.OrderBookAnalysis.Services.Tools;
using Xunit;

namespace Lykke.Service.OrderBookAnalysis.Tests
{
    public sealed class ticker_tests
    {
        [Theory]
        [InlineData("01-01-2010 18:57:00", "00:03:00")]
        [InlineData("01-01-2010 18:50:00", "00:10:00")]
        [InlineData("01-01-2010 18:29:30", "00:00:30")]
        public void next_tick_interval(string now, string expected)
        {
            var ticker = new Ticker(TimeSpan.FromMinutes(10));

            var actual = ticker.GetNextTickInterval(DateTime.Parse(now, CultureInfo.InvariantCulture));

            Assert.Equal(TimeSpan.Parse(expected, CultureInfo.InvariantCulture), actual);
        }
    }
}
