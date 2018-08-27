using Lykke.Common.ExchangeAdapter.Contracts;

namespace Lykke.Service.OrderBookAnalysis.Services.MarketVolume
{
    public struct OrderBookWithCrossTickPrice
    {
        public OrderBookWithCrossTickPrice(bool crossRevert, OrderBook orderBook, TickPrice tickPrice, int decimals)
        {
            CrossRevert = crossRevert;
            OrderBook = orderBook;
            TickPrice = tickPrice;
            Decimals = decimals;
        }

        public readonly bool CrossRevert;
        public readonly OrderBook OrderBook;
        public readonly TickPrice TickPrice;
        public readonly int Decimals;
    }
}
