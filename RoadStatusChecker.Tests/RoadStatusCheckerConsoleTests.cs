using Moq;
using NUnit.Framework;
using RoadStatusChecker;
using RoadStatusChecker.Services.Exceptions;
using RoadStatusChecker.Services.Models;
using RoadStatusChecker.Services.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Tests
{
    public class RoadStatusCheckerConsoleTests
    {

        private RoadStatus validRoadStatus;

        [SetUp]
        public void Setup()
        {
            validRoadStatus = new RoadStatus
            {
                Id = "a2",
                DisplayName = "A2",
                StatusSeverity = "Good",
                StatusSeverityDescription = "No Exceptional Delays"
            };
        }

        [Test]
        public async Task GivenValidRoadIdIsSpecifiedWhenTheClientIsRunThenTheRoadDisplayNameShouldBeDisplayedAsync()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                var mockRoadStatusService = new Mock<IRoadStatusService>();
                mockRoadStatusService.Setup(r => r.GetRoadStatusAsync(It.IsAny<string>())).Returns(Task.FromResult(validRoadStatus));

                RoadStatusPrinter printer = new RoadStatusPrinter(mockRoadStatusService.Object);

                var result = await printer.PrintRoadStatusResult("A2");

                string expected = $"The status of the {validRoadStatus.DisplayName} is as follows";

                Assert.IsTrue(sw.ToString().Contains(expected));

            }
        }


        [Test]
        public async Task GivenValidRoadIdIsSpecifiedWhenTheClientIsRunThenTheStatusSeverityShouldBeDisplayedAsRoadStatusAsync()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                var mockRoadStatusService = new Mock<IRoadStatusService>();
                mockRoadStatusService.Setup(r => r.GetRoadStatusAsync(It.IsAny<string>())).Returns(Task.FromResult(validRoadStatus));

                RoadStatusPrinter printer = new RoadStatusPrinter(mockRoadStatusService.Object);

                var result = await printer.PrintRoadStatusResult("A2");

                string expected = $"Road Status is {validRoadStatus.StatusSeverity}";

                Assert.IsTrue(sw.ToString().Contains(expected));

            }
        }

        [Test]
        public async Task GivenValidRoadIdIsSpecifiedWhenTheClientIsRunThenTheStatusSeverityDescriptionShouldBeDisplayedAsRoadStatusDescriptionAsync()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                var mockRoadStatusService = new Mock<IRoadStatusService>();
                mockRoadStatusService.Setup(r => r.GetRoadStatusAsync(It.IsAny<string>())).Returns(Task.FromResult(validRoadStatus));

                RoadStatusPrinter printer = new RoadStatusPrinter(mockRoadStatusService.Object);

                var result = await printer.PrintRoadStatusResult("A2");

                string expected = $"Road Status Description is {validRoadStatus.StatusSeverityDescription}";

                Assert.IsTrue(sw.ToString().Contains(expected));

            }
        }

        [Test]
        public async Task GivenValidRoadIdIsSpecifiedWhenTheClientIsRunThenShouldReturnZeroSystemSuccessCodeAsync()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                var mockRoadStatusService = new Mock<IRoadStatusService>();
                mockRoadStatusService.Setup(r => r.GetRoadStatusAsync(It.IsAny<string>())).Returns(Task.FromResult(validRoadStatus));

                RoadStatusPrinter printer = new RoadStatusPrinter(mockRoadStatusService.Object);

                var result = await printer.PrintRoadStatusResult("A2");

                Assert.AreEqual(result, 0);

            }
        }

        [Test]
        public async Task GivenInvalidRoadIdIsSpecifiedWhenTheClientIsRunThenApplicationShouldDisplayInformativeErrorAsync()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                var mockRoadStatusService = new Mock<IRoadStatusService>();
                mockRoadStatusService.Setup(r => r.GetRoadStatusAsync(It.IsAny<string>())).Throws(new ApiException
                {
                    StatusCode = 404,
                    Error = new ApiErrorResponse
                    {
                        HttpStatus = "NotFound",
                        HttpStatusCode = 404,
                        Message = "The following road id is not recognised: A211"                        
                    }
                });

                RoadStatusPrinter printer = new RoadStatusPrinter(mockRoadStatusService.Object);

                var result = await printer.PrintRoadStatusResult("A211");

                string expected = "A211 is not a valid road";

                Assert.IsTrue(sw.ToString().Contains(expected));

            }
        }


        [Test]
        public async Task GivenInvalidRoadIdIsSpecifiedWhenTheClientIsRunThenShouldReturnNonZeroSystemErrorCodeAsync()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                var mockRoadStatusService = new Mock<IRoadStatusService>();
                mockRoadStatusService.Setup(r => r.GetRoadStatusAsync(It.IsAny<string>())).Throws(new ApiException
                {
                    StatusCode = 404,
                    Error = new ApiErrorResponse
                    {
                        HttpStatus = "NotFound",
                        HttpStatusCode = 404,
                        Message = "The following road id is not recognised: A211"
                    }
                });

                RoadStatusPrinter printer = new RoadStatusPrinter(mockRoadStatusService.Object);

                var result = await printer.PrintRoadStatusResult("A211");

                Assert.AreEqual(result, 1);

            }
        }


    }
}