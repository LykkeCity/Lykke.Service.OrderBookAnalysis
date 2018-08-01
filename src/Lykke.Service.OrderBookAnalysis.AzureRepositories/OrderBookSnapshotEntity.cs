using System;
using System.Globalization;
using Lykke.AzureStorage.Tables;
using Lykke.Service.OrderBookAnalysis.Contracts;

namespace Lykke.Service.OrderBookAnalysis.AzureRepositories
{
    public sealed class OrderBookSnapshotEntity : AzureTableEntity, IOrderBookSnapshot
    {
        public string Exchange { get; set; }
        public string AssetPair { get; set; }

        DateTime IOrderBookSnapshot.Timestamp {
            get => base.Timestamp.DateTime;
            set => base.Timestamp = value;
        }

        public decimal Ask { get; set; }
        public decimal Bid { get; set; }
        public decimal Mid { get; set; }

        public int CountAskOrders { get; set; }
        public int CountBidOrders { get; set; }

        public decimal SumAskVolume { get; set; }
        public decimal SumBidVolume { get; set; }

        public decimal LastAsk { get; set; }
        public decimal LastBid { get; set; }

        public decimal AskVolume01 { get; set; }
        public decimal BidVolume01 { get; set; }
        public decimal AskVolume03 { get; set; }
        public decimal BidVolume03 { get; set; }
        public decimal AskVolume05 { get; set; }
        public decimal BidVolume05 { get; set; }
        public decimal AskVolume07 { get; set; }
        public decimal BidVolume07 { get; set; }
        public decimal AskVolume10 { get; set; }
        public decimal BidVolume10 { get; set; }
        public decimal AskVolume15 { get; set; }
        public decimal BidVolume15 { get; set; }
        public decimal AskVolume20 { get; set; }
        public decimal BidVolume20 { get; set; }
        public decimal AskVolume25 { get; set; }
        public decimal BidVolume25 { get; set; }
        public decimal AskVolume30 { get; set; }
        public decimal BidVolume30 { get; set; }
        public decimal AskVolume40 { get; set; }
        public decimal BidVolume40 { get; set; }
        public decimal AskVolume50 { get; set; }
        public decimal BidVolume50 { get; set; }

        public OrderBookSnapshotEntity()
        {
        }

        public static class ByOrderBook
        {
            public static string GeneratePartitionKey(DateTimeOffset ts)
            {
                return ts.ToString("s", CultureInfo.InvariantCulture);
            }

            public static string GenerateRowKey(string source, string asset)
            {
                return $"{source}_{asset}";
            }
        }
    }
}
