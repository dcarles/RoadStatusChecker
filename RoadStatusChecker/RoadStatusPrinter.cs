using RoadStatusChecker.Services.Exceptions;
using RoadStatusChecker.Services.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RoadStatusChecker
{
    public class RoadStatusPrinter
    {
        public readonly IRoadStatusService _roadStatusService;

        public RoadStatusPrinter(IRoadStatusService roadStatusService)
        {
            _roadStatusService = roadStatusService;
        }

        public async Task<int> PrintRoadStatusResult(string road)
        {

            if (String.IsNullOrEmpty(road))
            {
                Console.WriteLine("Road id argument is missing. Command should be RoadStatus.exe [RoadId]");
                return 1;
            }

            try
            {              
                var roadStatus = await _roadStatusService.GetRoadStatusAsync(road);

                Console.WriteLine($"The status of the {roadStatus.DisplayName} is as follows:");
                Console.WriteLine($"Road Status is {roadStatus.StatusSeverity}");
                Console.WriteLine($"Road Status Description is {roadStatus.StatusSeverityDescription}");

                return 0;
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == 404)
                {
                    Console.WriteLine($"{road} is not a valid road");
                }
                else
                {
                    //TODO: LOG with details from Exception
                    Console.WriteLine($"There was an error running the application");
                }

                return 1;
            }
        }

    }
}
