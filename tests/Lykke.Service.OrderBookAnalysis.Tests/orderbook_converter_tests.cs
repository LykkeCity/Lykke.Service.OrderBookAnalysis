using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Service.OrderBookAnalysis.Services.MarketVolume;
using Lykke.Service.OrderBookAnalysis.Services.OrderBooks;
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

            var ss = OrderBookSnapshotConverter.FromOrderBook(ts, ob);

            Assert.Equal(90, ss.Mid);
            Assert.Equal(100, ss.Ask);
            Assert.Equal(80, ss.Bid);
        }

        [Fact]
        public void fill_calculation()
        {
            var orders = new Dictionary<decimal, decimal>
            {
                { 500, 200 },
                { 600, 200 },
                { 700, 200 }
            };

            var fill400 = MarketVolumeSnapshotConverter.Fill(400, orders).ToArray();

            Assert.Equal(2, fill400.Length);
            Assert.Equal(200, fill400[1].Value);
            Assert.Equal(600, fill400[1].Key);
            Assert.Equal(550, MarketVolumeSnapshotConverter.WeightedPrice(fill400));

            var fill500 = MarketVolumeSnapshotConverter.Fill(500, orders).ToArray();

            Assert.Equal(3, fill500.Length);
            Assert.Equal(100, fill500[2].Value);
            Assert.Equal(700, fill500[2].Key);
            Assert.Equal(580, MarketVolumeSnapshotConverter.WeightedPrice(fill500));
        }
    }


}

