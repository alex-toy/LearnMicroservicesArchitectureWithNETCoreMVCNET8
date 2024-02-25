using Mango.Services.AuthAPI.Models;

namespace Mango.Services.AuthAPI.Service.Authentication.AuthHelpers.Handlers
{
    public abstract class GenericHandler<Request, Response> : IGenericHandler<Request, Response> where Request : class where Response : class, new()
    {
        protected IGenericHandler<Request, Response>? _successor;

        public abstract Task<Response> HandleAsync(Request request, ApplicationUser user);

        public IGenericHandler<Request, Response> SetNext(IGenericHandler<Request, Response> handler)
        {
            _successor = handler;
            return _successor;
        }

        protected async Task<Response> Proceed(Request request, ApplicationUser user)
        {
            if (_successor is not null)
            {
                return await _successor.HandleAsync(request, user);
            }

            return new Response();
        }
    }
}
