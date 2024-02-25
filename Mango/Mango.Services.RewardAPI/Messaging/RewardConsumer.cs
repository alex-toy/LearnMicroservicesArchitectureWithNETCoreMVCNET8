using Azure.Messaging.ServiceBus;
using Mango.Services.RewardAPI.Models;
using Mango.Services.RewardAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.RewardAPI.Messaging
{
    public class RewardConsumer : Consumer
    {
        public RewardConsumer(string serviceBusConnectionString, string topic, string subscription, RewardService rewardService)
            : base(serviceBusConnectionString, topic, subscription, rewardService)
        {
        }

        protected override async Task ProcessMessage(ProcessMessageEventArgs args)
        {
            ServiceBusReceivedMessage message = args.Message;
            string body = Encoding.UTF8.GetString(message.Body);

            RewardsMessage rewardsMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);
            try
            {
                await _rewardService.UpdateRewards(rewardsMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
