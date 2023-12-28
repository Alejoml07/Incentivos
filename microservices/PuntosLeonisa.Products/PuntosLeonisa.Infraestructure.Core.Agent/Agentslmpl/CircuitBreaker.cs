using Microsoft.Extensions.Configuration;
using Polly;
using Polly.CircuitBreaker;
using PuntosLeonisa.Infraestructure.Core.Agent.Interfaces;


namespace PuntosLeonisa.Infraestructure.Core.Agent.Agentslmpl
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
