using JetBrains.Annotations;

namespace Lykke.Service.OrderBookAnalysis.Client
{
    /// <summary>
    /// OrderBookAnalysis client interface.
    /// </summary>
    [PublicAPI]
    public interface IOrderBookAnalysisClient
    {
        /// <summary>Application Api interface</summary>
        IOrderBookAnalysisApi Api { get; }
    }
}
