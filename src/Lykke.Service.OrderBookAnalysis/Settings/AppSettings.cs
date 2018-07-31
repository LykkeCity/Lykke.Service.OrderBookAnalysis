using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.Service.OrderBookAnalysis.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public OrderBookAnalysisSettings OrderBookAnalysisService { get; set; }
    }
}
