namespace MinimalApi.Dominio.Servicos;


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



public class Veiculo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Marca { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public int Ano { get; set; }

    public string Cor { get; set; } = null!;
    public string Placa { get; set; } = null!;

}