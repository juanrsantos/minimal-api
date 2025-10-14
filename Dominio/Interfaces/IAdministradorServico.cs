namespace MinimalApi.Dominio.Interfaces;

using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;


public interface IAdministradorServico
{
    // Defina os métodos que o serviço deve implementar
    // Exemplo:
    // Administrador Autenticar(string email, string senha);
    bool? Login(LoginDTO loginDTO);

    Administrador Incluir(Administrador admin);

    List<Administrador> Todos(int? pagina = 1);
}       