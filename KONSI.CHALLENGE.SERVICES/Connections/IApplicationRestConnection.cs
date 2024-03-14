using RestSharp;

namespace KONSI.CHALLENGE.SERVICES.Connections
{
    public interface IApplicationRestConnection
    {
        Task<string> Connection(Method method, string urlAddOn, string? content);
    }
}