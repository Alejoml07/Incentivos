using Logistic.Infrastructure.Agents.Interfaces;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logistic.Infrastructure.Agents.AgentsImpl
{
    public class TransientRetry : ITransientRetry
    {
        private readonly IConfiguration _configuration;
        public TransientRetry(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public AsyncRetryPolicy<HttpResponseMessage> GetTransientRetry()
        {
            int retries = Convert.ToInt32(_configuration["RetryNumber"]);
            Random Jitterer = new Random();
            AsyncRetryPolicy<HttpResponseMessage> TransientErrorRetryPolicy =
                Policy.HandleResult<HttpResponseMessage>(
                    message => (int)message.StatusCode == 429 || (int)message.StatusCode >= 500)
                .WaitAndRetryAsync(2, sleepDurationProvider: retryAttemp =>
                {
                    Console.WriteLine($"Reintentando {retryAttemp}");
                    return TimeSpan.FromSeconds(Math.Pow(retries, retryAttemp)) + TimeSpan.FromMilliseconds(Jitterer.Next(0, 1000));
                });
            return TransientErrorRetryPolicy;
        }
    }
}
