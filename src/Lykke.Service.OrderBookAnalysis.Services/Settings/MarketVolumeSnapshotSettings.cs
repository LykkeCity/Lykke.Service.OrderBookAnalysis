namespace Lykke.Service.OrderBookAnalysis.Services.Settings
{
    public sealed class MarketVolumeSnapshotSettings
    {
        /// <summary>
        /// asset for monitoring, we check all pair where BaseAsset == Asset
        /// </summary>
        public string Asset { get; set; }

        /// <summary>
        /// asset pair for convert Asset to USD
        /// </summary>
        public string CrossAssetPair { get; set; }

        /// <summary>
        /// IsRevert flag in asset pair for convert Asset to USD
        /// </summary>
        public bool CrossRevert { get; set; }

        /// <summary>
        /// Source for filtering quote by CrossAssetPair from CrossRateExchange
        /// </summary>
        public string CrossRateSource { get; set; }

        /// <summary>
        /// Exchange for receive quote by CrossAssetPair
        /// </summary>
        public string CrossRateExchange { get; set; }

        public int Decimals { get; set; }
    }
}
