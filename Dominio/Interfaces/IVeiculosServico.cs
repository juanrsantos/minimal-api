using MinimalApi.Dominio.Servicos;

namespace MinimalApi.Dominio.Interfaces;


public interface IVeiculosServico
{
    List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null);
    Veiculo? BuscarPorId(int id);
    void Incluir(Veiculo veiculo);
    void Atualizar(int id, Veiculo veiculo);

    void Apagar(Veiculo veiculo);
}