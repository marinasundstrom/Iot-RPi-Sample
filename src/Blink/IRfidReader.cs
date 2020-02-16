using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blink
{
    public interface IRfidReader
    {
        IObservable<CardData> WhenCardUidRead { get; }

        void Dispose();
        Task<CardData> ReadCardUidAsync(CancellationToken cancellationToken = default);
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
