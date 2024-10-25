using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.Data.SqlClient;

namespace ClaseAPI.Middlewares
{
    public class BasicAuthenticationHandlerMiddleware
    {

        private readonly RequestDelegate next;
        private readonly string realm;
        public BasicAuthenticationHandlerMiddleware(RequestDelegate next, string realm)
        {
            this.next = next;
            this.realm = realm;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Acceso denegado.");
                return;
            }

            var header = context.Request.Headers["Authorization"].ToString();
            if (header.StartsWith("Basic "))
            {
                var encodedCredentials = header.Substring("Basic ".Length).Trim();
                var credentialsUncoded = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
                var userpwd = credentialsUncoded.Split(":");

                if (userpwd.Length != 2)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Formato de credenciales inválido.");
                    return;
                }

                var username = userpwd[0];
                var password = userpwd[1];

                if (!await ValidateCredentialsAsync(username, password))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                await next(context);
            }
            else
            {
                context.Response.StatusCode = 401;
                context.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{realm}\"";
                await context.Response.WriteAsync("Formato de autorización inválido.");
            }
        }

        private async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            try
            {
                string sql_connection = DBHelper.GetConnectionString();
                using (var connection = new SqlConnection(sql_connection))
                {
                    await connection.OpenAsync();

                    var command = new SqlCommand("SELECT UserPassword, UserPasswordSalt FROM Users WHERE UserName = @Username", connection);
                    command.Parameters.Add(new SqlParameter("@Username", username));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (!reader.Read())
                        {
                            return false; // Usuario no encontrado
                        }

                        string storedHash = (string)reader["UserPassword"];
                        string storedSalt = (string)reader["UserPasswordSalt"];

                        return PasswordHasher.VerifyPassword(password, storedHash, storedSalt);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}