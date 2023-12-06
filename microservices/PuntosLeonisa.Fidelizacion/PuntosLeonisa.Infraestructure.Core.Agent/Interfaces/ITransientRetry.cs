using Polly.Retry;

namespace Logistic.Infrastructure.Agents.Interfaces
{
    public interface ITransientRetry
    {
        AsyncRetryPolicy<HttpResponseMessage> GetTransientRetry();
    }
}