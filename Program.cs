using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext usando connection string do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=(localdb)\\mssqllocaldb;Database=MinimalApiDb;Trusted_Connection=True;";
builder.Services.AddDbContext<MinimalApi.Infraestutura.Db.DbContexto>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (MinimalApi.DTOs.LoginDTO loginDTO) =>
{
    if (loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "123456")
    {
        return Results.Ok("logado com sucesso");
    }
    else
    {
        return Results.Unauthorized();
    }
});

app.Run();


