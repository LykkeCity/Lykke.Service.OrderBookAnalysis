using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.ExchangeAdapter;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.ExchangeAdapter.Server;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.OrderBookAnalysis.AzureRepositories;
using Lykke.Service.OrderBookAnalysis.Contracts;
using Microsoft.Extensions.Hosting;

namespace Lykke.Service.OrderBookAnalysis.Services
{
    public sealed class SnapshotService : IHostedService
    {
        private readonly ILogFactory _lf;
        private readonly OrderBooksSourceSettings _settings;
        private readonly SnapshotRepository _snapshotRepository;
        private IDisposable _worker;
        private readonly IObservable<Unit> _workers;

        static readonly TimeSpan StatInterval = TimeSpan.FromMinutes(1);

        public SnapshotService(
            ILogFactory lf,
            OrderBooksSourceSettings settings,
            SnapshotRepository snapshotRepository)
        {
            _lf = lf;
            var log = lf.CreateLog(this);
            _settings = settings;
            _snapshotRepository = snapshotRepository;

            var orderBooks = GetOrderBooks()
                .Merge()
                .Share();

            var snapshots = GetSnapshots(orderBooks)
                .RetryWithBackoff(TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(10))
                .Share();

            var rmqPublisher = PublishToRmq(snapshots);

            var dbPublisher = PublishToAzure(snapshots);

            var statReport = ReportStat(orderBooks, log);

            _workers = Observable.Merge(
                rmqPublisher,
                dbPublisher,
                statReport);
        }

        private static IObservable<Unit> ReportStat(IObservable<OrderBook> orderBooks, ILog log)
        {
            return orderBooks
                .WindowCount(StatInterval)
                .Sample(StatInterval)
                .Do(x => log.Info($"{x} order books received from RMQ in last {StatInterval}"))
                .Select(_ => Unit.Default);
        }

        private IObservable<Unit> PublishToAzure(IObservable<IReadOnlyCollection<OrderBookSnapshot>> snapshots)
        {
            return snapshots
                .SelectMany(InsertSnapshot)
                .RetryWithBackoff(TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(10));
        }

        private IObservable<Unit> PublishToRmq(IObservable<IReadOnlyCollection<OrderBookSnapshot>> snapshots)
        {
            return snapshots
                .SelectMany(x => x)
                .PublishToRmq<OrderBookSnapshot>(_settings.ConnString, _settings.SnapshotsExchange, _lf)
                .RetryWithBackoff(TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(10));
        }

        private IObservable<IReadOnlyCollection<OrderBookSnapshot>> GetSnapshots(IObservable<OrderBook> orderBooks)
        {
            // Observable.Defer is to have DateTime.UtcNow calculated in ticker when subscription actually made,
            // not at the moment of pipeline builder works

            return Observable.Defer(() =>
            {
                var ticker = Observable.Timer(
                    Ticker.GetNextTickInterval(DateTime.UtcNow, _settings.SnapshotInterval),
                    _settings.SnapshotInterval);

                return orderBooks
                    .GroupBy(x => new SnapshotKey {Asset = x.Asset, Source = x.Source})
                    .SelectMany(g => g.Select(x => (g.Key, x)))
                    .Scan(
                        ImmutableDictionary.Create<SnapshotKey, OrderBook>(),
                        (d, o) => d.SetItem(o.Item1, o.Item2))
                    .Sample(ticker)
                    .Select(ConvertToSnapshots);
            });
        }

        private IReadOnlyCollection<OrderBookSnapshot> ConvertToSnapshots(
            IReadOnlyDictionary<SnapshotKey, OrderBook> dict)
        {
            return dict
                .Values
                .Select(ob =>
                    OrderBookConverter.FromOrderBook(
                        Ticker.Round(DateTime.UtcNow, _settings.SnapshotInterval),
                        ob))
                .ToArray();
        }

        private async Task<Unit> InsertSnapshot(IEnumerable<OrderBookSnapshot> snapshots)
        {
            await _snapshotRepository.InsertSnapshot(snapshots);

            return Unit.Default;
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

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _worker = _workers.Subscribe();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _worker?.Dispose();

            return Task.CompletedTask;
        }
    }
}
