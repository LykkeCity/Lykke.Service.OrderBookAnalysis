using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Lykke.Common.ExchangeAdapter;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.ExchangeAdapter.Server;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.OrderBookAnalysis.Services.Settings;
using Lykke.Service.OrderBookAnalysis.Services.Tools;

namespace Lykke.Service.OrderBookAnalysis.Services.OrderBooks
{
    public sealed class OrderBookListener
    {
        private readonly OrderBooksSourceSettings _settings;
        private readonly ILogFactory _lf;

        public OrderBookListener(OrderBooksSourceSettings settings, ILogFactory lf)
        {
            _settings = settings;
            _lf = lf;

            var orderBooks = GetOrderBooks().Merge().Share();

            OrderBooks = Observable.Merge(
                    orderBooks.ReportStatistics(
                            TimeSpan.FromMinutes(1),
                            lf.CreateLog(this),
                            "OrderBooks received from RabbitMQ in the last {0} - {1}")
                        .Select(_ => (false, (OrderBook) null)),
                    orderBooks.Select(x => (true, x)))
                .Where(x => x.Item1)
                .Select(x => x.Item2)
                .Share();
        }

        private IEnumerable<IObservable<OrderBook>> GetOrderBooks()
        {
            var exchanges = _settings.Exchanges;

            foreach (var input in exchanges)
            {
                var settings = new RabbitMqSubscriptionSettings
                {
                    ConnectionString = _settings.ConnString,
                    ExchangeName = input,
                    QueueName = $"{input}.orderbook-analysis-{Guid.NewGuid()}",
                    IsDurable = false
                };

                yield return RmqHelper.ReadAsJson<OrderBook>(settings, _lf)
                    .RetryWithBackoff(
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromMinutes(10));
            }
        }

        public IObservable<OrderBook> OrderBooks { get; }
    }
}
