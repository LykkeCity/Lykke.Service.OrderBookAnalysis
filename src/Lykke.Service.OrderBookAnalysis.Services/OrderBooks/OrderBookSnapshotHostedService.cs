using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter;
using Lykke.Service.OrderBookAnalysis.AzureRepositories;
using Lykke.Service.OrderBookAnalysis.Services.Tools;
using Microsoft.Extensions.Hosting;

namespace Lykke.Service.OrderBookAnalysis.Services.OrderBooks
{
    public sealed class OrderBookSnapshotHostedService : IHostedService
    {
        private readonly IObservable<Unit> _worker;
        private IDisposable _subscription;

        public OrderBookSnapshotHostedService(
            OrderBookListener obListener,
            Ticker ticker,
            IAzureRepository<Contracts.OrderBookSnapshot> azureRepository,
            IRmqPublisher<Contracts.OrderBookSnapshot> rmqPublisher)
        {
            var snapshots = ticker.ApplyTicker(
                    obListener.OrderBooks,
                    x => new SnapshotKey
                    {
                        Asset = x.Asset,
                        Source = x.Source
                    })
                .Select(chunk => chunk.Select(x => OrderBookSnapshotConverter.FromOrderBook(ticker.Round(DateTime.UtcNow), x)))
                .RetryWithBackoff(TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(10))
                .Share();

            _worker = Observable.Merge(
                azureRepository.Publish(snapshots),
                rmqPublisher.Publish(snapshots.SelectMany(x => x)));
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
