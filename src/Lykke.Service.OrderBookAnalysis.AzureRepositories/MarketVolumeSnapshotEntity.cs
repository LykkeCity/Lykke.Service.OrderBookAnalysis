using System;
using System.Globalization;
using Lykke.AzureStorage.Tables;
using Lykke.Service.OrderBookAnalysis.Contract;

namespace Lykke.Service.OrderBookAnalysis.AzureRepositories
{
    public sealed class MarketVolumeSnapshotEntity : AzureTableEntity, IMarketVolumeSnapshot
    {
        public string Exchange { get; set; }
        public string AssetPair { get; set; }
        public string CrossAssetPair { get; set; }
        DateTime IMarketVolumeSnapshot.Timestamp {
            get => base.Timestamp.DateTime;
            set => base.Timestamp = value;
        }
        public decimal Ask { get; set; }
        public decimal Bid { get; set; }
        public decimal Mid { get; set; }
        public decimal CrossAsk { get; set; }
        public decimal CrossBid { get; set; }
        public decimal CrossMid { get; set; }
        public decimal Buy1000 { get; set; }
        public decimal Sell1000 { get; set; }
        public decimal Buy10000 { get; set; }
        public decimal Sell10000 { get; set; }
        public decimal Buy20000 { get; set; }
        public decimal Sell20000 { get; set; }
        public decimal Buy30000 { get; set; }
        public decimal Sell30000 { get; set; }
        public decimal Buy40000 { get; set; }
        public decimal Sell40000 { get; set; }
        public decimal Buy50000 { get; set; }
        public decimal Sell50000 { get; set; }
        public decimal SumBuyVolume { get; set; }
        public decimal SumSellVolume { get; set; }

        public static class BySnapshot
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
