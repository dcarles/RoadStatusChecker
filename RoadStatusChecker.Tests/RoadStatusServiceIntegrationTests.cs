using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RoadStatusChecker.Services.Clients;
using RoadStatusChecker.Services.Exceptions;
using RoadStatusChecker.Services.Models;
using RoadStatusChecker.Services.Services;
using System;
using System.Threading.Tasks;

namespace Tests
{
    public class RoadStatusServiceIntegrationTests
    {

        private IConfigurationRoot Configuration { get; set; }
        private ServiceProvider ServiceProvider { get; set; }

        [SetUp]
        public void Setup()
        {
            ServiceProvider = ConfigureDI();
        }

        [Test]
        public async Task GivenValidRoadIdWhenServiceCalledTheDetailsOfStatusShouldBeReturned()
        {
            // Given a valid road
            var road = "A2";

            // When we call the service         
            var roadStatusService = ServiceProvider.GetService<IRoadStatusService>();
            var roadStatus = await roadStatusService.GetRoadStatusAsync(road);

            //Then detail of the road status should be returned
            Assert.AreEqual(road, roadStatus.DisplayName);
            Assert.AreEqual(road.ToLower(), roadStatus.Id);
            Assert.AreEqual("/Road/a2", roadStatus.Url);
            Assert.IsNotEmpty(roadStatus.StatusSeverity);
            Assert.IsNotEmpty(roadStatus.StatusSeverityDescription);
        }


        [Test]
        public void GivenInvalidRoadIdWhenServiceCalledAnApiExceptionShouldBeThrownWithNotFoundExceptionStatus()
        {
            // Given an ivalid road
            var road = "INVALID_ROAD";

            // When we call the service        
            var roadStatusService = ServiceProvider.GetService<IRoadStatusService>();

            //Then an ApiException should be thrown with 404 (Not Found) status
            ApiException ex = Assert.ThrowsAsync<ApiException>(async () => await roadStatusService.GetRoadStatusAsync(road));
            Assert.AreEqual(404, ex.StatusCode);
            Assert.AreEqual("EntityNotFoundException", ex.Error.ExceptionType);
            Assert.AreEqual("NotFound", ex.Error.HttpStatus);
            Assert.AreEqual("The following road id is not recognised: INVALID_ROAD", ex.Error.Message);          
        }


        private ServiceProvider ConfigureDI()
        {
            var devEnvironmentVariable = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

            //Determines the working environment as IHostingEnvironment is unavailable in a console app
            var isDevelopment = string.IsNullOrEmpty(devEnvironmentVariable) ||
                                devEnvironmentVariable.ToLower() == "development";

            var builder = new ConfigurationBuilder();

            // tell the builder to look for the appsettings.json file
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            //only add secrets in development
            if (isDevelopment)
            {
                builder.AddUserSecrets<RoadStatusServiceIntegrationTests>();
            }

            Configuration = builder.Build();

            IServiceCollection services = new ServiceCollection();

            //Map the implementations of your classes here ready for DI
            services.AddTransient<IHttpClientHandler, HttpClientHandler>();
            services
                .Configure<TFLAccountDetails>(Configuration.GetSection(nameof(TFLAccountDetails)))
                .AddOptions()               
                .AddSingleton<IRoadStatusService, RoadStatusService>()
                .BuildServiceProvider();

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

    }
}