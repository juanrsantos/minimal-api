namespace MinimalApi.Dominio.Servicos;

using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.DTOs;
using MinimalApi.Infraestutura.Db;

public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _db;

    public AdministradorServico(DbContexto contexto) 
    {
        _db = contexto;
    }
    public bool? Login(LoginDTO loginDTO)
    {
        var admin = _db.Administradores
            .FirstOrDefault(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha);
        
        return admin != null;
    }


}
