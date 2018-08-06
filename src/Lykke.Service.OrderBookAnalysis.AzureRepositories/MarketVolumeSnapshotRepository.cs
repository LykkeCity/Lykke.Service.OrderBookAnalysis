using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Common.Log;
using Lykke.Common.ExchangeAdapter;
using Lykke.Common.Log;
using Lykke.Service.OrderBookAnalysis.Contract;

namespace Lykke.Service.OrderBookAnalysis.AzureRepositories
{
    public sealed class MarketVolumeSnapshotRepository : IAzureRepository<MarketVolumeSnapshot>
    {
        private readonly INoSQLTableStorage<MarketVolumeSnapshotEntity> _storage;

        public MarketVolumeSnapshotRepository(
            INoSQLTableStorage<MarketVolumeSnapshotEntity> storage,
            ILogFactory lf)
        {
            _storage = storage;
            _log = lf.CreateLog(this);
        }

        static MarketVolumeSnapshotRepository()
        {
            var config = new MapperConfiguration(
                cfg => cfg.CreateMap<MarketVolumeSnapshot, MarketVolumeSnapshotEntity>()

                .ForMember(
                    x => x.PartitionKey,
                    m => m.MapFrom(ss => MarketVolumeSnapshotEntity.BySnapshot.GeneratePartitionKey(ss.Timestamp)))

                .ForMember(x => x.RowKey,
                    m => m.MapFrom(ss =>
                        MarketVolumeSnapshotEntity.BySnapshot.GenerateRowKey(ss.Exchange, ss.AssetPair))));

            Mapper = config.CreateMapper();
        }

        private static readonly IMapper Mapper;
        private ILog _log;

        public IObservable<Unit> Publish(IObservable<IEnumerable<MarketVolumeSnapshot>> source)
        {
            return source.SelectMany(async x =>
                {
                    await InsertSnapshot(x);
                    return Unit.Default;
                })
                .RetryWithBackoff(TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(10));
        }

        public async Task InsertSnapshot(IEnumerable<MarketVolumeSnapshot> snapshots)
        {
            var entities = snapshots.Select(x => Mapper.Map<MarketVolumeSnapshot, MarketVolumeSnapshotEntity>(x))
                .ToArray();

            if (entities.Any())
            {
                await _storage.InsertAsync(entities);
                _log.Info($"{entities.Length} market volume snapshots have been written to storage " +
                          $"for timestamp: {entities[0].PartitionKey}");
            }
        }
    }
}
