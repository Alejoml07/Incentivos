using Polly.CircuitBreaker;

namespace Logistic.Infrastructure.Agents.Interfaces
{
    public interface ICircuitBreaker
    {
        AsyncCircuitBreakerPolicy<HttpResponseMessage> GetCircuitBreaker();
    }
}