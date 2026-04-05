using System.ComponentModel.DataAnnotations;

namespace SistemaLivros.Models;

public class Livro
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O título é obrigatório")]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    public string Autor { get; set; } = string.Empty;

    [Range(1500, 2100)]
    public int Ano { get; set; }
}