namespace Mango.MessageBus
{
    public class Service
    {
        public delegate Task Action(IDto dto);
        public Action Act_ { get; set; }

        public Task Act(IDto dto)
        {
            return Act_(dto);
        }
    }
}
