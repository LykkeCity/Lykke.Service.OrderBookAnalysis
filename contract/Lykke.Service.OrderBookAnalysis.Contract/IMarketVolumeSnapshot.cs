using System;

namespace Lykke.Service.OrderBookAnalysis.Contract
{
    public interface IMarketVolumeSnapshot
    {
        string Exchange { get; set; }
        string AssetPair { get; set; }
        string CrossAssetPair { get; set; }
        DateTime Timestamp { get; set; }
        decimal Ask { get; set; }
        decimal Bid { get; set; }
        decimal Mid { get; set; }
        decimal CrossAsk { get; set; }
        decimal CrossBid { get; set; }
        decimal CrossMid { get; set; }

        decimal Buy1000 { get; set; }
        decimal Sell1000 { get; set; }
        decimal Buy10000 { get; set; }
        decimal Sell10000 { get; set; }
        decimal Buy20000 { get; set; }
        decimal Sell20000 { get; set; }
        decimal Buy30000 { get; set; }
        decimal Sell30000 { get; set; }
        decimal Buy40000 { get; set; }
        decimal Sell40000 { get; set; }
        decimal Buy50000 { get; set; }
        decimal Sell50000 { get; set; }
        decimal SumBuyVolume { get; set; }
        decimal SumSellVolume { get; set; }
    }
}
