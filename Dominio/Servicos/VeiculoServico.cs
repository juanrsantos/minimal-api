
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Infraestutura.Db;

namespace MinimalApi.Dominio.Servicos;

public class VeiculoServico : IVeiculosServico
{
    private readonly DbContexto _db;
    public VeiculoServico(DbContexto contexto)
    {
        _db = contexto;
    }

    public void Apagar(Veiculo veiculo)
    {
        _db.Veiculos.Remove(veiculo);
        _db.SaveChanges();
    }

    public void Atualizar(int id, Veiculo veiculo)
    {
        _db.Veiculos.Update(veiculo);
        _db.SaveChanges();
    }

    public Veiculo? BuscarPorId(int id)
    {
      return _db.Veiculos.Where(x => x.Id == id).FirstOrDefault();
    }

    public void Incluir(Veiculo veiculo)
    {
       _db.Veiculos.Add(veiculo);
       _db.SaveChanges();
    }

    public List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        var query = _db.Veiculos.AsQueryable();

        if (!string.IsNullOrEmpty(nome))
            query = query.Where(v => v.Nome.Contains(nome));

        if (!string.IsNullOrEmpty(marca))
            query = query.Where(v => v.Marca.Contains(marca));

        int pageSize = 10;

        if(pagina != null && pagina > 0)
        {
            query = query.Skip(((int) pagina - 1) * pageSize).Take(pageSize);
        }
        return query.ToList();
    }
}