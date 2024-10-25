using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using ClaseAPI.Models;
using Microsoft.Win32;

namespace ClaseAPI.Controllers
{
	public class RegisterController : Controller
	{
		public IActionResult Index()
        {
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Index(IFormCollection formData)
		{
			SqlConnectionStringBuilder connectionString = new();

            var cs = DBHelper.GetConnectionString();

            bool emailExists;
			var checkEmail = formData["UserEmail"].ToString();

			using (SqlConnection connection = new SqlConnection(cs))
			{
				connection.Open();
				SqlCommand checkEmailCmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Users WHERE UserEmail = @UserEmail", connection);
				checkEmailCmd.Parameters.AddWithValue("@UserEmail", checkEmail);
				int emailCount = (int)checkEmailCmd.ExecuteScalar();
				emailExists = emailCount > 0;
			}

			if (emailExists)
			{
				return BadRequest("El email ya está registrado en la base de datos.");
			}

			var password = formData["UserPassword"];

            var hashedPassword = PasswordHasher.HashPassword(password);
            var parts = hashedPassword.Split(':');
            var passwordHash = parts[1];
            var passwordSalt = parts[0];

            var user = new User
			{
				UserName = formData["UserName"],
				UserSurname = formData["UserSurname"],
				UserEmail = formData["UserEmail"],
				UserPassword = passwordHash,
				UserPasswordSalt = passwordSalt
			};

			using (SqlConnection connection = new SqlConnection(cs))
			{
				connection.Open();

				SqlCommand cmd = new SqlCommand("INSERT INTO dbo.Users(UserName, UserSurname, UserEmail, UserPassword, UserPasswordSalt) VALUES(@UserName, @UserSurname, @UserEmail, @UserPassword, @UserPasswordSalt)", connection);
				cmd.Parameters.AddWithValue("@UserName", user.UserName);
				cmd.Parameters.AddWithValue("@UserSurname", user.UserSurname);
				cmd.Parameters.AddWithValue("@UserEmail", user.UserEmail);
				cmd.Parameters.AddWithValue("@UserPassword", user.UserPassword);
                cmd.Parameters.AddWithValue("@UserPasswordSalt", user.UserPasswordSalt);
                cmd.ExecuteNonQuery();

				connection.Close();
			}

			return Redirect("https://localhost:7230/swagger/index.html");
        }
	}
}
