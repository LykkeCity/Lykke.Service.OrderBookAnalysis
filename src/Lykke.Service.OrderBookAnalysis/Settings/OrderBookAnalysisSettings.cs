using JetBrains.Annotations;
using Lykke.Service.OrderBookAnalysis.Services;

namespace Lykke.Service.OrderBookAnalysis.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class OrderBookAnalysisSettings
    {
        public DbSettings Db { get; set; }
        public OrderBooksSourceSettings OrderBooksSource { get; set; }
    }
}
