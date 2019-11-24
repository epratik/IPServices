using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace IPServiceAggregator.Filters
{
    public class RateLimitFilter: ActionFilterAttribute
    {
        public int Seconds { get; set; }
        private IMemoryCache cache;
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            cache = context.HttpContext.RequestServices.GetService<IMemoryCache>();

            var ipAddress = context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress;

            var memoryCacheKey = ipAddress;

            if (!cache.TryGetValue(memoryCacheKey, out bool entry))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(Seconds));

                cache.Set(memoryCacheKey, true, cacheEntryOptions);
            }
            else
            {
                context.Result = new ContentResult
                {
                    Content = $"Requests are limited to 1, every {Seconds} seconds.",
                };

                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            }
        }

    }
}
