using Polly.CircuitBreaker;

namespace PuntosLeonisa.Infraestructure.Core.Agent.Interfaces
{
    public interface ICircuitBreaker
    {
        AsyncCircuitBreakerPolicy<HttpResponseMessage> GetCircuitBreaker();
    }
}
