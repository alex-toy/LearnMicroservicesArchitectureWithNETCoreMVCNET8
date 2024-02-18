using Mango.MessageBus;

namespace Mango.Services.EmailAPI.Models.Dto
{ 
    public class CartDto : IDto
    {
        public CartHeaderDto CartHeader { get; set; }
        public IEnumerable<CartDetailsDto>? CartDetails { get; set; }
    }
}
