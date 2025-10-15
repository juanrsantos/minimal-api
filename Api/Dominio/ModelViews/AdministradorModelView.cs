namespace MinimalApi.Dominio.ModelViews;

using MinimalApi.Dominio.Enuns;


public record AdministradorModelView
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public Perfil Perfil { get; set; }
}