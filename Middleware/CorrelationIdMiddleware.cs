using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context; 

namespace VulnerableApp.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var cid = Guid.NewGuid().ToString();
            context.Response.Headers["X-Correlation-ID"] = cid;
            
            using (LogContext.PushProperty("CorrelationId", cid))
            {
                await _next(context);
            }
        }
    }
}
