using DataExportPlatform.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataExportPlatform.PushNotifications
{
    public class BackgoundMessageListener : IHostedService
    {
        private readonly IModel _rabbit;
        private readonly IServiceProvider _serviceProvider;

        public BackgoundMessageListener(IModel rabbit, IServiceProvider serviceProvider)
        {
            _rabbit = rabbit;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _rabbit.QueueDeclare(queue: "DataExportUpdated",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_rabbit);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var body = ea.Body.ToArray();
                        var message = JsonConvert.DeserializeObject<DataExportUpdatedMessage>(Encoding.UTF8.GetString(body));
                        var handler = scope.ServiceProvider.GetService(typeof(IDataExportUpdatedHandler)) as IDataExportUpdatedHandler;
                        await handler.Handle(message);
                        _rabbit.BasicAck(ea.DeliveryTag, false);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            };
            _rabbit.BasicConsume(queue: "DataExportUpdated",
                                    autoAck: false,
                                    consumer: consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
