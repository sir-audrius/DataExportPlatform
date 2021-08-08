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
            CreateRegisteredListener();
            CreateUpdatedListener();            

            return Task.CompletedTask;
        }

        private void CreateRegisteredListener()
        {
            var exchangeName = "DataExportRegistered";
            var consumerId = Guid.NewGuid();
            var queueName = $"{exchangeName}-{consumerId}";

            _rabbit.ExchangeDeclare(exchangeName, ExchangeType.Fanout);

            _rabbit.QueueDeclare(queue: queueName,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: true,
                                    arguments: null);

            _rabbit.QueueBind(queueName, exchangeName, string.Empty, null);

            var registeredConsumer = new AsyncEventingBasicConsumer(_rabbit);
            registeredConsumer.Received += async (model, ea) =>
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var body = ea.Body.ToArray();
                        var message = JsonConvert.DeserializeObject<DataExportRegisteredMessage>(Encoding.UTF8.GetString(body));
                        var handler = scope.ServiceProvider.GetService(typeof(IDataExportRegisteredHandler)) as IDataExportRegisteredHandler;
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
            _rabbit.BasicConsume(queue: queueName,
                                    autoAck: false,
                                    consumer: registeredConsumer);
        }

        private void CreateUpdatedListener()
        {
            var exchangeName = "DataExportUpdated";
            var consumerId = Guid.NewGuid();
            var queueName = $"{exchangeName}-{consumerId}";

            _rabbit.ExchangeDeclare(exchangeName, ExchangeType.Fanout);

            _rabbit.QueueDeclare(queue: queueName,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: true,
                                    arguments: null);

            _rabbit.QueueBind(queueName, exchangeName, string.Empty, null);

            var updateConsumer = new AsyncEventingBasicConsumer(_rabbit);
            updateConsumer.Received += async (model, ea) =>
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
            _rabbit.BasicConsume(queue: queueName,
                                    autoAck: false,
                                    consumer: updateConsumer);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
