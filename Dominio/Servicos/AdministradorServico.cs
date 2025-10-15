namespace MinimalApi.Dominio.Servicos;

using System.Collections.Generic;
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

    public Administrador Incluir(Administrador admin)
    {
      
        _db.Administradores.Add(admin);
        _db.SaveChanges();
        return admin;
    }

    public bool? Login(LoginDTO loginDTO)
    {
        var admin = _db.Administradores
            .FirstOrDefault(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha);
        
        return admin != null;
    }

    public Administrador? Autenticar(string email, string senha)
    {
        var admin = _db.Administradores
            .FirstOrDefault(a => a.Email == email && a.Senha == senha);
        return admin;
    }

    public List<Administrador> Todos(int? pagina = 1)
    {
        var query = _db.Administradores.AsQueryable();
        int pageSize = 10;
        if (pagina != null && pagina > 0)
        {
            query = query.Skip(((int)pagina - 1) * pageSize).Take(pageSize);
        }   
        return query.ToList();
    }
}
