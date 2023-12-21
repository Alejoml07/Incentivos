using Polly.Retry;

namespace PuntosLeonisa.Infraestructure.Core.Agent.Interfaces
{
    public interface ITransientRetry
    {
        AsyncRetryPolicy<HttpResponseMessage> GetTransientRetry();
    }
}
