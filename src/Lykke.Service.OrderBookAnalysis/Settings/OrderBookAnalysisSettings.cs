using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.OrderBookAnalysis.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class OrderBookAnalysisSettings
    {
        public DbSettings Db { get; set; }
    }
}
