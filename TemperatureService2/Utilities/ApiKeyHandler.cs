using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TemperatureService2.Models;

namespace TemperatureService2.Utilities
{
    public class ApiKeyHandler : AuthorizationHandler<ApiKeyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
        {
            try
            {
                if (context.Resource is AuthorizationFilterContext mvcContext)
                {
                    var apikey = mvcContext.HttpContext.Request.Headers["X-APIKEY"].FirstOrDefault();
                    if (string.IsNullOrEmpty(apikey))
                    {
                        using (var sr = new StreamReader(mvcContext.HttpContext.Request.Body))
                        {
                            var requestBody = sr.ReadToEnd();
                            var sensorData = JsonConvert.DeserializeObject<SensorDto>(requestBody);

                            if (sensorData.ApiKey == requirement.ApiKey)
                                context.Succeed(requirement);
                            else
                                context.Fail();
                        }
                    }
                    else if (apikey == requirement.ApiKey)
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }

                    return Task.CompletedTask;
                }
            }
            catch
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}