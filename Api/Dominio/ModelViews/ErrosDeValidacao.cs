namespace Dominio.ModelViews;

public struct ErrosDeValidacao
{
    public ErrosDeValidacao()
    {
    }

    public List<string> Erros { get; set; } = new List<string>();
}   