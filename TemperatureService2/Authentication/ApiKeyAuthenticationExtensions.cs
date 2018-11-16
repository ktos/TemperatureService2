using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
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

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "API Key User") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var at = new AuthenticationTicket(principal, ApiKeyAuthenticationDefaults.AuthenticationScheme);

            var apikey = Context.Request.Headers["X-APIKEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apikey))
            {
                // workaround for https://github.com/aspnet/Security/issues/1638
                var ms = new MemoryStream();
                await Request.Body.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);

                var requestBody = await new StreamReader(ms).ReadToEndAsync();
                ms.Seek(0, SeekOrigin.Begin);
                Request.Body = ms;

                SensorDto sensorData = null;
                try
                {
                    sensorData = JsonConvert.DeserializeObject<SensorDto>(requestBody);
                }
                catch (JsonReaderException)
                {
                    sensorData = null;
                }

                if (sensorData == null)
                {
                    return AuthenticateResult.NoResult();
                }
                else
                {
                    if (sensorData.ApiKey == Options.ApiKey)
                    {
                        Context.User.AddIdentity(new ClaimsIdentity(ApiKeyAuthenticationDefaults.AuthenticationScheme));
                        return AuthenticateResult.Success(at);
                    }
                    else
                        return AuthenticateResult.Fail("API key is invalid");
                }
            }
            else if (apikey == Options.ApiKey)
            {
                Context.User.AddIdentity(new ClaimsIdentity(ApiKeyAuthenticationDefaults.AuthenticationScheme));
                return AuthenticateResult.Success(at);
            }
            else
            {
                return AuthenticateResult.NoResult();
            }
        }
    }
}