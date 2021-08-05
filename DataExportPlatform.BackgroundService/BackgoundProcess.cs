using DataExportPlatform.Shared;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataExportPlatform.BackgroundService
{
    public class BackgoundProcess : IHostedService
    {
        private readonly IModel _rabbit;
        private readonly IServiceProvider _serviceProvider;

        public BackgoundProcess(IModel rabbit, IServiceProvider serviceProvider)
        {
            _rabbit = rabbit;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _rabbit.QueueDeclare(queue: "DataExportRegistered",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_rabbit);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = JsonConvert.DeserializeObject<DataExportRegisteredMessage>(Encoding.UTF8.GetString(body));
                Console.WriteLine($" [x] Received created message for record #{message.Id}");
                var handler = _serviceProvider.GetService(typeof(IDataExportRegisteredHandler)) as IDataExportRegisteredHandler;
                await handler.Handle(message);
                _rabbit.BasicAck(ea.DeliveryTag, false);
            };
            _rabbit.BasicConsume(queue: "DataExportRegistered",
                                    autoAck: false,
                                    consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
