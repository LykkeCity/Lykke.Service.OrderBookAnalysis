using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Service.OrderBookAnalysis.AzureRepositories;
using Lykke.Service.OrderBookAnalysis.Contract;
using Lykke.Service.OrderBookAnalysis.Services.OrderBooks;
using Lykke.Service.OrderBookAnalysis.Services.Settings;
using Lykke.Service.OrderBookAnalysis.Services.Tools;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Hosting;

namespace Lykke.Service.OrderBookAnalysis.Services.MarketVolume
{
    public class MarketVolumeHostedService : IHostedService
    {
        private readonly OrderBooksSourceSettings _settings;
        private readonly IObservable<Unit> _worker;
        private IDisposable _subscription;

        public MarketVolumeHostedService(
            TickPriceListener tpListener,
            OrderBookListener obListener,
            Ticker ticker,
            OrderBooksSourceSettings settings,
            IAzureRepository<MarketVolumeSnapshot> azureRepository,
            IRmqPublisher<MarketVolumeSnapshot> rmqPublisher)
        {
            _settings = settings;

            var combined = CombineWithTickPrices(
                    obListener.OrderBooks,
                    tpListener.TickPrices)
                .Merge();

            var snapshots = ticker.ApplyTicker(
                    combined,
                    x => new SnapshotKey {Asset = x.OrderBook.Asset, Source = x.OrderBook.Source})
                .Select(chunk => chunk.Select(
                    x => MarketVolumeSnapshotConverter.ConvertToSnapshot(
                        ticker.Round(DateTime.UtcNow),
                        x)))
                .RetryWithBackoff(TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(10))
                .Share();

            _worker = Observable.Merge(
                rmqPublisher.Publish(snapshots.SelectMany(x => x)),
                azureRepository.Publish(snapshots));
        }

        private IEnumerable<IObservable<OrderBookWithCrossTickPrice>> CombineWithTickPrices(
            IObservable<OrderBook> orderBooks,
            IReadOnlyDictionary<string, IObservable<TickPrice>> tickPrices)
        {
            foreach (var mvs in _settings.MarketVolume)
            {
                var ob = orderBooks.Where(x =>
                    string.Equals(x.Asset, mvs.Asset, StringComparison.InvariantCultureIgnoreCase));

                var tp = tickPrices[mvs.CrossRateExchange]
                    .Where(x =>
                        string.Equals(
                            x.Asset,
                            mvs.CrossAssetPair,
                            StringComparison.InvariantCultureIgnoreCase)

                        && string.Equals(
                            x.Source,
                            mvs.CrossRateSource,
                            StringComparison.InvariantCultureIgnoreCase));

                yield return Observable.CombineLatest(
                    ob,
                    tp,
                    (o, t) => new OrderBookWithCrossTickPrice(mvs.CrossRevert, o, t, mvs.Decimals));
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscription = _worker.Subscribe();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _subscription?.Dispose();
            return Task.CompletedTask;
        }
    }
}
