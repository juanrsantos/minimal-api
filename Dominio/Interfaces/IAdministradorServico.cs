namespace MinimalApi.Dominio.Interfaces;

using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;


public interface IAdministradorServico
{
    // Defina os métodos que o serviço deve implementar
    // Exemplo:
    // Administrador Autenticar(string email, string senha);
    bool? Login(LoginDTO loginDTO);

    /// <summary>
    /// Autentica e retorna o Administrador quando as credenciais estão corretas.
    /// </summary>
    MinimalApi.Dominio.Entidades.Administrador? Autenticar(string email, string senha);

    Administrador Incluir(Administrador admin);

    List<Administrador> Todos(int? pagina = 1);
}       