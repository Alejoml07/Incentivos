using Logistic.Infrastructure.Agents.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.DTO;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace Logistic.Infrastructure.Agents.AgentsImpl
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
                return JsonConvert.DeserializeObject<T1>(response);
#pragma warning restore CS8603 // Posible tipo de valor devuelto de referencia nulo
            }
            catch (Exception ex)
            {
                Exception exception = new("Failed" + ex.InnerException + "\n" + ex.Message);
                throw exception;
            }
        }



        public async Task<string> GetRequestXml<T1>(Uri requestUrl)
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
                //using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(response)))
                //{
                //    T1 responseData = (T1)serializer.Deserialize(memoryStream);

                return response;
                //}

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
                httpClient.DefaultRequestHeaders.Add("Authorization", _configuration["Authorization"]);
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

        public async Task<T1> PostRequestScanner<T1, T2>(Uri requestUrl, T2 content)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(content);
                HttpContent contentHttp = new StringContent(jsonString, Encoding.UTF8, "application/json");

                var CircuitBreakerPolicy = _circuitBreaker.GetCircuitBreaker();
                var TransientErrorRetryPolicy = _transientRetry.GetTransientRetry();
                var httpClient = _httpClientFactory.CreateClient();

                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("Authorization", _configuration["AuthorizationScanner"]);

                var resultData = await CircuitBreakerPolicy.ExecuteAsync(() =>
                    TransientErrorRetryPolicy.ExecuteAsync(() =>
                        httpClient.PostAsync(requestUrl, contentHttp))
                );

                var response = await resultData.Content.ReadAsStringAsync();

                // Configuración personalizada para manejar enteros grandes
                var settings = new JsonSerializerSettings
                {
                    MaxDepth = 128, // Ajusta la profundidad si es necesario
                    TypeNameHandling = TypeNameHandling.None,
                    MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                    // Aquí se ignoran errores de tamaño de entero grande
                    Error = (sender, args) =>
                    {
                        if (args.ErrorContext.Error.Message.Contains("JSON integer"))
                        {
                            // Ignorar errores relacionados con enteros grandes
                            args.ErrorContext.Handled = true;
                        }
                    }
                };

                // Deserializa usando los settings personalizados
                return JsonConvert.DeserializeObject<T1>(response, settings);
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
                //httpClient.DefaultRequestHeaders.Add("user", _configuration["userOp"]);
                //httpClient.DefaultRequestHeaders.Add("password", _configuration["passwordOp"]);
                httpClient.DefaultRequestHeaders.Add("Authorization", _configuration["AuthorizationOP"]);
                //httpClient.DefaultRequestHeaders.Add("Bearer",);
                //convierte el usuario y contraseña en base64
                var byteArray = Encoding.ASCII.GetBytes($"{_configuration["userOp"]}:{_configuration["passwordOp"]}");
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

        public Task<bool> SendMail(EmailDTO email)
        {
            var url = $"{_configuration["UrlMail"]}";
            var result = this.PostStringAsync<dynamic>(new Uri(url), email);

            return Task.FromResult(true);

        }


        public async Task<T1> PostRequestWithToken<T1, T2>(Uri requestUrl, T2 content, string token)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(content);
                HttpContent contentHttp = new StringContent(jsonString, Encoding.UTF8, "application/json");

                var CircuitBreakerPolicy = _circuitBreaker.GetCircuitBreaker();
                var TransientErrorRetryPolicy = _transientRetry.GetTransientRetry();
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("Bearer", token);
                //convierte el usuario y contraseña en base64
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
    }
}
