using MinimalApi.Dominio.Entidades;

namespace MinimalApi.Infraestutura.Token;

public interface ITokenService
{
    string GerarToken(Administrador admin);
}