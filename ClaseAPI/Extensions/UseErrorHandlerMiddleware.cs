using ClaseAPI.Middlewares;

namespace ClaseAPI.Extensions
{
    public static class UseErrorHandlerMiddleware
    {
        public static void UseErrorHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandler>();
        }
    }
}
