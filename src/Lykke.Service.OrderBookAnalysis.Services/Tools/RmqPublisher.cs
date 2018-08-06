using System;
using System.Reactive;
using Lykke.Common.ExchangeAdapter;
using Lykke.Common.ExchangeAdapter.Server;
using Lykke.Common.Log;

namespace Lykke.Service.OrderBookAnalysis.Services.Tools
{
    public sealed class RmqPublisher<T> : IRmqPublisher<T>
    {
        private readonly ILogFactory _lf;
        private readonly string _exchange;
        private readonly string _connectionString;

        public RmqPublisher(ILogFactory lf, string exchange, string connectionString)
        {
            _lf = lf;
            _exchange = exchange;
            _connectionString = connectionString;
        }

        public IObservable<Unit> Publish(IObservable<T> source)
        {
            return source
                .PublishToRmq(_connectionString, _exchange, _lf)
                .RetryWithBackoff(TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(10));
        }
    }
}
