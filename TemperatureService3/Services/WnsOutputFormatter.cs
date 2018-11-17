using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TemperatureService3.ViewModels;

namespace TemperatureService3.Services
{
    public class WnsOutputFormatter : TextOutputFormatter
    {
        public WnsOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/xml"));

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanWriteType(Type type)
        {
            return typeof(SensorViewModel).IsAssignableFrom(type) ? base.CanWriteType(type) : false;
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            IServiceProvider serviceProvider = context.HttpContext.RequestServices;
            var logger = serviceProvider.GetService(typeof(ILogger<WnsOutputFormatter>)) as ILogger;

            var response = context.HttpContext.Response;

            context.HttpContext.Response.Headers.Add("X-WNS-Expires",
                (context.Object as SensorViewModel).LastUpdated.ToUniversalTime().AddMinutes(60).ToString("r"));

            var r = RazorWnsViewOutput.WnsOutput(context);
            r.Wait();

            return response.WriteAsync(r.Result);
        }
    }

    public static class RazorWnsViewOutput
    {
        public static async Task<string> WnsOutput(OutputFormatterWriteContext context)
        {
            var actionContext = new ActionContext(context.HttpContext, new RouteData(), new ActionDescriptor());

            var response = context.HttpContext.Response;

            var _razorViewEngine = context.HttpContext.RequestServices.GetService<IRazorViewEngine>();
            var _tempDataProvider = context.HttpContext.RequestServices.GetService<ITempDataProvider>();

            using (var sw = new StringWriter())
            {
                string viewName = "Sensor_WNS";
                var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = context.Object
                };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }
    }
}