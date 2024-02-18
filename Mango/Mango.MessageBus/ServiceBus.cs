using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace Mango.MessageBus
{
    public class ServiceBus : IServiceBus
    {
        private string connectionString = "Endpoint=sb://mangoalexei.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=rdhjpH+0zsuKpCcs91XylSQ4Mg1199Obo+ASbHfQJ+0=";

        public async Task PublishMessage(object message, string topic_queue_Name)
        {
            await using var client = new ServiceBusClient(connectionString);

            ServiceBusSender sender = client.CreateSender(topic_queue_Name);

            string jsonMessage = JsonConvert.SerializeObject(message);
            byte[] body = Encoding.UTF8.GetBytes(jsonMessage);
            ServiceBusMessage finalMessage = new ServiceBusMessage(body)
            {
                CorrelationId = Guid.NewGuid().ToString(),
            };

            await sender.SendMessageAsync(finalMessage);
            await client.DisposeAsync();
        }
    }
}
