using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PuntosLeonisa.Infraestructure.Core.Agent.Interfaces;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace PuntosLeonisa.Infraestructure.Core.Agent.Agentslmpl
{
    public class HttpClientAgents : IHttpClientAgent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITransientRetry _transientRetry;
        private readonly ICircuitBreaker _circuitBreaker;
        private readonly IConfiguration _configuration;

        public HttpClientAgents(IHttpClientFactory httpClientFactory, ITransientRetry transientRetry, ICircuitBreaker circuitBreaker, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _transientRetry = transientRetry;
            _circuitBreaker = circuitBreaker;
            _configuration = configuration;
        }

        public async Task<T1> GetRequest<T1>(Uri requestUrl)
        {
            try
            {
                var CircuitBreakerPolicy = _circuitBreaker.GetCircuitBreaker();
                var TransientErrorRetryPolicy = _transientRetry.GetTransientRetry();
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                var resultData = await CircuitBreakerPolicy.ExecuteAsync(() =>
                        TransientErrorRetryPolicy.ExecuteAsync(() =>
                        httpClient.GetAsync(requestUrl))
                    );
                var response = await resultData.Content.ReadAsStringAsync();
#pragma warning disable CS8603 // Posible tipo de valor devuelto de referencia nulo
                return JsonConvert.DeserializeObject<T1>(response, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                    Formatting = Formatting.Indented,
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                });
#pragma warning restore CS8603 // Posible tipo de valor devuelto de referencia nulo
            }
            catch (Exception ex)
            {
                Exception exception = new("Failed" + ex.InnerException + "\n" + ex.Message);
                throw exception;
            }
        }



        public async Task<T1> GetRequestXml<T1>(Uri requestUrl)
        {
            try
            {
                var CircuitBreakerPolicy = _circuitBreaker.GetCircuitBreaker();
                var TransientErrorRetryPolicy = _transientRetry.GetTransientRetry();
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                var resultData = await CircuitBreakerPolicy.ExecuteAsync(() =>
                        TransientErrorRetryPolicy.ExecuteAsync(() =>
                        httpClient.GetAsync(requestUrl))
                    );
                var response = await resultData.Content.ReadAsStringAsync();
                XmlSerializer serializer = new XmlSerializer(typeof(T1));
                using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(response)))
                {
                    T1 responseData = (T1)serializer.Deserialize(memoryStream);

                    return responseData;
                }

#pragma warning restore CS8603 // Posible tipo de valor devuelto de referencia nulo
            }
            catch (Exception ex)
            {
                Exception exception = new("Failed" + ex.InnerException + "\n" + ex.Message);
                throw exception;
            }
        }

        public async Task<T1> PostRequest<T1, T2>(Uri requestUrl, T2 content)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(content);
                HttpContent contentHttp = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var CircuitBreakerPolicy = _circuitBreaker.GetCircuitBreaker();
                var TransientErrorRetryPolicy = _transientRetry.GetTransientRetry();
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                var resultData = await CircuitBreakerPolicy.ExecuteAsync(() =>
                        TransientErrorRetryPolicy.ExecuteAsync(() =>
                        httpClient.PostAsync(requestUrl, contentHttp))
                    );
                var response = await resultData.Content.ReadAsStringAsync();
#pragma warning disable CS8603 // Posible tipo de valor devuelto de referencia nulo
                return JsonConvert.DeserializeObject<T1>(response);
#pragma warning restore CS8603 // Posible tipo de valor devuelto de referencia nulo
            }
            catch (Exception ex)
            {
                Exception exception = new("Failed" + ex.InnerException + "\n" + ex.Message);
                throw exception;
            }
        }

        public async Task<T1> PostRequestWhitHeader<T1, T2>(Uri requestUrl, T2 content)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(content);
                HttpContent contentHttp = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var CircuitBreakerPolicy = _circuitBreaker.GetCircuitBreaker();
                var TransientErrorRetryPolicy = _transientRetry.GetTransientRetry();
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("UserName", _configuration["UserNameAPI"]);
                httpClient.DefaultRequestHeaders.Add("AuthenticationToken", _configuration["AuthenticationTokenAPI"]);
                var resultData = await CircuitBreakerPolicy.ExecuteAsync(() =>
                        TransientErrorRetryPolicy.ExecuteAsync(() =>
                        httpClient.PostAsync(requestUrl, contentHttp))
                    );
                var response = await resultData.Content.ReadAsStringAsync();
#pragma warning disable CS8603 // Posible tipo de valor devuelto de referencia nulo
                return JsonConvert.DeserializeObject<T1>(response);
#pragma warning restore CS8603 // Posible tipo de valor devuelto de referencia nulo
            }
            catch (Exception ex)
            {
                Exception exception = new("Failed" + ex.InnerException + "\n" + ex.Message);
                throw exception;
            }
        }

        public async Task<string> PostStringAsync<T>(Uri requestUrl, T content)
        {
            try
            {
                var CircuitBreakerPolicy = _circuitBreaker.GetCircuitBreaker();
                var TransientErrorRetryPolicy = _transientRetry.GetTransientRetry();
                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(_configuration["userSap"], _configuration["passwordSap"]),
                };
                var httpClient = new HttpClient(httpClientHandler);
                var resultData = await CircuitBreakerPolicy.ExecuteAsync(() =>
                        TransientErrorRetryPolicy.ExecuteAsync(() =>
                        httpClient.PostAsync(requestUrl, CreateHttpContent<T>(content)))
                    );
                var response = await resultData.Content.ReadAsStringAsync();
                return response;
            }
            catch (Exception ex)
            {
                Exception exception = new("Failed" + ex.InnerException + "\n" + ex.Message);
                throw exception;
            }
        }

        public async Task<T1> PutRequest<T1, T2>(Uri requestUrl, T2 content)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(content);
                HttpContent contentHttp = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var CircuitBreakerPolicy = _circuitBreaker.GetCircuitBreaker();
                var TransientErrorRetryPolicy = _transientRetry.GetTransientRetry();
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                var resultData = await CircuitBreakerPolicy.ExecuteAsync(() =>
                        TransientErrorRetryPolicy.ExecuteAsync(() =>
                        httpClient.PutAsync(requestUrl, contentHttp))
                    );
                var response = await resultData.Content.ReadAsStringAsync();
#pragma warning disable CS8603 // Posible tipo de valor devuelto de referencia nulo
                return JsonConvert.DeserializeObject<T1>(response);
#pragma warning restore CS8603 // Posible tipo de valor devuelto de referencia nulo
            }
            catch (Exception ex)
            {
                Exception exception = new("Failed" + ex.InnerException + "\n" + ex.Message);
                throw exception;
            }
        }

        private HttpContent CreateHttpContent<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content, MicrosoftDateFormatSettings);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
        private static JsonSerializerSettings MicrosoftDateFormatSettings
        {
            get
            {
                return new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                    Formatting = Formatting.Indented,
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                };
            }
        }

    }
}
