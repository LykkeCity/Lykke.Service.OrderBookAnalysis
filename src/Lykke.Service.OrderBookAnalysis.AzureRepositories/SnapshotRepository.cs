using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.OrderBookAnalysis.Contracts;

namespace Lykke.Service.OrderBookAnalysis.AzureRepositories
{
    public sealed class SnapshotRepository
    {
        static SnapshotRepository()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<OrderBookSnapshot, OrderBookSnapshotEntity>()

                .ForMember(
                    x => x.PartitionKey,
                    m => m.MapFrom(ss => OrderBookSnapshotEntity.ByOrderBook.GeneratePartitionKey(ss.Timestamp)))

                .ForMember(x => x.RowKey,
                    m => m.MapFrom(ss =>
                        OrderBookSnapshotEntity.ByOrderBook.GenerateRowKey(ss.Exchange, ss.AssetPair))));

            Mapper = config.CreateMapper();
        }

        private readonly INoSQLTableStorage<OrderBookSnapshotEntity> _storage;
        private readonly ILog _log;
        private static readonly IMapper Mapper;

        public SnapshotRepository(
            INoSQLTableStorage<OrderBookSnapshotEntity> storage,
            ILogFactory logFactory)
        {
            _storage = storage;
            _log = logFactory.CreateLog(this);
        }

        public async Task InsertSnapshot(IEnumerable<OrderBookSnapshot> snapshots)
        {
            var orderBookSnapshots = snapshots.Select(x => Mapper.Map<OrderBookSnapshot, OrderBookSnapshotEntity>(x))
                .ToArray();

            if (orderBookSnapshots.Any())
            {
                await _storage.InsertAsync(orderBookSnapshots);
                _log.Info($"{orderBookSnapshots.Length} order book snapshots have been written to storage " +
                          $"for timestamp: {orderBookSnapshots[0].PartitionKey}");
            }
        }
    }
}
