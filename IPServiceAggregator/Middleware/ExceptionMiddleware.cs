using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IPServiceAggregator.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, ILogger<ExceptionMiddleware> logger)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception innerEx)
            {
                try
                {
                    try
                    {
                        logger.LogError(innerEx, "Uncaught exception.", null);
                        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        await httpContext.Response.WriteAsync("Error occured while handling the request.");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Log Writing Exception - " + ex.Message, innerEx);
                    }
                }
                catch (Exception Outer)
                {
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry(Outer.Message + " Stack Trace -" + Outer
                            .StackTrace, EventLogEntryType.Information, 101, 1);
                    }

                    if (Outer.InnerException != null)
                        using (EventLog eventLog = new EventLog("Application"))
                        {
                            eventLog.Source = "Application";
                            eventLog.WriteEntry(Outer.InnerException.Message + " Stack Trace Inner -" + Outer.InnerException
                                .StackTrace, EventLogEntryType.Information, 101, 1);
                        }

                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await httpContext.Response.WriteAsync("Error occured while handling the request.");
                }
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
