using Logistic.Infrastructure.Agents.Interfaces;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.CircuitBreaker;

namespace PuntosLeonisa.Infraestructure.Core.Agent.AgentsImpl
{
        public class CircuitBreaker : ICircuitBreaker
        {
            private readonly IConfiguration _configuration;

            public CircuitBreaker(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public AsyncCircuitBreakerPolicy<HttpResponseMessage> GetCircuitBreaker()
            {
                int errorNumber = Convert.ToInt32(_configuration["ErrorNumber"]);
                int circuitTime = Convert.ToInt32(_configuration["CircuitTime"]);

                AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreakerPolicy =
                Policy.HandleResult<HttpResponseMessage>(message => (int)message.StatusCode == 429 || (int)message.StatusCode >= 500)
                .CircuitBreakerAsync(errorNumber, TimeSpan.FromSeconds(circuitTime));

                return CircuitBreakerPolicy;
            }
        }
}
