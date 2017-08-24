using Microsoft.AspNetCore.Builder;

namespace  WebAPICore.Sample.Middleware
{
    public static class LogHeaderMiddlewareExtensions
    {
        public static IApplicationBuilder UseLogHeaderMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogHeaderMiddleware>();
        }
    }
}
