using Autofac;
using AzureStorage;
using AzureStorage.Tables;
using Lykke.Common.Log;
using Lykke.Service.OrderBookAnalysis.AzureRepositories;
using Lykke.Service.OrderBookAnalysis.Services;
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

            builder.RegisterType<SnapshotService>()
                .AsSelf()
                .As<IHostedService>()
                .WithParameter(
                    new TypedParameter(typeof(OrderBooksSourceSettings),
                        _appSettings.CurrentValue.OrderBookAnalysisService.OrderBooksSource))
                .SingleInstance();

            builder.Register(c => AzureTableStorage<OrderBookSnapshotEntity>.Create(
                    _appSettings.ConnectionString(x => x.OrderBookAnalysisService.Db.SnapshotsConnStr),
                    _appSettings.CurrentValue.OrderBookAnalysisService.Db.SnapshotsTableName,
                    c.Resolve<ILogFactory>()))
                .As<INoSQLTableStorage<OrderBookSnapshotEntity>>();

            builder.RegisterType<SnapshotRepository>()
                .As<SnapshotRepository>()
                .SingleInstance();
        }
    }
}
