using System;
using System.Collections.Generic;
using System.Reactive;

namespace Lykke.Service.OrderBookAnalysis.AzureRepositories
{
    public interface IAzureRepository<in T>
    {
        IObservable<Unit> Publish(IObservable<IEnumerable<T>> source);
    }
}
