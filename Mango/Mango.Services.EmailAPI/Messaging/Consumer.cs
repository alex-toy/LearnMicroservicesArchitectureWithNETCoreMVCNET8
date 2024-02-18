using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Services;

namespace Mango.Services.EmailAPI.Messaging
{
    public abstract class Consumer
    {
        protected readonly EmailService _emailService;

        protected ServiceBusProcessor _serviceProcessor;

        public Consumer(string serviceBusConnectionString, string queue, EmailService emailService)
        {
            _emailService = emailService;
            ServiceBusClient client = new ServiceBusClient(serviceBusConnectionString);
            _serviceProcessor = client.CreateProcessor(queue);
        }

        public Consumer(string serviceBusConnectionString, string topic, string subscription, EmailService emailService)
        {
            ServiceBusClient client = new ServiceBusClient(serviceBusConnectionString);
            _serviceProcessor = client.CreateProcessor(topic, subscription);
            _emailService = emailService;
        }

        public async Task Start()
        {
            _serviceProcessor.ProcessMessageAsync += ProcessMessage;
            _serviceProcessor.ProcessErrorAsync += ErrorHandler;
            await _serviceProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _serviceProcessor.StopProcessingAsync();
            await _serviceProcessor.DisposeAsync();
        }

        protected abstract Task ProcessMessage(ProcessMessageEventArgs args);

        protected Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
