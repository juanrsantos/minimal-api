using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Infraestutura.Db;
using MinimalApi.DTOs;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Servicos;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext usando connection string do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? @"Server=(localdb)\mssqllocaldb;Database=MinimalApiDb;Trusted_Connection=True;";
builder.Services.AddDbContext<MinimalApi.Infraestutura.Db.DbContexto>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();

var app = builder.Build();

// Exemplo simplificado (no Program.cs, após app.Build())
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<DbContexto>();
db.Database.Migrate(); // opcional: aplicar migrations em runtime

if (!db.Administradores.Any(a => a.Email == "administrador@teste.com"))
{
    var admin = new Administrador {
        Nome = "Administrador",
        Email = "administrador@teste.com",
        Senha = "123456", 
        Perfil = "Admin"
    };
    db.Administradores.Add(admin);
    db.SaveChanges();
}


app.MapGet("/", () => "Hello World!");

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico _adminServico) =>
{
    if (_adminServico.Login(loginDTO) == true)
        return Results.Ok("logado com sucesso");
    else
        return Results.Unauthorized();
});

app.Run();


