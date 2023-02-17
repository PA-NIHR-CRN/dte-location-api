using LocationApi.Common;
using Microsoft.AspNetCore.Builder;

namespace LocationApi.Extensions
{
    public static class CustomMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
        
        public static IApplicationBuilder UseCustomHeaderForwarderHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HeaderForwarderMiddleware>();
        }
    }
}