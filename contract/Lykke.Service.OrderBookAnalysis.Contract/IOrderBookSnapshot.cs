using System;

namespace Lykke.Service.OrderBookAnalysis.Contracts
{
    public interface IOrderBookSnapshot
    {
        string Exchange { get; set; }
        string AssetPair { get; set; }

        DateTime Timestamp { get; set; }

        decimal Ask { get; set; }
        decimal Bid { get; set; }
        decimal Mid { get; set; }

        int CountAskOrders { get; set; }
        int CountBidOrders { get; set; }

        decimal SumAskVolume { get; set; }
        decimal SumBidVolume { get; set; }

        decimal LastAsk { get; set; }
        decimal LastBid { get; set; }

        decimal AskVolume01 { get; set; }
        decimal BidVolume01 { get; set; }
        decimal AskVolume03 { get; set; }
        decimal BidVolume03 { get; set; }
        decimal AskVolume05 { get; set; }
        decimal BidVolume05 { get; set; }
        decimal AskVolume07 { get; set; }
        decimal BidVolume07 { get; set; }
        decimal AskVolume10 { get; set; }
        decimal BidVolume10 { get; set; }
        decimal AskVolume15 { get; set; }
        decimal BidVolume15 { get; set; }
        decimal AskVolume20 { get; set; }
        decimal BidVolume20 { get; set; }
        decimal AskVolume25 { get; set; }
        decimal BidVolume25 { get; set; }
        decimal AskVolume30 { get; set; }
        decimal BidVolume30 { get; set; }
        decimal AskVolume40 { get; set; }
        decimal BidVolume40 { get; set; }
        decimal AskVolume50 { get; set; }
        decimal BidVolume50 { get; set; }
    }
}
