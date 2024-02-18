using Mango.Services.EmailAPI.Services;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private EmailCartConsumer _emailCartConsumer;
        private EmailOrderPlacedConsumer _emailOrderPlacedConsumer;
        private RegisterUserConsumer _registerUserConsumer;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            string emailCartQueue = configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            string orderCreated_Topic = configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            string orderCreated_Email_Subscription = configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Email_Subscription");
            string registerUserQueue = configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");

            string serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");

            _emailCartConsumer = new EmailCartConsumer(serviceBusConnectionString, emailCartQueue, emailService);
            _emailOrderPlacedConsumer = new EmailOrderPlacedConsumer(serviceBusConnectionString, orderCreated_Topic, orderCreated_Email_Subscription, emailService);
            _registerUserConsumer = new RegisterUserConsumer(serviceBusConnectionString, registerUserQueue, emailService);
        }

        public async Task Start()
        {
            await _emailCartConsumer.Start();
            await _registerUserConsumer.Start();
            await _emailOrderPlacedConsumer.Start();
        }

        public async Task Stop()
        {
            await _emailCartConsumer.Stop();
            await _registerUserConsumer.Stop();
            await _emailOrderPlacedConsumer.Stop();
        }
    }
}
