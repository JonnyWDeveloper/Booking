using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Booking.Web.MiddleWare
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class MiddlewareTest
    {
        private readonly RequestDelegate _next;

        public MiddlewareTest(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareTestExtensions
    {
        public static IApplicationBuilder UseMiddlewareTest(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MiddlewareTest>();
        }
    }
}
