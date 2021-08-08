using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace DataExportPlatform.Shared
{
    public interface IMessageBus
    {
        void SendRegistered(int id);
        void SendUpdated(int id);
    }

    public class MessageBus : IMessageBus
    {
        private readonly IModel _rabbit;

        public MessageBus(IModel rabbit)
        {
            _rabbit = rabbit;
        }

        public void SendRegistered(int id)
        {
            var message = new DataExportRegisteredMessage
            {
                Id = id
            };

            var serializedMessage = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _rabbit.BasicPublish(exchange: "DataExportRegistered",
                                 routingKey: string.Empty,
                                 basicProperties: null,
                                 body: serializedMessage);
        }

        public void SendUpdated(int id)
        {
            var message = new DataExportUpdatedMessage
            {
                Id = id
            };

            var serializedMessage = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _rabbit.BasicPublish(exchange: "DataExportUpdated",
                                 routingKey: string.Empty,
                                 basicProperties: null,
                                 body: serializedMessage);
        }
    }
}