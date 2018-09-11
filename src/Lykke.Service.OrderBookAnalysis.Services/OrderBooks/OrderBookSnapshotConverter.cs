using System;
using System.Linq;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Service.OrderBookAnalysis.Contracts;

namespace Lykke.Service.OrderBookAnalysis.Services.OrderBooks
{
    public static class OrderBookSnapshotConverter
    {
        public static OrderBookSnapshot FromOrderBook(DateTime ts, OrderBook orderBook)
        {
            var mid = (orderBook.BestAskPrice + orderBook.BestBidPrice) / 2;

            decimal Ask(decimal percent)
            {
                decimal koef = percent / 10 / 100;
                var maxAsk = mid * (1 + koef / 2);
                return orderBook.AskLevels.Where(x => x.Key <= maxAsk).Sum(x => x.Value);
            }

            decimal Bid(decimal percent)
            {
                decimal koef = percent / 10 / 100;
                var minBid = mid * (1 - koef / 2);
                return orderBook.BidLevels.Where(x => x.Key >= minBid).Sum(x => x.Value);
            }

            return new OrderBookSnapshot
            {
                Exchange = orderBook.Source,
                AssetPair = orderBook.Asset,
                Timestamp = ts,

                CountAskOrders = orderBook.AskLevels.Count,
                CountBidOrders = orderBook.BidLevels.Count,

                SumAskVolume = orderBook.AskLevels.Sum(x => x.Value),
                SumBidVolume = orderBook.BidLevels.Sum(x => x.Value),

                Ask = orderBook.BestAskPrice,
                Bid = orderBook.BestBidPrice,
                Mid = mid,

                LastAsk = orderBook.AskLevels.Max(x => x.Key),
                LastBid = orderBook.BidLevels.Min(x => x.Key),

                AskVolume01 = Ask(01),
                BidVolume01 = Bid(01),
                AskVolume03 = Ask(03),
                BidVolume03 = Bid(03),
                AskVolume05 = Ask(05),
                BidVolume05 = Bid(05),
                AskVolume07 = Ask(07),
                BidVolume07 = Bid(07),
                AskVolume10 = Ask(10),
                BidVolume10 = Bid(10),
                AskVolume15 = Ask(15),
                BidVolume15 = Bid(15),
                AskVolume20 = Ask(20),
                BidVolume20 = Bid(20),
                AskVolume25 = Ask(25),
                BidVolume25 = Bid(25),
                AskVolume30 = Ask(30),
                BidVolume30 = Bid(30),
                AskVolume40 = Ask(40),
                BidVolume40 = Bid(40),
                AskVolume50 = Ask(50),
                BidVolume50 = Bid(50)
            };
        }
    }
}
