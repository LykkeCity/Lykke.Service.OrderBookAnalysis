using Autofac;
using AzureStorage;
using AzureStorage.Tables;
using Lykke.Common.Log;
using Lykke.Service.OrderBookAnalysis.AzureRepositories;
using Lykke.Service.OrderBookAnalysis.Contract;
using Lykke.Service.OrderBookAnalysis.Contracts;
using Lykke.Service.OrderBookAnalysis.Services.MarketVolume;
using Lykke.Service.OrderBookAnalysis.Services.OrderBooks;
using Lykke.Service.OrderBookAnalysis.Services.Tools;
using Lykke.Service.OrderBookAnalysis.Settings;
using Lykke.SettingsReader;
using Microsoft.Extensions.Hosting;

namespace Lykke.Service.OrderBookAnalysis.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Do not register entire settings in container, pass necessary settings to services which requires them

            builder.RegisterInstance(_appSettings.CurrentValue.OrderBookAnalysisService.OrderBooksSource)
                .AsSelf();

            builder.RegisterType<OrderBookListener>()
                .SingleInstance();

            builder.RegisterType<TickPriceListener>()
                .SingleInstance();

            builder.RegisterType<OrderBookSnapshotRepository>()
                .As<IAzureRepository<OrderBookSnapshot>>()
                .SingleInstance();

            builder.RegisterType<MarketVolumeSnapshotRepository>()
                .As<IAzureRepository<MarketVolumeSnapshot>>()
                .SingleInstance();

            builder.RegisterType<Ticker>()
                .WithParameter(new NamedParameter(
                    "settingsSnapshotInterval",
                    _appSettings.CurrentValue.OrderBookAnalysisService.OrderBooksSource.SnapshotInterval))
                .SingleInstance();

            builder.RegisterType<OrderBookSnapshotHostedService>()
                .As<IHostedService>()
                .SingleInstance();

            builder.RegisterType<RmqPublisher<OrderBookSnapshot>>()
                .WithParameter(new NamedParameter(
                    "connectionString",
                    _appSettings.CurrentValue.OrderBookAnalysisService.OrderBooksSource.ConnString))
                .WithParameter(new NamedParameter(
                    "exchange",
                    _appSettings.CurrentValue.OrderBookAnalysisService.OrderBooksSource.SnapshotsExchange))
                .As<IRmqPublisher<OrderBookSnapshot>>()
                .SingleInstance();

            builder.RegisterType<RmqPublisher<MarketVolumeSnapshot>>()
                .WithParameter(new NamedParameter(
                    "connectionString",
                    _appSettings.CurrentValue.OrderBookAnalysisService.OrderBooksSource.ConnString))
                .WithParameter(new NamedParameter(
                    "exchange",
                    _appSettings.CurrentValue.OrderBookAnalysisService.OrderBooksSource.MarketVolumeSnapshotsExchange))
                .As<IRmqPublisher<MarketVolumeSnapshot>>()
                .SingleInstance();

            builder.Register(c => AzureTableStorage<OrderBookSnapshotEntity>.Create(
                    _appSettings.ConnectionString(x => x.OrderBookAnalysisService.Db.SnapshotsConnStr),
                    _appSettings.CurrentValue.OrderBookAnalysisService.Db.SnapshotsTableName,
                    c.Resolve<ILogFactory>()))
                .As<INoSQLTableStorage<OrderBookSnapshotEntity>>();

            builder.Register(c => AzureTableStorage<MarketVolumeSnapshotEntity>.Create(
                    _appSettings.ConnectionString(x => x.OrderBookAnalysisService.Db.SnapshotsConnStr),
                    _appSettings.CurrentValue.OrderBookAnalysisService.Db.MarketVolumeSnapshotsTable,
                    c.Resolve<ILogFactory>()))
                .As<INoSQLTableStorage<MarketVolumeSnapshotEntity>>();

            builder.RegisterType<MarketVolumeHostedService>()
                .As<IHostedService>()
                .SingleInstance();
        }
    }
}
