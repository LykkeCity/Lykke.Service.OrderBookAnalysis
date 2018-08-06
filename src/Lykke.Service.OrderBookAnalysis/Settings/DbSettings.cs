using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.OrderBookAnalysis.Settings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }

        [AzureTableCheck]
        public string SnapshotsConnStr { get; set; }

        public string SnapshotsTableName { get; set; }
        public string MarketVolumeSnapshotsTable { get; set; }
    }
}
