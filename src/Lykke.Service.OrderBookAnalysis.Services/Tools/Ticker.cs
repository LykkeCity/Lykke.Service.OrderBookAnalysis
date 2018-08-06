using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Linq;

namespace Lykke.Service.OrderBookAnalysis.Services.Tools
{
    public sealed class Ticker
    {
        private readonly TimeSpan _settingsSnapshotInterval;

        public Ticker(TimeSpan settingsSnapshotInterval)
        {
            _settingsSnapshotInterval = settingsSnapshotInterval;
        }

        public IObservable<IEnumerable<T>> ApplyTicker<T>(IObservable<T> source, Func<T, SnapshotKey> getSnapshotKey)
        {
            // Observable.Defer is to have DateTime.UtcNow calculated in ticker when a subscription actually made,
            // not at the moment when pipeline builder works

            return Observable.Defer(() =>
                {
                    var ticker = Observable.Timer(
                        GetNextTickInterval(DateTime.UtcNow),
                        _settingsSnapshotInterval);

                    return source
                        .GroupBy(getSnapshotKey)
                        .SelectMany(g => g.Select(x => (g.Key, x)))
                        .Scan(
                            ImmutableDictionary.Create<SnapshotKey, T>(),
                            (d, o) => d.SetItem(o.Item1, o.Item2))
                        .Sample(ticker);
                })
                .Select(x => x.Values);
        }

        public DateTime Round(DateTime dateTime)
        {
            var fullPeriods = (int) Math.Round(dateTime.TimeOfDay / _settingsSnapshotInterval);
            return dateTime.Date + fullPeriods * _settingsSnapshotInterval;
        }

        public TimeSpan GetNextTickInterval(DateTime now)
        {
            var fullPeriods = (int) Math.Floor (now.TimeOfDay / _settingsSnapshotInterval);
            return _settingsSnapshotInterval * (fullPeriods + 1) - now.TimeOfDay;
        }
    }
}
