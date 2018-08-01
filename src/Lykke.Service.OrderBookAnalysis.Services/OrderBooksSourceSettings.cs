using System;
using System.Collections.Generic;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.OrderBookAnalysis.Services
{
    public sealed class OrderBooksSourceSettings
    {
        [AmqpCheck]
        public string ConnString { get; set; }

        public IReadOnlyCollection<string> Exchanges { get; set; }

        public TimeSpan SnapshotInterval { get; set; }
        public string SnapshotsExchange { get; set; }
    }
}
