using Mango.Services.RewardAPI.Services;

namespace Mango.Services.RewardAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private RewardConsumer _rewardConsumer;

        public AzureServiceBusConsumer(IConfiguration configuration, RewardService rewardService)
        {
            string orderCreatedTopic = configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            string orderCreatedRewardSubscription = configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Rewards_Subscription");
            string serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");

            _rewardConsumer = new RewardConsumer(serviceBusConnectionString, orderCreatedTopic, orderCreatedRewardSubscription, rewardService);
        }

        public async Task Start()
        {
            await _rewardConsumer.Start();
        }

        public async Task Stop()
        {
            await _rewardConsumer.Stop();
        }
    }
}
