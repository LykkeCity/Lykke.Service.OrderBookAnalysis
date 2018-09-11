using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.OrderBookAnalysis.Contract;

namespace Lykke.Service.OrderBookAnalysis.Services.MarketVolume
{
    public static class MarketVolumeSnapshotConverter
    {
        public static MarketVolumeSnapshot ConvertToSnapshot(DateTime ts, OrderBookWithCrossTickPrice snapshot)
        {
            var crossMid = (snapshot.TickPrice.Ask + snapshot.TickPrice.Bid) / 2;

            decimal TargetVolume(decimal amount)
            {
                return snapshot.CrossRevert ? amount * crossMid : amount / crossMid;
            }

            decimal Buy(decimal amount)
            {
                return Math.Round(
                    WeightedPrice(Fill(TargetVolume(amount), snapshot.OrderBook.AskLevels).ToArray()),
                    snapshot.Decimals);
            }

            decimal Sell(decimal amount)
            {
                return Math.Round(
                    WeightedPrice(Fill(TargetVolume(amount), snapshot.OrderBook.BidLevels).ToArray()),
                    snapshot.Decimals);
            }

            return new MarketVolumeSnapshot
            {
                Exchange = snapshot.OrderBook.Source,
                AssetPair = snapshot.OrderBook.Asset,
                CrossAssetPair = snapshot.TickPrice.Asset,
                Timestamp = ts,
                Ask = snapshot.OrderBook.BestAskPrice,
                Bid = snapshot.OrderBook.BestBidPrice,
                Mid = Math.Round(
                    (snapshot.OrderBook.BestAskPrice + snapshot.OrderBook.BestBidPrice) / 2,
                    snapshot.Decimals),
                CrossAsk = snapshot.TickPrice.Ask,
                CrossBid = snapshot.TickPrice.Bid,
                CrossMid = Math.Round(crossMid, snapshot.Decimals),

                SumBuyVolume = snapshot.OrderBook.AskLevels.Sum(x => x.Value),
                SumSellVolume = snapshot.OrderBook.BidLevels.Sum(x => x.Value),

                Buy1000 = Buy(1000),
                Sell1000 = Sell(1000),
                Buy10000 = Buy(10000),
                Sell10000 = Sell(10000),
                Buy20000 = Buy(20000),
                Sell20000 = Sell(20000),
                Buy30000 = Buy(30000),
                Sell30000 = Sell(30000),
                Buy40000 = Buy(40000),
                Sell40000 = Sell(40000),
                Buy50000 = Buy(50000),
                Sell50000 = Sell(50000),
            };
        }

        public static decimal WeightedPrice(IReadOnlyCollection<KeyValuePair<decimal, decimal>> orders)
        {
            return orders.Sum(x => x.Key * x.Value) / orders.Sum(x => x.Value);
        }

        public static IEnumerable<KeyValuePair<decimal, decimal>> Fill(
            decimal targetVolume,
            IEnumerable<KeyValuePair<decimal, decimal>> orders)
        {
            foreach (var order in orders)
            {
                if (targetVolume <= 0) yield break;

                yield return KeyValuePair.Create(order.Key, Math.Min(order.Value, targetVolume));

                targetVolume -= order.Value;
            }
        }
    }
}
