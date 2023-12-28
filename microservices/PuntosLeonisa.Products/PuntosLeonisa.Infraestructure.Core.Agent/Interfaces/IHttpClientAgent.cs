

namespace PuntosLeonisa.Infraestructure.Core.Agent.Interfaces
{
    public interface IHttpClientAgent
    {
        Task<T1> GetRequest<T1>(Uri requestUrl);
        Task<T1> GetRequestXml<T1>(Uri requestUrl);
        Task<T1> PostRequest<T1, T2>(Uri requestUrl, T2 content);
        Task<T1> PostRequestWhitHeader<T1, T2>(Uri requestUrl, T2 content);
        Task<T1> PutRequest<T1, T2>(Uri requestUrl, T2 content);
        Task<string> PostStringAsync<T>(Uri requestUrl, T content);
    }
}
