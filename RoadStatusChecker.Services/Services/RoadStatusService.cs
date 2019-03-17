using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RoadStatusChecker.Services.Clients;
using RoadStatusChecker.Services.Exceptions;
using RoadStatusChecker.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace RoadStatusChecker.Services.Services
{
    public class RoadStatusService : IRoadStatusService
    {
        private readonly IHttpClientHandler _restClient;
        private readonly string _baseUrl;    
        private readonly TFLAccountDetails _secrets;

        public RoadStatusService(IHttpClientHandler restClient, IOptions<TFLAccountDetails> secrets)
        {
            _baseUrl = "https://api.tfl.gov.uk";
            _restClient = restClient;
            _secrets = secrets.Value ?? throw new ArgumentNullException(nameof(secrets));
        }

        public async Task<RoadStatus> GetRoadStatusAsync(string roadId)
        {
            var response = await _restClient.SendAsync(_baseUrl + $"/Road/{roadId}?app_id={_secrets.AppId}&app_key={_secrets.AppKey}");
            
            if (response.IsSuccessStatusCode == false)
            {
                var content = JsonConvert.DeserializeObject <ApiErrorResponse> (await response.Content.ReadAsStringAsync());

                throw new ApiException
                {
                    StatusCode = (int)response.StatusCode,
                    Error = content
                };
            }

            var roadResponse = JsonConvert.DeserializeObject<List<RoadStatus>>(await response.Content.ReadAsStringAsync());

            return roadResponse.First();

        }
        
    }
}
