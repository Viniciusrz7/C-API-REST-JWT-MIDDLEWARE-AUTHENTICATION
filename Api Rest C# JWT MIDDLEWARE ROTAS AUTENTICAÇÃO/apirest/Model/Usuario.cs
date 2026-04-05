using System.ComponentModel.DataAnnotations;

namespace SistemaLivros.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty; // Hash
    public string Perfil { get; set; } = "CLIENTE"; // AGENTE, ADMIN, etc.
}