using Dominio.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enuns;
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

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculosServico _veiculoServico) =>
{
    var validacao = validaDTO(veiculoDTO);

    if (validacao.Erros.Count > 0)
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
    .WithSummary("Cria um novo veículo.")
    .WithDescription("Cria um novo veículo com os dados fornecidos no corpo da requisição.");




app.MapPut("/veiculos/{id}", (int id, [FromBody] VeiculoDTO veiculoDTO, IVeiculosServico _veiculoServico) =>
{
    var veiculoExistente = _veiculoServico.BuscarPorId(id);
    if (veiculoExistente == null)
        return Results.NotFound("Veículo não encontrado");

    var validacao = validaDTO(veiculoDTO);
    if (validacao.Erros.Count > 0)
        return Results.BadRequest(validacao);

    veiculoExistente.Nome = veiculoDTO.Nome;
    veiculoExistente.Marca = veiculoDTO.Marca;
    veiculoExistente.Ano = veiculoDTO.Ano;
    veiculoExistente.Cor = veiculoDTO.Cor;
    veiculoExistente.Placa = veiculoDTO.Placa;

    _veiculoServico.Atualizar(id, veiculoExistente);
    return Results.Ok(veiculoExistente);
}).WithTags("Veículos")
    .WithName("PutVeiculos")
    .WithSummary("Atualiza um veículo existente.")
    .WithDescription("Atualiza os dados de um veículo existente com base no ID fornecido e nos dados do corpo da requisição.");

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


app.MapDelete("/veiculos/{id}", (int id, IVeiculosServico _veiculoServico) =>
{
    var veiculo = _veiculoServico.BuscarPorId(id);
    if (veiculo == null)
        return Results.NotFound("Veículo não encontrado");

    _veiculoServico.Apagar(veiculo);
    return Results.Ok("Veículo apagado com sucesso");
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

#region 

app.MapGet("/administradores", ([FromQuery(Name = "pagina")] int? pagina, IAdministradorServico _adminServico) =>
{
    var paginaAtual = pagina ?? 1;
    var admins = new List<AdministradorModelView>();
    var administradores = _adminServico.Todos(paginaAtual);
    
    foreach (var admin in administradores)
    {
        admins.Add(new AdministradorModelView
        {
            Id = admin.Id,
            Nome = admin.Nome,
            Email = admin.Email,
            Perfil = Enum.TryParse<Perfil>(admin.Perfil, out var perfil) ? perfil : Perfil.User
        });
    }
    return Results.Ok(admins);
}).WithTags("Administradores")
    .WithName("GetAdministradores")
    .WithSummary("Obtém a lista de administradores paginada.")
    .WithDescription("Retorna uma lista de administradores. O parâmetro 'pagina' é opcional e indica o número da página para paginação (padrão: 1).");  
    
app.MapGet("/administradores/{id}", (int id, IAdministradorServico _adminServico) =>
{
    var admin = _adminServico.Todos().FirstOrDefault(a => a.Id == id);
    if (admin == null)
        return Results.NotFound("Administrador não encontrado");

    return Results.Ok(admin);
}).WithTags("Administradores")
    .WithName("GetAdministradorById")
    .WithSummary("Obtém um administrador pelo ID.")
    .WithDescription("Retorna os dados de um administrador específico com base no ID fornecido.");

app.MapPost("administradores", ([FromBody] AdministradorDTO adminDTO, IAdministradorServico _adminServico) =>
{
    var validacao = new ErrosDeValidacao
    {
        Erros = new List<string>()
    };

    if (string.IsNullOrEmpty(adminDTO.Nome) || adminDTO.Nome.Length < 3)
        validacao.Erros.Add("O nome do administrador deve ter pelo menos 3 caracteres.");
    if (string.IsNullOrEmpty(adminDTO.Email) || !adminDTO.Email.Contains("@"))
        validacao.Erros.Add("Um email válido é obrigatório.");
    if (string.IsNullOrEmpty(adminDTO.Senha) || adminDTO.Senha.Length < 6)
        validacao.Erros.Add("A senha deve ter pelo menos 6 caracteres.");
        
    if (validacao.Erros.Count > 0)
        return Results.BadRequest(validacao);
        

    var admin = new Administrador
    {
        Nome = adminDTO.Nome,
        Email = adminDTO.Email,
        Senha = adminDTO.Senha,
        Perfil = Perfil.Admin.ToString() ?? Perfil.Editor.ToString()

    };

    _adminServico.Incluir(admin);
    return Results.Created($"/administradores/{admin.Id}", admin);
}).WithTags("Administradores")
    .WithName("PostAdministradores")
    .WithSummary("Cria um novo administrador.")
    .WithDescription("Cria um novo administrador com os dados fornecidos no corpo da requisição.");
#endregion


app.Run();


