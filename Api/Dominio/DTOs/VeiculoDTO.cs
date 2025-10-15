namespace MinimalApi.Dominio.DTOs;

public class VeiculoDTO
{
    public string Nome { get; set; } = null!;
    public string Marca { get; set; } = null!;
    public int Ano { get; set; }
    public string Cor { get; set; } = null!;

    public string Placa { get; set; } = null!;
}