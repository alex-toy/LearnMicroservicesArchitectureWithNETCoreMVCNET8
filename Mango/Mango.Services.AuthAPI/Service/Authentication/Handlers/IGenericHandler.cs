using Mango.Services.AuthAPI.Models;

namespace Mango.Services.AuthAPI.Service.Authentication.AuthHelpers.Handlers
{
    public interface IGenericHandler<Request, Response> where Request : class where Response : class, new()
    {
        IGenericHandler<Request, Response> SetNext(IGenericHandler<Request, Response> handler);
        Task<Response> HandleAsync(Request request, ApplicationUser user);
    }
}
