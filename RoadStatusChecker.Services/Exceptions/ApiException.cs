using RoadStatusChecker.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadStatusChecker.Services.Exceptions
{
    [Serializable]
    public class ApiException : Exception
    {
        public int StatusCode { get; set; }
        public ApiErrorResponse Error { get; set; }
    }
}
