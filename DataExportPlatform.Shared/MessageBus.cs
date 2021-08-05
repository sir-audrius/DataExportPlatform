using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace DataExportPlatform.Shared
{
    public interface IMessageBus
    {
        void SendRegistered(int id);
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
            _rabbit.BasicPublish(exchange: "",
                                 routingKey: "DataExportRegistered",
                                 basicProperties: null,
                                 body: serializedMessage);
        }
    }
}