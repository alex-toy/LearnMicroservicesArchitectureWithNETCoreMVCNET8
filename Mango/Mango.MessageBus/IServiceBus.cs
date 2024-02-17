namespace Mango.MessageBus
{
    public interface IServiceBus
    {
        Task PublishMessage(object message, string topic_queue_Name);
    }
}
