using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using TemperatureService2.Models;

namespace TemperatureService2.Authentication
{
    public static class ApiKeyAuthenticationDefaults
    {
        public static string AuthenticationScheme = "APIKey";
    }

    public static class ApiKeyAuthenticationExtensions
    {
        public static AuthenticationBuilder AddApiKeyAuthentication(this AuthenticationBuilder builder, Action<ApiKeyAuthenticationOptions> configureOptions) => builder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationDefaults.AuthenticationScheme, ApiKeyAuthenticationDefaults.AuthenticationScheme, configureOptions);
    }

    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string ApiKey { get; set; }
    }

    internal class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        //protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        //{
        //    var principal = new System.Security.Claims.ClaimsPrincipal();
        //    var at = new AuthenticationTicket(principal, ApiKeyAuthenticationDefaults.AuthenticationScheme);

        //    var apikey = Context.Request.Headers["X-APIKEY"].FirstOrDefault();
        //    if (string.IsNullOrEmpty(apikey))
        //    {
        //        using (var sr = new StreamReader(Context.Request.Body))
        //        {
        //            var requestBody = sr.ReadToEnd();
        //            var sensorData = JsonConvert.DeserializeObject<SensorDto>(requestBody);

        //            if (sensorData == null)
        //            {
        //                return Task.CompletedTask;

        //                //return AuthenticateResult.NoResult();
        //            }
        //            else
        //            {
        //                if (sensorData.ApiKey == Options.ApiKey)
        //                {
        //                    Context.User.AddIdentity(new System.Security.Claims.ClaimsIdentity(ApiKeyAuthenticationDefaults.AuthenticationScheme));
        //                    //return AuthenticateResult.Success(at);
        //                }
        //                //else
        //                //return AuthenticateResult.Fail("API key is invalid");
        //            }
        //        }
        //    }
        //    else if (apikey == Options.ApiKey)
        //    {
        //        Context.User.AddIdentity(new System.Security.Claims.ClaimsIdentity(ApiKeyAuthenticationDefaults.AuthenticationScheme));
        //        //return AuthenticateResult.Success(at);
        //    }
        //    else
        //    {
        //        //return AuthenticateResult.NoResult();
        //    }

        //    return Task.CompletedTask;
        //}

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "user") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var at = new AuthenticationTicket(principal, ApiKeyAuthenticationDefaults.AuthenticationScheme);

            var apikey = Context.Request.Headers["X-APIKEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apikey))
            {
                using (var sr = new StreamReader(Context.Request.Body))
                {
                    var requestBody = sr.ReadToEnd();
                    var sensorData = JsonConvert.DeserializeObject<SensorDto>(requestBody);

                    if (sensorData == null)
                    {
                        return AuthenticateResult.NoResult();
                    }
                    else
                    {
                        if (sensorData.ApiKey == Options.ApiKey)
                        {
                            Context.User.AddIdentity(new System.Security.Claims.ClaimsIdentity(ApiKeyAuthenticationDefaults.AuthenticationScheme));
                            return AuthenticateResult.Success(at);
                        }
                        else
                            return AuthenticateResult.Fail("API key is invalid");
                    }
                }
            }
            else if (apikey == Options.ApiKey)
            {
                Context.User.AddIdentity(new System.Security.Claims.ClaimsIdentity(ApiKeyAuthenticationDefaults.AuthenticationScheme));
                return AuthenticateResult.Success(at);
            }
            else
            {
                return AuthenticateResult.NoResult();
            }

            // build the claims and put them in "Context"; you need to import the Microsoft.AspNetCore.Authentication package
            return AuthenticateResult.NoResult();
        }
    }
}