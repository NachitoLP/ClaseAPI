using ClaseAPI.Extensions;
using ClaseAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Agrega tu middleware de autenticación aquí si es necesario
// app.UseMiddleware<BasicAuthenticationHandlerMiddleware>("Test");

app.UseHttpsRedirection();

// Aquí es donde debes agregar UseRouting
app.UseRouting(); // Agregado aquí

app.UseAuthorization();

app.UseErrorHandler();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();

