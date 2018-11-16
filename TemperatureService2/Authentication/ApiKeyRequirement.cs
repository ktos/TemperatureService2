using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TemperatureService2.Authentication
{
    public class ApiKeyRequirement : IAuthorizationRequirement
    {
        public string ApiKey { get; private set; }

        public ApiKeyRequirement(string apiKey)
        {
            ApiKey = apiKey;
        }
    }
}