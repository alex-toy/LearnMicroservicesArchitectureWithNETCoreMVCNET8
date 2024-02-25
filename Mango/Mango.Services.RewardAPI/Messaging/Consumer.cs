using Azure.Messaging.ServiceBus;
using Mango.Services.RewardAPI.Services;

namespace Mango.Services.RewardAPI.Messaging
{
    public abstract class Consumer
    {
        protected readonly RewardService _rewardService;

        protected ServiceBusProcessor _serviceProcessor;

        protected Consumer(string serviceBusConnectionString, string topic, string subscription, RewardService rewardService)
        {
            _rewardService = rewardService;
            ServiceBusClient client = new ServiceBusClient(serviceBusConnectionString);
            _serviceProcessor = client.CreateProcessor(topic, subscription);
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
