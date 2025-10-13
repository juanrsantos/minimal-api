using Dominio.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Servicos;
using MinimalApi.DTOs;
using MinimalApi.Infraestutura.Db;

#region builder

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext usando connection string do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? @"Server=(localdb)\mssqllocaldb;Database=MinimalApiDb;Trusted_Connection=True;";
builder.Services.AddDbContext<MinimalApi.Infraestutura.Db.DbContexto>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculosServico, VeiculoServico>();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region app
var app = builder.Build();

// Habilitar Swagger apenas em Development (pode ativar em outros ambientes se desejar)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
#endregion

// Exemplo simplificado (no Program.cs, após app.Build())
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<DbContexto>();
db.Database.Migrate(); // opcional: aplicar migrations em runtime

if (!db.Administradores.Any(a => a.Email == "administrador@teste.com"))
{
    var admin = new Administrador
    {
        Nome = "Administrador",
        Email = "administrador@teste.com",
        Senha = "123456",
        Perfil = "Admin"
    };
    db.Administradores.Add(admin);
    db.SaveChanges();
}
#region Home
app.MapGet("/", () => new Home());
#endregion

#region Veiculos

ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
{
    var mensagens = new ErrosDeValidacao();
    if (string.IsNullOrEmpty(veiculoDTO.Nome) || veiculoDTO.Nome.Length < 3)
        mensagens.Erros.Add("O nome do veículo deve ter pelo menos 3 caracteres.");

    if (string.IsNullOrEmpty(veiculoDTO.Marca))
        mensagens.Erros.Add("A marca do veículo é obrigatória.");

    return mensagens;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDTO  veiculoDTO, IVeiculosServico _veiculoServico) =>
{
    var validacao = validaDTO(veiculoDTO);

if(validacao.Erros.Count > 0)
        return Results.BadRequest(validacao);

    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano,
        Cor = veiculoDTO.Cor,
        Placa = veiculoDTO.Placa
    };

    _veiculoServico.Incluir(veiculo);
    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
}).WithTags("Veículos")
    .WithName("PostVeiculos")
    .WithSummary("Crie um novo veículo.")
    .WithDescription("Retorna uma lista de veículos. O parâmetro 'pagina' é opcional e indica o número da página para paginação (padrão: 1).");

/// <summary>
/// Obtém a lista de veículos paginada.
/// </summary>
/// <param name="pagina">Número da página para paginação (opcional, padrão: 1).</param>
/// <returns>Lista de veículos.</returns>
app.MapGet("/veiculos",
    ([FromQuery(Name = "pagina")] int? pagina, IVeiculosServico _veiculoServico) =>
    {
        // Se não informado, define página como 1
        var paginaAtual = pagina ?? 1;
        var veiculos = _veiculoServico.Todos(paginaAtual, null, null);
        return Results.Ok(veiculos);
    })
    .WithTags("Veículos")
    .WithName("GetVeiculos")
    .WithSummary("Obtém a lista de veículos paginada.")
    .WithDescription("Retorna uma lista de veículos. O parâmetro 'pagina' é opcional e indica o número da página para paginação (padrão: 1).");
  


app.MapGet("/veiculos/{id}", (int id, IVeiculosServico _veiculoServico) =>
{
    var veiculo = _veiculoServico.BuscarPorId(id);
    if (veiculo == null)
        return Results.NotFound("Veículo não encontrado");

    return Results.Ok(veiculo);
}).WithTags("Veículos");
#endregion

#region login

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico _adminServico) =>
{
    if (_adminServico.Login(loginDTO) == true)
        return Results.Ok("logado com sucesso");
    else
        return Results.Unauthorized();
});
#endregion



app.Run();


