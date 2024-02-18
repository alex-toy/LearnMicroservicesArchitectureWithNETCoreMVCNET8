using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class RegisterUserConsumer : Consumer
    {
        public RegisterUserConsumer(string serviceBusConnectionString, string queue, EmailService emailService) 
            : base(serviceBusConnectionString, queue, emailService)
        {
        }

        protected override async Task ProcessMessage(ProcessMessageEventArgs args)
        {
            ServiceBusReceivedMessage message = args.Message;
            string body = Encoding.UTF8.GetString(message.Body);

            string email = JsonConvert.DeserializeObject<string>(body);
            try
            {
                await _emailService.RegisterUserEmailAndLog(email);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
