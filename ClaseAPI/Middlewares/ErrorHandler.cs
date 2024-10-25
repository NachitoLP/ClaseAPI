using Azure;
using ClaseAPI.Extensions;
using System.Net;
using System.Text.Json;

namespace ClaseAPI.Middlewares
{
    public class ErrorHandler(RequestDelegate next)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);

            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = new Response<string>() { Succeeded = false, Message = error?.Message };

                switch (error)
                {
                    case ApiException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;


                }
                var result = JsonSerializer.Serialize(responseModel);
                await response.WriteAsync(result);

            }
        }
    }
}
