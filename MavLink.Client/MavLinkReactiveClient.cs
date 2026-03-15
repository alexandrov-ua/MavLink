using System.Collections.Concurrent;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using MavLink.Serialize.Dialects.Common;
using MavLink.Serialize.Messages;

namespace MavLink.Client;

public interface IMavLinkReactiveClient : IObservable<IPocket<IPayload>>, IDisposable
{
    Task<TResponse> SendWithResponse<TRequest, TResponse>(TRequest request,
        Func<TRequest, TResponse, bool> predicate)
        where TRequest : IPocket<IPayload>
        where TResponse : IPocket<IPayload>;
}

public class MavLinkReactiveClient : IMavLinkReactiveClient
{
    private readonly IMavLinkClient _mavLinkClient;
    private readonly IObservable<IPocket<IPayload>> _receivedEvents;
    private readonly ConcurrentQueue<IPocket<IPayload>> _pocketsToSend = new ConcurrentQueue<IPocket<IPayload>>();
    private readonly IScheduler _scheduler;
    private readonly CompositeDisposable _disposable = new CompositeDisposable();

    public MavLinkReactiveClient(IMavLinkClient mavLinkClient)
    {
        _scheduler = new EventLoopScheduler();
        _mavLinkClient = mavLinkClient;
        var messages = ReceiveMessages()
            .Retry()
            .SubscribeOn(_scheduler)
            .Publish();
        _receivedEvents = messages;
        _disposable.Add(messages.Connect());
    }

    private IObservable<IPocket<IPayload>> ReceiveMessages()
    {
        return Observable.Create<IPocket<IPayload>>(observer =>
        {
            var cts = new CancellationTokenSource();

            var task = Task.Run(() =>
            {
                try
                {
                    while (!cts.Token.IsCancellationRequested)
                    {
                        var message = _mavLinkClient.Receive();
                        if (message is null)
                            continue;
                        observer.OnNext(message);
                    }
                }
                catch (OperationCanceledException)
                {
                    // normal shutdown
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR!!!, {ex}");
                    observer.OnError(ex);
                }
            }, cts.Token);

            return Disposable.Create(() =>
            {
                cts.Cancel();
                cts.Dispose();
            });
        });
    }

    public async Task<TResponse> SendWithResponse<TRequest, TResponse>(TRequest request,
        Func<TRequest, TResponse, bool> predicate)
        where TRequest : IPocket<IPayload>
        where TResponse : IPocket<IPayload>
    {
        var response = this.OfType<TResponse>()
            .Where(t => predicate(request, t))
            .FirstAsync()
            .ToTask();
        await _mavLinkClient.Send(request);
        return await response;
    }

    public async Task Send<TRequest>(TRequest request)
        where TRequest : IPocket<IPayload>
    {
        await _mavLinkClient.Send(request);
    }





    public IDisposable Subscribe(IObserver<IPocket<IPayload>> observer)
    {
        return _receivedEvents.Subscribe(observer);
    }

    public void Dispose()
    {
        _disposable.Dispose();
        _mavLinkClient.Dispose();
    }
}