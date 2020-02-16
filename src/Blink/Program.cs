using System;
using System.Device.Gpio;
using System.Device.Spi;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Reactive.Linq;

namespace Blink
{
    class Program
    {
        private static CancellationTokenSource cts = new CancellationTokenSource();

        static async Task Main(string[] args)
        {
            Console.CancelKeyPress += (s, a) =>
            {
                cts.Cancel();
            };

            //RfidReader();

            await Blink();
        }

        private static void RfidReader()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IRfidReader>(sp =>
            {
                var connection = new SpiConnectionSettings(0, 0);
                connection.ClockFrequency = 500000;
                var spi = SpiDevice.Create(connection);
                var logger = sp.GetService<ILogger<RfidReader>>();
                return new RfidReader(logger, spi);
            });

            services.AddLogging(c => c.AddConsole());

            var serviceProvider = services.BuildServiceProvider();

            var rfidReader = serviceProvider.GetService<IRfidReader>();

            //await ReadLoop(rfidReader, cts);

            //await ReadObservable(rfidReader);
        }

        private static async Task ReadObservable(IRfidReader rfidReader)
        {
            await rfidReader.StartAsync();

            var controller = new GpioController();
            controller.OpenPin(5, PinMode.Output);

            var subscription = rfidReader.WhenCardUidRead.Subscribe(async data =>
            {
                controller.Write(5, PinValue.High);
                Console.WriteLine($"UID: {string.Join(',', data.UID)}");
                await Task.Delay(1000);
                controller.Write(5, PinValue.Low);
            });

            await Task.Delay(TimeSpan.FromMilliseconds(-1), cts.Token);

            await rfidReader.StopAsync();

            subscription.Dispose();
        }

        private static async Task ReadLoop(IRfidReader rfidReader)
        {
            var cancellationToken = cts!.Token;

            while (!cancellationToken.IsCancellationRequested)
            {
                var data = await rfidReader.ReadCardUidAsync();
                Console.WriteLine($"UID: {string.Join(',', data.UID)}");
                await Task.Delay(200);
            }
        }

        private static async Task Blink()
        {
            GpioController controller = new GpioController();
            controller.OpenPin(5, PinMode.Output);
            controller.OpenPin(19, PinMode.Output);
            controller.OpenPin(6, PinMode.Output);

            while (!cts.Token.IsCancellationRequested)
            {
                controller.Write(5, PinValue.High);
                await Task.Delay(200);
                controller.Write(5, PinValue.Low);
                await Task.Delay(200);
                controller.Write(19, PinValue.High);
                await Task.Delay(200);
                controller.Write(19, PinValue.Low);
                await Task.Delay(200);
                controller.Write(6, PinValue.High);
                await Task.Delay(200);
                controller.Write(6, PinValue.Low);
                await Task.Delay(200);
            }

            controller.Write(5, PinValue.Low);
            controller.Write(19, PinValue.Low);
            controller.Write(6, PinValue.Low);

            controller.ClosePin(5);
            controller.ClosePin(19);
            controller.ClosePin(6);
        }
    }
}
