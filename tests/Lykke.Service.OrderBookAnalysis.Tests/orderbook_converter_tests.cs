using System;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Service.OrderBookAnalysis.Services;
using Xunit;

namespace Lykke.Service.OrderBookAnalysis.Tests
{
    public sealed class orderbook_converter_tests
    {
        [Fact]
        public void mid_calculation()
        {
            var ts = DateTime.MaxValue;

            var ob = new OrderBook("", "", ts,
                asks: new[] {new OrderBookItem(100, 100)},
                bids: new[] {new OrderBookItem(80, 100)});

            var ss = OrderBookConverter.FromOrderBook(ts, ob);

            Assert.Equal(90, ss.Mid);
            Assert.Equal(100, ss.Ask);
            Assert.Equal(80, ss.Bid);
        }
    }
}

