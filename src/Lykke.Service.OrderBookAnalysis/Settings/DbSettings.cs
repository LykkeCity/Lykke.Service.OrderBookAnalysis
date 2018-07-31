using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.OrderBookAnalysis.Settings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
