using RoadStatusChecker.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RoadStatusChecker.Services.Services
{
    public interface IRoadStatusService
    {
        Task<RoadStatus> GetRoadStatusAsync(string roadId);
    }
}
