using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.OrderBookAnalysis.Client 
{
    /// <summary>
    /// OrderBookAnalysis client settings.
    /// </summary>
    public class OrderBookAnalysisServiceClientSettings 
    {
        /// <summary>Service url.</summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl {get; set;}
    }
}
