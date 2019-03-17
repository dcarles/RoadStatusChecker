using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RoadStatusChecker.Services.Clients
{
    public class HttpClientHandler : IHttpClientHandler
    {
        private HttpClient _client = new HttpClient();

        public HttpResponseMessage Get(string url)
        {
            return GetAsync(url).Result;
        }

        public HttpResponseMessage Post(string url, HttpContent content)
        {
            return PostAsync(url, content).Result;
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await _client.GetAsync(url);
        }

        public async Task<string> GetStringAsync(string url)
        {
            return await _client.GetStringAsync(url);
        }

        public async Task<HttpResponseMessage> SendAsync(string url)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                return await _client.SendAsync(request);
        }

        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            return await _client.PostAsync(url, content);
        }

        public void SetHeaders(Dictionary<string,string> headers)
        {
            foreach (var header in headers)
            {
                _client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }            
        }

        public void ClearHeaders()
        {
            _client.DefaultRequestHeaders.Clear();
        }
    }
}
