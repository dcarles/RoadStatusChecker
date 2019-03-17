using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoadStatusChecker.Services.Clients;
using RoadStatusChecker.Services.Models;
using RoadStatusChecker.Services.Services;
using System;
using System.Threading.Tasks;


namespace RoadStatusChecker
{
    public class Program
    {
        public static IConfigurationRoot Configuration { get; set; }


        public static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        private static async Task MainAsync(string[] args)
        {
            var serviceProvider = ConfigureDI();
            var roadStatusService = serviceProvider.GetService<IRoadStatusService>();

            var road = "";

            if (args.Length >= 1)
                road = args[0];   

            var roadStatusPrinter = new RoadStatusPrinter(roadStatusService);
            var result = await roadStatusPrinter.PrintRoadStatusResult(road);

            Environment.Exit(result);
        }

        private static ServiceProvider ConfigureDI()
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
                builder.AddUserSecrets<Program>();
            }

            Configuration = builder.Build();

            IServiceCollection services = new ServiceCollection();

            //Map the implementations of your classes here ready for DI
            services.AddTransient<IHttpClientHandler, HttpClientHandler>();
            services
                .Configure<TFLAccountDetails>(Configuration.GetSection(nameof(TFLAccountDetails)))
                .AddOptions()
                .AddLogging()
                .AddSingleton<IRoadStatusService, RoadStatusService>()
                .BuildServiceProvider();

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }
    }
}
