using Lykke.Common.ExchangeAdapter.Contracts;

namespace Lykke.Service.OrderBookAnalysis.Services.MarketVolume
{
    public struct OrderBookWithCrossTickPrice
    {
        public bool CrossRevert;
        public OrderBook OrderBook;
        public TickPrice TickPrice;
        public int Decimals { get; set; }
    }
}
