using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace RoadStatusChecker.Services.Clients
{
    public interface IHttpClientHandler
    {
        HttpResponseMessage Get(string url);
        HttpResponseMessage Post(string url, HttpContent content);
        Task<HttpResponseMessage> GetAsync(string url);
        Task<string> GetStringAsync(string url);
        Task<HttpResponseMessage> SendAsync(string url);
        Task<HttpResponseMessage> PostAsync(string url, HttpContent content);
        void SetHeaders(Dictionary<string, string> headers);
        void ClearHeaders();
    }
}
