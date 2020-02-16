using System;
using System.Collections.Generic;
using System.Device.Spi;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Iot.Device.Mfrc522;
using Microsoft.Extensions.Logging;

namespace Blink
{

    public class RfidReader : IRfidReader
    {
        private CancellationTokenSource? cancellationTokenSource;
        private Task? _thread;
        private readonly Mfrc522Controller _rfidController;
        private readonly Subject<CardData> _whenCardDetected;
        private readonly ILogger<RfidReader> _logger;

        public RfidReader(ILogger<RfidReader> logger, SpiDevice spiDevice)
        {
            _rfidController = new Mfrc522Controller(spiDevice);

            _whenCardDetected = new Subject<CardData>();
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("RFID Service is starting.");

            cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource!.Token, cancellationToken);

            _thread = Task.Run(DoWork, cancellationTokenSource!.Token);

            return Task.CompletedTask;
        }

        private async Task DoWork()
        {
            _logger.LogInformation("RFID Reader Service started.");

            while (true)
            {
                if (cancellationTokenSource!.IsCancellationRequested)
                {
                    break;
                }

                var (status, _) = _rfidController.Request(RequestMode.RequestIdle);

                if (status != Status.OK)
                {
                    continue;
                }

                var (status2, uid) = _rfidController.AntiCollision();

                if (status2 != Status.OK)
                {
                    continue;
                }

                _logger.LogDebug($"Card UID read: {string.Join(", ", uid)}");

                _whenCardDetected.OnNext(new CardData(uid));

                await Task.Delay(100);
            }

            _logger.LogInformation("RFID Reader Service stopped.");
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("RFID Reader Service is stopping.");

            _thread = null;
            cancellationTokenSource!.Cancel();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _thread = null;
            cancellationTokenSource!.Cancel();
        }

        public Task<CardData> ReadCardUidAsync(CancellationToken cancellationToken = default)
        {
            var taskCompletionSource = new TaskCompletionSource<CardData>();

            Task.Run(async () =>
            {   
                while (true)
                {
                    var (status, _) = _rfidController.Request(RequestMode.RequestIdle);

                    if (status != Status.OK)
                    {
                        continue;
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                    
                    var (status2, uid) = _rfidController.AntiCollision();

                    if(status2 == Status.OK) 
                    {
                        taskCompletionSource.SetResult(new CardData(uid));
                        return;
                    }

                    await Task.Delay(200);
                }
            }, cancellationToken);

            return taskCompletionSource.Task;
        }

        public IObservable<CardData> WhenCardUidRead => _whenCardDetected;
    }
}
