namespace Mango.MessageBus
{
    public interface IService
    {
        Task Act(IDto dto);
    }
}
