using Lykke.HttpClientGenerator;

namespace Lykke.Service.OrderBookAnalysis.Client
{
    /// <summary>
    /// OrderBookAnalysis API aggregating interface.
    /// </summary>
    public class OrderBookAnalysisClient : IOrderBookAnalysisClient
    {
        // Note: Add similar Api properties for each new service controller

        /// <summary>Inerface to OrderBookAnalysis Api.</summary>
        public IOrderBookAnalysisApi Api { get; private set; }

        /// <summary>C-tor</summary>
        public OrderBookAnalysisClient(IHttpClientGenerator httpClientGenerator)
        {
            Api = httpClientGenerator.Generate<IOrderBookAnalysisApi>();
        }
    }
}
