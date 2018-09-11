using System;
using System.Reactive;

namespace Lykke.Service.OrderBookAnalysis.Services.Tools
{
    public interface IRmqPublisher<T>
    {
        IObservable<Unit> Publish(IObservable<T> source);
    }
}