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
            var exchangeName = "DataExportRegistered";
            _rabbit.QueueDeclare(queue: exchangeName,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);           

            _rabbit.ExchangeDeclare(exchangeName, ExchangeType.Fanout);


            _rabbit.QueueBind(exchangeName, exchangeName, string.Empty, null);

            var consumer = new AsyncEventingBasicConsumer(_rabbit);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var body = ea.Body.ToArray();
                        var message = JsonConvert.DeserializeObject<DataExportRegisteredMessage>(Encoding.UTF8.GetString(body));
                        Console.WriteLine($" [x] Received created message for record #{message.Id}");
                        var handler = scope.ServiceProvider.GetService(typeof(IDataExportRegisteredHandler)) as IDataExportRegisteredHandler;
                        await handler.Handle(message);
                        _rabbit.BasicAck(ea.DeliveryTag, false);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            };
            _rabbit.BasicConsume(queue: "DataExportRegistered",
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
