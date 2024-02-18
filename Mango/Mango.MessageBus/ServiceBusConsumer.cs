using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace Mango.MessageBus
{
    public class ServiceBusConsumer<T> where T : IDto
    {
        public Service Service { get; set; }

        private ServiceBusProcessor _processor;

        public ServiceBusConsumer(string serviceBusConnectionString, string topic, string subscription = null)
        {
            ServiceBusClient client = new ServiceBusClient(serviceBusConnectionString);
            _processor = client.CreateProcessor(topic, subscription);
        }

        public async Task Start()
        {
            _processor.ProcessMessageAsync += React;
            _processor.ProcessErrorAsync += ErrorHandler;
            await _processor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _processor.StopProcessingAsync();
            await _processor.DisposeAsync();
        }

        public async Task React(ProcessMessageEventArgs args)
        {
            ServiceBusReceivedMessage message = args.Message;
            string body = Encoding.UTF8.GetString(message.Body);

            T dto = JsonConvert.DeserializeObject<T>(body);
            try
            {
                await Service.Act(dto);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
