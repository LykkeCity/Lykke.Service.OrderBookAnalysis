using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Lykke.Common.ExchangeAdapter;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.OrderBookAnalysis.Services.Settings;
using Lykke.Service.OrderBookAnalysis.Services.Tools;

namespace Lykke.Service.OrderBookAnalysis.Services.MarketVolume
{
    public sealed class TickPriceListener
    {
        private readonly OrderBooksSourceSettings _settings;
        private readonly ILogFactory _lf;

        public TickPriceListener(OrderBooksSourceSettings settings, ILogFactory lf)
        {
            _settings = settings;
            _lf = lf;

            TickPrices = GetTickPrices().ToDictionary(x => x.Key, x => x.Value);
        }

        private IEnumerable<KeyValuePair<string, IObservable<TickPrice>>> GetTickPrices()
        {
            foreach (var exchange in _settings.MarketVolume.Select(x => x.CrossRateExchange).Distinct())
            {
                var settings = new RabbitMqSubscriptionSettings
                {
                    ConnectionString = _settings.ConnString,
                    ExchangeName = exchange,
                    QueueName = $"{exchange}.orderbook-analysis-{Guid.NewGuid()}",
                    IsDurable = false
                };

                yield return KeyValuePair.Create(exchange, RmqHelper.ReadAsJson<TickPrice>(settings, _lf)
                    .RetryWithBackoff(
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromMinutes(10))
                    .Share());
            }
        }

        public IReadOnlyDictionary<string, IObservable<TickPrice>> TickPrices { get; }
    }
}
