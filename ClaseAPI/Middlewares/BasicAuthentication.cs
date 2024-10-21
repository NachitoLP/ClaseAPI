using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ClaseAPI.Middlewares
{
    //public async Task InvokeAsync(HttpContext context)
    //{
    //    if (!context.Request.Headers.ContainsKey("Authorization"))
    //    {
    //        context.Response.StatusCode = 401;
    //        await context.Response.WriteAsync("Acceso denegado.");

    //        return;
    //    }
    //    else
    //    {
    //        var header = context.Request.Headers["Authorization"];
    //        var encodedCredentials = header.ToString().Substring(6);
    //        var credentialsUncoded = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));

    //        string[] userypwd = credentialsUncoded.Split(":");
    //        var user = userypwd[0];
    //        var password = userypwd[1];

    //        return;
    //        //await next(context);
    //    }
    //}
}