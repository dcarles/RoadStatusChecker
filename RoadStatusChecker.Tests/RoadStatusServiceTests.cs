using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RoadStatusChecker.Services.Clients;
using RoadStatusChecker.Services.Exceptions;
using RoadStatusChecker.Services.Models;
using RoadStatusChecker.Services.Services;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tests
{
    public class RoadStatusServiceTests
    {
        private string validJsonResponse;
        private string invalidJsonResponse;

        [SetUp]
        public void Setup()
        {
            validJsonResponse = @"[
    {
        '$type': 'Tfl.Api.Presentation.Entities.RoadCorridor, Tfl.Api.Presentation.Entities',
        'id': 'a2',
        'displayName': 'A2',
        'statusSeverity': 'Good',
        'statusSeverityDescription': 'No Exceptional Delays',
        'bounds': '[[-0.0857,51.44091],[0.17118,51.49438]]',
        'envelope': '[[-0.0857,51.44091],[-0.0857,51.49438],[0.17118,51.49438],[0.17118,51.44091],[-0.0857,51.44091]]',
        'url': '/Road/a2'
    }
]";

            invalidJsonResponse = @"{
    '$type': 'Tfl.Api.Presentation.Entities.ApiError, Tfl.Api.Presentation.Entities',
    'timestampUtc': '2019-03-16T20:21:17.3672617Z',
    'exceptionType': 'EntityNotFoundException',
    'httpStatusCode': 404,
    'httpStatus': 'NotFound',
    'relativeUri': '/Road/A211',
    'message': 'The following road id is not recognised: A211'
}";
        }

        [Test]
        public async Task GivenValidRoadIdWhenServiceCalledTheDetailsOfStatusShouldBeReturned()
        {
            // Given a valid road
            var road = "A2";

            // When we call the service
            var mockClient = new Mock<IHttpClientHandler>();
            var response = new HttpResponseMessage
            {
                Content = new StringContent(validJsonResponse),
                StatusCode = HttpStatusCode.OK
            };
            mockClient.Setup(r => r.SendAsync(It.IsAny<string>())).Returns(Task.FromResult(response));

            IOptions<TFLAccountDetails> dummySecrets = Options.Create(new TFLAccountDetails());

            var roadStatusService = new RoadStatusService(mockClient.Object, dummySecrets);

            var roadStatus = await roadStatusService.GetRoadStatusAsync(road);

            //Then detail of the road status should be returned
            Assert.AreEqual(road, roadStatus.DisplayName);
            Assert.AreEqual("Good", roadStatus.StatusSeverity);
            Assert.AreEqual("No Exceptional Delays", roadStatus.StatusSeverityDescription);
        }


        [Test]
        public void GivenInvalidRoadIdWhenServiceCalledAnApiExceptionShouldBeThrownWithNotFoundExceptionStatus()
        {
            // Given an ivalid road
            var road = "A211";

            // When we call the service
            var mockClient = new Mock<IHttpClientHandler>();
            var response = new HttpResponseMessage
            {
                Content = new StringContent(invalidJsonResponse),
                StatusCode = HttpStatusCode.NotFound
            };
            mockClient.Setup(r => r.SendAsync(It.IsAny<string>())).Returns(Task.FromResult(response));

            IOptions<TFLAccountDetails> dummySecrets = Options.Create(new TFLAccountDetails());

            var roadStatusService = new RoadStatusService(mockClient.Object, dummySecrets);

            //Then an ApiException should be thrown with 404 (Not Found) status
            ApiException ex = Assert.ThrowsAsync<ApiException>(async () => await roadStatusService.GetRoadStatusAsync(road));
            Assert.AreEqual(404, ex.StatusCode);
            Assert.AreEqual("EntityNotFoundException", ex.Error.ExceptionType);
            Assert.AreEqual("NotFound", ex.Error.HttpStatus);
            Assert.AreEqual("The following road id is not recognised: A211", ex.Error.Message);          
        }

    }
}