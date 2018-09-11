using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Service.OrderBookAnalysis.Services.Tools
{
    public static class RmqHelper
    {
        public static IObservable<T> ReadAsJson<T>(RabbitMqSubscriptionSettings settings, ILogFactory lf)
        {
            var log = lf.CreateLog(settings);

            return Observable.Create<T>(async (obs, ct) =>
            {
                var subscriber = new RabbitMqSubscriber<T>(
                        lf,
                        settings,
                        new ResilientErrorHandlingStrategy(
                            settings: settings,
                            logFactory: lf,
                            retryTimeout: TimeSpan.FromSeconds(10),
                            next: new DeadQueueErrorHandlingStrategy(lf, settings)))
                    .SetMessageDeserializer(new JsonMessageDeserializer<T>())
                    .Subscribe(x =>
                    {
                        obs.OnNext(x);
                        return Task.CompletedTask;
                    })
                    .CreateDefaultBinding();

                using (subscriber.Start())
                {
                    log.Info($"Binding created {settings.ExchangeName} -> {settings.QueueName}");
                    var cts = new TaskCompletionSource<Unit>();
                    ct.Register(() => cts.SetResult(Unit.Default));
                    await cts.Task;
                }

                obs.OnCompleted();
            });
        }
    }
}
